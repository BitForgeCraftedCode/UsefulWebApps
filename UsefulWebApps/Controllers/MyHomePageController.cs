using Microsoft.AspNetCore.Mvc;
using UsefulWebApps.Models.ViewModels.MyHomePage;

namespace UsefulWebApps.Controllers
{
    public class MyHomePageController : Controller
    {
        private IWebHostEnvironment Environment;
        public MyHomePageController(IWebHostEnvironment _environment)
        {
            Environment = _environment;
        }
        public IActionResult Index()
        {
            IEnumerable<string> paths = Directory.EnumerateFiles(Path.Combine(this.Environment.WebRootPath, "images/customhomepage/cityskylines/"));
           
            List<string> filesToShow = new List<string>();
            foreach (string path in paths)
            {
                filesToShow.Add(Path.GetFileName(path));
            }

            IEnumerable<string> shortCutPaths = Directory.EnumerateFiles(Path.Combine(this.Environment.WebRootPath, "icons/"));

            List<string> shortCutsToShow = new List<string>();
            foreach (string path in shortCutPaths)
            {
                shortCutsToShow.Add(Path.GetFileName(path));
            }


            MyHomePageVM myHomePageVM = new() 
            { 
                FilesToShow = filesToShow,
                ShortCutsToShow = shortCutsToShow
            };
            return View(myHomePageVM);
        }
    }
}
