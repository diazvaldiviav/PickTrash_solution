namespace WebApplication1.Models.Domain
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Plate { get; set; } = string.Empty;
        public int Year { get; set; }
        public int TransportCategoryId { get; set; }

        // Relaciones
        public TransportCategory TransportCategory { get; set; } = null!;
        public ICollection<DriverVehicle> DriverVehicles { get; set; } = new List<DriverVehicle>();
    }
}
