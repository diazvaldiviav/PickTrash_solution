namespace WebApplication1.Geolocalization
{
    // Clases compartidas para Geocoding y Directions
    public class GoogleApiResponse
    {
        public string Status { get; set; } = string.Empty;
    }

    // Para Geocoding
    public class GoogleGeocodingResponse : GoogleApiResponse
    {
        public List<GeocodingResult> Results { get; set; } = new();
    }

    public class GeocodingResult
    {
        public Geometry Geometry { get; set; } = new();
    }

    public class Geometry
    {
        public Location Location { get; set; } = new();
    }

    public class Location
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    // Para Directions
    public class GoogleDirectionsResponse : GoogleApiResponse
    {
        public List<Route> Routes { get; set; } = new();
    }

    public class Route
    {
        public List<Leg> Legs { get; set; } = new();
        public OverviewPolyline Overview_Polyline { get; set; } = new();
    }

    public class Leg
    {
        public TextValue Distance { get; set; } = new();
        public TextValue Duration { get; set; } = new();
    }

    public class OverviewPolyline
    {
        public string Points { get; set; } = string.Empty;
    }

    public class TextValue
    {
        public string Text { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
