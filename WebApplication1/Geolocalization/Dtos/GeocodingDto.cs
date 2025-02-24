namespace WebApplication1.Geolocalization.Dtos
{
    public class TrackingUpdateDTO
    {
        public int RequestId { get; set; }
        public double DriverLatitude { get; set; }
        public double DriverLongitude { get; set; }
        public int EstimatedMinutes { get; set; }
        public double DistanceToPickup { get; set; }  // en metros
        public bool RouteRecalculated { get; set; }
        public string EncodedPolyline { get; set; } = string.Empty;
    }

    public class DirectionsResponseDTO
    {
        public string EncodedPolyline { get; set; } = string.Empty;
        public int DurationInSeconds { get; set; }
        public int DistanceInMeters { get; set; }
    }

    public class LocationUpdateDTO
    {
        public int RequestId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
