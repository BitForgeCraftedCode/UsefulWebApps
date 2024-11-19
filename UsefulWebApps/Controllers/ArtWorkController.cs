using Microsoft.AspNetCore.Mvc;
using System.IO;
using UsefulWebApps.Models.ViewModels.ArtWork;

namespace UsefulWebApps.Controllers
{
    public class ArtWorkController : Controller
    {
        private IWebHostEnvironment Environment;
        public ArtWorkController(IWebHostEnvironment _environment)
        {
            Environment = _environment;
        }
        public IActionResult Index(int page)
        {
            IEnumerable<string> paths = Directory.EnumerateFiles(Path.Combine(this.Environment.WebRootPath, "images/artwork/"));
            List<string> files = new List<string>();
            List<string> filesToShow = new List<string>();
            if (page == 0)
            {
                page = 1;
            }
           
            int limit = 4;
            int offset = (limit * (page - 1));
            //count is total number of images
            int count = paths.Count();
            int totalPages = (int)Math.Ceiling(count / (double)limit);

            foreach (string path in paths)
            {
                files.Add(Path.GetFileName(path));
            }

            if (page < totalPages)
            {
                //works until last page because last page may have less than 10 elements
                filesToShow = files.GetRange(offset, limit);
            }
            //for last page skip the rest
            else if (page == totalPages) 
            {
                filesToShow = new List<string>(files.Skip(offset));
        
            }
            
            ArtWorkVM artWorkVM = new() 
            { 
                FilesToShow = filesToShow,
                CurrentPage = page,
                TotalPages = totalPages,
            };
            return View(artWorkVM);
        }
    }
}
