using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Domain;

namespace WebApplication1.Models.Dtos.UserDto
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string DefaultAddress { get; set; } = string.Empty;
    }

    public class DriverDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public decimal Rating { get; set; }
    }

    public class AvailableDriverDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Rating { get; set; }
        public double Distance { get; set; } // Distancia al cliente
        public IEnumerable<string> VehicleTypes { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class UpdateLocationDTO
    {
        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }
    }

    public class UpdateLocationByAddressDTO
    {
        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Address { get; set; } = string.Empty;
    }

  
}
