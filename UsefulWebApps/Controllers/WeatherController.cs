using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using UsefulWebApps.Helpers.Weather;
using UsefulWebApps.Models.ViewModels.Weather;
using UsefulWebApps.Models.Weather;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Controllers
{
    [Authorize(Roles = "StandardUser, Admin")]
    [AutoValidateAntiforgeryToken]
    public class WeatherController : Controller
    {
        private static string apiKey = ManageAPIKey.GetAPIKey();
        private static readonly HttpClient client = new HttpClient();

        private readonly IUnitOfWork _unitOfWork;
        public WeatherController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Locations locationObj)
        {
            //get location lat and lon
            string jsonLocation = string.Empty;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (locationObj.Country.ToUpper() == "US" && locationObj.State.ToUpper() != "NA")
            {
                jsonLocation = await client.GetStringAsync($"http://api.openweathermap.org/geo/1.0/direct?q={locationObj.City},{locationObj.State},{locationObj.Country}&limit=1&appid={apiKey}");
            }
            else
            {
                jsonLocation = await client.GetStringAsync($"http://api.openweathermap.org/geo/1.0/direct?q={locationObj.City},{locationObj.Country}&limit=1&appid={apiKey}");
            }
            List<LocationJSON> location = JsonSerializer.Deserialize<List<LocationJSON>>(jsonLocation);
            //get weather for lat and lon
            string jsonCurrentWeather = string.Empty;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string lat = location[0].Latitude.ToString();
            string lon = location[0].Longitude.ToString();  
            jsonCurrentWeather = await client.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=imperial&appid={apiKey}");
            CurrentWeatherJSON currentWeather = JsonSerializer.Deserialize<CurrentWeatherJSON>(jsonCurrentWeather);
            Console.WriteLine(currentWeather.Main.temp);
            CurrentWeatherVM currentWeatherVM = new() 
            { 
                LocationJSON = location[0],
                CurrentWeatherJSON = currentWeather 
            };
            
            return PartialView("_CurrentWeatherPartial", currentWeatherVM);
        }

        public IActionResult AddLocations()
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            Locations location = new() 
            { 
                UserId = userId
            };
            return View(location);
        }

        [HttpPost]
        public async Task<IActionResult> AddLocations(Locations locationObj)
        {
            Console.WriteLine(ModelState.IsValid);
            //get location lat and lon
            string jsonLocation = string.Empty;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (locationObj.Country.ToUpper() == "US" && locationObj.State.ToUpper() != "NA")
            {
                jsonLocation = await client.GetStringAsync($"http://api.openweathermap.org/geo/1.0/direct?q={locationObj.City},{locationObj.State},{locationObj.Country}&limit=1&appid={apiKey}");
            }
            else
            {
                jsonLocation = await client.GetStringAsync($"http://api.openweathermap.org/geo/1.0/direct?q={locationObj.City},{locationObj.Country}&limit=1&appid={apiKey}");
            }
            //always a list of lenght 1 -- limit 1 on api call above
            List<LocationJSON> locationList = JsonSerializer.Deserialize<List<LocationJSON>>(jsonLocation);
            //LocationJSON location = locationList[0];
            Locations location = new() 
            {
                City = locationList[0].Name,
                Latitude = locationList[0].Latitude,
                Longitude = locationList[0].Longitude,
                Country = locationList[0].Country,
                State = locationList[0].State,
                UserId = locationObj.UserId,
                IsDefault = locationObj.IsDefault,
            };
            bool success = await _unitOfWork.Locations.Add(location);
            Console.WriteLine("success " + success);
            return View();
            
        }
    }
}
