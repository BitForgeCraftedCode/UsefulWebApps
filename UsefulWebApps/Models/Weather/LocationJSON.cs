using System.Text.Json.Serialization;

namespace UsefulWebApps.Models.Weather
{
    public record class LocationJSON(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("lat")] double Latitude,
        [property: JsonPropertyName("lon")] double Longitude,
        [property: JsonPropertyName("country")] string Country,
        [property: JsonPropertyName("state")] string State);
}
