using Microsoft.AspNetCore.Mvc;

namespace UsefulWebApps.Controllers
{
    public class ArtWorkController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
