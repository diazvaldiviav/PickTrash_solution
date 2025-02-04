using WebApplication1.Models.Enums;

namespace WebApplication1.Models.Domain
{
    public class Request
    {
        public int Id { get; set; }

        // Relaciones principales
        public int ClientId { get; set; }        
        public int? DriverId { get; set; }
        public string ClientName { get; set; } = string.Empty; 
        public string DriverName { get; set; } = string.Empty;


        // Información de ubicación
        public string PickupAddress { get; set; }
        public string DropoffAddress { get; set; }
        public decimal PickupLatitude { get; set; }
        public decimal PickupLongitude { get; set; }

        // Información temporal
        public DateTime CreatedAt { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Estado y categoría
        public RequestStatus Status { get; set; }
        public string LastModifiedBy { get; set; } = string.Empty;
        public DateTime LastModifiedAt { get; set; }


        // Detalles del servicio
        public string TrashType { get; set; }
        public decimal EstimatedWeight { get; set; }
        public string SpecialInstructions { get; set; }

        // Información de precio
        public decimal? ProposedPrice { get; set; }
        public decimal? FinalPrice { get; set; }

        // Tracking
        public DateTime LastModified { get; set; }

        public Client Client { get; set; }

        public Driver Driver { get; set; }
    }
}
