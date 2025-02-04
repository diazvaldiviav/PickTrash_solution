using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApplication1.Helpers
{
    public class GoogleGeocodingService : IGeocodingService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleGeocodingService> _logger;
        private readonly HttpClient _httpClient;

        public GoogleGeocodingService(
            IConfiguration configuration,
            ILogger<GoogleGeocodingService> logger,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<(decimal Latitude, decimal Longitude)?> GetCoordinatesAsync(string address)
        {
            try
            {
                var apiKey = _configuration["GoogleMaps:ApiKey"];
                var encodedAddress = Uri.EscapeDataString(address);
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={apiKey}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Google API Response: {Content}", content); // Para debug

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<GoogleGeocodingResponse>(content, options);

                if (result?.Results != null && result.Results.Any() &&
                    result.Results[0]?.Geometry?.Location != null)
                {
                    var location = result.Results[0].Geometry.Location;
                    return ((decimal)location.Lat, (decimal)location.Lng);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error geocoding address: {Address}", address);
                throw;
            }
        }

        public async Task<string?> GetAddressAsync(decimal latitude, decimal longitude)
        {
            try
            {
                var apiKey = _configuration["GoogleMaps:ApiKey"];
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={apiKey}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GoogleGeocodingResponse>(content);

                return result?.Results?.FirstOrDefault()?.FormattedAddress;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reverse geocoding coordinates: {Latitude}, {Longitude}", latitude, longitude);
                throw;
            }
        }
    }

    // Clases para deserialización
    public class GoogleGeocodingResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("results")]
        public List<GeocodingResult> Results { get; set; } = new();
    }

    public class GeocodingResult
    {
        [JsonPropertyName("formatted_address")]
        public string FormattedAddress { get; set; } = string.Empty;

        [JsonPropertyName("geometry")]
        public Geometry Geometry { get; set; } = new();
    }

    public class Result
    {
        public string? FormattedAddress { get; set; }
        public Geometry? Geometry { get; set; }
    }

    public class Geometry
    {
        [JsonPropertyName("location")]
        public Location Location { get; set; } = new();
    }

    public class Location
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lng")]
        public double Lng { get; set; }
    }
}
