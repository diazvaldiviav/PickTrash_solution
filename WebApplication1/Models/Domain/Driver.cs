namespace WebApplication1.Models.Domain
{
    public class Driver
    {
        public int Id { get; set; }
        public bool IsAvailable { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Rating { get; set; }

        // Relación con User
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Relación muchos a muchos con Vehicle
        public ICollection<DriverVehicle> DriverVehicles { get; set; } = new List<DriverVehicle>();
        public ICollection<Request> Requests { get; set; } = new List<Request>();
    }
}
