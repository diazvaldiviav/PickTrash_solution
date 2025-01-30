namespace WebApplication1.Models.Dtos.Vehicle
{
    public class CreateVehicleDTO
    {
        public string Plate { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Color { get; set; } = string.Empty;
        public string TransportCategoryName { get; set; } = string.Empty;
    }

    // DTOs/Vehicle/VehicleResponseDTO.cs
    public class VehicleResponseDTO
    {
        public int Id { get; set; }
        public string Plate { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Color { get; set; } = string.Empty;
        public string TransportCategoryName { get; set; } = string.Empty;
    }
}
