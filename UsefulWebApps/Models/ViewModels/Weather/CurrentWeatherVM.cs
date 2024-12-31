using UsefulWebApps.Models.Weather;

namespace UsefulWebApps.Models.ViewModels.Weather
{
    public class CurrentWeatherVM
    {
        public Locations Location { get; set; }
        public CurrentWeatherJSON CurrentWeatherJSON { get; set; }
    }
}
