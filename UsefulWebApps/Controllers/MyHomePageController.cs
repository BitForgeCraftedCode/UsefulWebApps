using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using UsefulWebApps.Models.MyHomePage;
using UsefulWebApps.Models.ViewModels.MyHomePage;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Controllers
{
    [Authorize(Roles = "StandardUser, Admin")]
    [AutoValidateAntiforgeryToken]
    public class MyHomePageController : Controller
    {
        private IWebHostEnvironment Environment;
        private readonly IUnitOfWork _unitOfWork;
        public MyHomePageController(IWebHostEnvironment _environment, IUnitOfWork unitOfWork)
        {
            Environment = _environment;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

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
            
            MyHomePageVM myHomePageVM = new() 
            { 
                SlideShowImagesToDisplay = userSlideShowImages,
                DefaultSlideShowImagesToDisplay = filesToShow,
                QuickLinksToDisplay = userQuickLinks
            };
            return View(myHomePageVM);
        }

        public async Task<IActionResult> SelectQuickLinks() 
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

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

        [HttpPost]
        public async Task<IActionResult> SelectQuickLinks(SelectQuickLinksVM selectQuickLinksVM)
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            string userName = currentUser.FindFirstValue(ClaimTypes.Name);

            if (ModelState.IsValid) 
            {
                bool success = await _unitOfWork.QuickLinks.UpdateQuickLinks(userId, userName, selectQuickLinksVM);
                if (success)
                {
                    TempData["success"] = "Quick links updated successfully";
                }
                else
                {
                    TempData["error"] = "Update quick link error. Please try again.";
                }
                return View(selectQuickLinksVM);
            }
            TempData["error"] = "Update quick link error. Please try again.";
            return View(selectQuickLinksVM);
        }
    }
}
