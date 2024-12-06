using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

            IEnumerable<string> paths = Directory.EnumerateFiles(Path.Combine(this.Environment.WebRootPath, "images/customhomepage/nature/"));
           
            List<string> filesToShow = new List<string>();
            foreach (string path in paths)
            {
                filesToShow.Add(Path.GetFileName(path));
            }

            //get the users quick links
            List<QuickLinks> userQuickLinks = await _unitOfWork.QuickLinks.GetQuickLinksForUser(userId);
            
            MyHomePageVM myHomePageVM = new() 
            { 
                FilesToShow = filesToShow,
                QuickLinksToDisplay = userQuickLinks
            };
            return View(myHomePageVM);
        }
    }
}
