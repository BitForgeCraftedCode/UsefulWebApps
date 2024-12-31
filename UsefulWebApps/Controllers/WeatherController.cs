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
        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Locations> locations = (List<Locations>)await _unitOfWork.Locations.GetAllWhere("UserId", userId);
            LocationsVM locationsVM = new() { Locations = locations };
            return View(locationsVM);
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
            if (ModelState.IsValid)
            {
                //get location lat and lon
                string jsonLocation = string.Empty;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (locationObj.Country.ToUpper() == "US" && locationObj.State.ToUpper() != "NA")
                {
                    try
                    {
                        jsonLocation = await client.GetStringAsync($"http://api.openweathermap.org/geo/1.0/direct?q={locationObj.City},{locationObj.State},{locationObj.Country}&limit=1&appid={apiKey}");
                    }
                    catch(Exception e)
                    {
                        TempData["error"] = $"Sorry the api returned an error. {e.Message}";
                        return View();
                    }
                }
                else if (locationObj.Country.ToUpper() != "US" && locationObj.State.ToUpper() == "NA")
                {
                    try
                    {
                        jsonLocation = await client.GetStringAsync($"http://api.openweathermap.org/geo/1.0/direct?q={locationObj.City},{locationObj.Country}&limit=1&appid={apiKey}");
                    }
                    catch (Exception e)
                    {
                        TempData["error"] = $"Sorry the api returned an error. {e.Message}";
                        return View();
                    }
                }
                else
                {
                    TempData["error"] = "Sorry that was an invalid input";
                    return View();
                }
                //always a list of lenght 1 -- limit 1 on api call above
                List<LocationJSON> locationList = JsonSerializer.Deserialize<List<LocationJSON>>(jsonLocation);
                //if nothing in list the api didnt find the location.
                if (locationList.Count == 0)
                {
                    TempData["error"] = "Sorry the api could not find that location";
                    return View();
                }
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
                //add location to data base
                bool success = await _unitOfWork.Locations.Add(location);
                if (success) 
                {
                    TempData["success"] = "Location Added successfully.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Add location error. Try again.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Add location error. Try again.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Weather(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Locations location = await _unitOfWork.Locations.GetById(id);
            //get weather for lat and lon
            string jsonCurrentWeather = string.Empty;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                jsonCurrentWeather = await client.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?lat={location.Latitude}&lon={location.Longitude}&units=imperial&appid={apiKey}");
            }
            catch (Exception e) 
            {
                TempData["error"] = $"Sorry the api returned an error. {e.Message}";
                return RedirectToAction("Index");
            }
            
            CurrentWeatherJSON currentWeather = JsonSerializer.Deserialize<CurrentWeatherJSON>(jsonCurrentWeather);

            CurrentWeatherVM currentWeatherVM = new()
            {
                Location = location,
                CurrentWeatherJSON = currentWeather
            };

            return View(currentWeatherVM);
        }
    }
}
