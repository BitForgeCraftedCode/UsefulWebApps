using Microsoft.AspNetCore.Mvc;

namespace UsefulWebApps.Controllers
{
    public class MyHomePageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
