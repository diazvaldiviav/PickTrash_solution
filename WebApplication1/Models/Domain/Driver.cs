namespace WebApplication1.Models.Domain
{
    public class Driver
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        // Relación con User
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Relación muchos a muchos con Vehicle
        public ICollection<DriverVehicle> DriverVehicles { get; set; } = new List<DriverVehicle>();
    }
}
