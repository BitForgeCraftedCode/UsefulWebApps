using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsefulWebApps.Helpers.Weather;

namespace UsefulWebApps.Controllers
{
    [Authorize(Roles = "StandardUser, Admin")]
    [AutoValidateAntiforgeryToken]
    public class WeatherController : Controller
    {
        private static string apiKey = ManageAPIKey.GetAPIKey();
        private static string json = string.Empty;
        private static readonly HttpClient client = new HttpClient();
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetWeather()
        {

            string city = "Denville";
            string state = "NJ";
            string country = "US";
            json = await client.GetStringAsync($"http://api.openweathermap.org/geo/1.0/direct?q={city},{state},{country}&limit=1&appid={apiKey}");
            Console.WriteLine(json);
            return RedirectToAction("Index");
        }
    }
}
