using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsefulWebApps.Helpers;
using UsefulWebApps.Models.MyHomePage;
using UsefulWebApps.Models.ViewModels.MyHomePage;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Controllers
{
    [Authorize(Roles = "StandardUser, Admin")]
    [AutoValidateAntiforgeryToken]
    public class MyHomePageController : Controller
    {
        private HtmlSanitizer sanitizer = new HtmlSanitizer();
        private IWebHostEnvironment Environment;
        private readonly IUnitOfWork _unitOfWork;
        public MyHomePageController(IWebHostEnvironment _environment, IUnitOfWork unitOfWork)
        {
            Environment = _environment;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            //get users slideshow choice
            List<SlideShowImages> userSlideShowImages = await _unitOfWork.SlideShow.GetSlideShowImagesForUser(userId);
            //if user doesnt have a choice the space images will be displayed
            IEnumerable<string> paths = Directory.EnumerateFiles(Path.Combine(this.Environment.WebRootPath, "images/customhomepage/space/"));
           
            List<string> filesToShow = new List<string>();
            foreach (string path in paths)
            {
                filesToShow.Add(Path.GetFileName(path));
            }
            
            //get the users quick links
            List<QuickLinks> userQuickLinks = await _unitOfWork.QuickLinks.GetQuickLinksForUser(userId);

            //get a random quote
            Quotes randomQuote = await _unitOfWork.Quotes.GetRandomRow();
            
            MyHomePageVM myHomePageVM = new() 
            { 
                SlideShowImagesToDisplay = userSlideShowImages,
                DefaultSlideShowImagesToDisplay = filesToShow,
                QuickLinksToDisplay = userQuickLinks,
                RandomQuote = randomQuote
            };
            return View(myHomePageVM);
        }

        public async Task<IActionResult> SelectQuickLinks() 
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            (
                List<QuickLinks> userQuickLinks, 
                List<QuickLinks> allQuickLinks
            ) result = await _unitOfWork.QuickLinks.GetQuickLinksForEditDisplay(userId);

            List<QuickLinks> userQuickLinks = result.userQuickLinks;
            List<QuickLinks> allQuickLinks = result.allQuickLinks;


            foreach (QuickLinks allql in allQuickLinks) 
            {
                if (userQuickLinks.Any(userql => userql.QuickLinkId == allql.QuickLinkId))
                {
                    allql.IsSelected = true;
                }
            }
            SelectQuickLinksVM selectQuickLinksVM = new() 
            { 
                AllQuickLinks = allQuickLinks
            };

            return View(selectQuickLinksVM);
        }

        //transaction method
        [HttpPost]
        public async Task<IActionResult> SelectQuickLinks(SelectQuickLinksVM selectQuickLinksVM)
        {
            string? userId = User.GetUserId();
            string? userName = User.GetUserName();
            if (string.IsNullOrEmpty(userId)) return NotFound();
            if (string.IsNullOrEmpty(userName)) return NotFound();

            if (ModelState.IsValid) 
            {
                await _unitOfWork.OpenConnectionAsync();
                await _unitOfWork.BeginTxnAsync();
                bool success = await _unitOfWork.QuickLinks.UpdateQuickLinks(userId, userName, selectQuickLinksVM);
                if (success)
                {
                    await _unitOfWork.CommitAsync();
                    TempData["success"] = "Quick links updated successfully";
                }
                else
                {
                    await _unitOfWork.RollbackAsync();
                    TempData["error"] = "Update quick link error. Please try again.";
                }
                return RedirectToAction("Index");
            }
            TempData["error"] = "Update quick link error. Please try again.";
            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> SelectSlideShow()
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            (SlideShowFolder userSlideShowFolder, List<SlideShowFolder> allSlideShowFolders) result = await _unitOfWork.SlideShow.GetSlideShowFoldersEditDisplay(userId);
            SlideShowFolder userSlideShowFolder = result.userSlideShowFolder;
            List<SlideShowFolder> allSlideShowFolders = result.allSlideShowFolders;
            //will be null if user has never picked a slideshow folder
            if (userSlideShowFolder != null)
            {
                foreach (SlideShowFolder slideShowFolder in allSlideShowFolders)
                {
                    if (slideShowFolder.FolderName == userSlideShowFolder.FolderName)
                    {
                        slideShowFolder.IsSelected = true;
                    }
                }
            }
            
            SelectSlideShowVM selectSlideShowVM = new() { SlideShowFolders = allSlideShowFolders };
            return View(selectSlideShowVM);
        }

        //transaction method
        [HttpPost]
        public async Task<IActionResult> SelectSlideShow(SelectSlideShowVM selectSlideShowVM)
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            if (ModelState.IsValid)
            {
                await _unitOfWork.OpenConnectionAsync();
                await _unitOfWork.BeginTxnAsync();
                bool success = await _unitOfWork.SlideShow.UpdateSlideShow(userId, selectSlideShowVM);
                if (success)
                {
                    await _unitOfWork.CommitAsync();
                    TempData["success"] = "Slideshow updated successfully";
                }
                else
                {
                    await _unitOfWork.RollbackAsync();
                    TempData["error"] = "Update slideshow error. Please try again.";
                }
                return RedirectToAction("Index");
            }
            TempData["error"] = "Update slideshow error. Please try again.";
            return RedirectToAction("Index");
        }

        public IActionResult CreateQuote()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuote(Quotes obj)
        {
            obj.Quote = sanitizer.Sanitize(obj.Quote);
            if (ModelState.IsValid) 
            {
                bool success = await _unitOfWork.Quotes.Add(obj);
                if (success)
                {
                    TempData["success"] = "Quote added successfully";
                }
                else
                {
                    TempData["error"] = "Add quote error. Please try again.";
                }
                return RedirectToAction("Index");
            }
            TempData["error"] = "Add quote error. Please try again.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditQuote(long? id) 
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Quotes quote = await _unitOfWork.Quotes.GetById(id);
            return View(quote);
        }

        [HttpPost]
        public async Task<IActionResult> EditQuote(Quotes obj)
        {
            obj.Quote = sanitizer.Sanitize(obj.Quote);
            if (ModelState.IsValid) 
            { 
                bool success = await _unitOfWork.Quotes.Update(obj);
                if (success)
                {
                    TempData["success"] = "Quote edited successfully.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Edit quote error. Try again.";
                return RedirectToAction("index");
            }
            TempData["error"] = "Edit quote error. Try again.";
            return RedirectToAction("index");
        }
    }
}
