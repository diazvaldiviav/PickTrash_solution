using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Enums;

namespace WebApplication1.Models.Dtos.Request
{
    public class RequestDTO
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int? DriverId { get; set; }
        public string? ClientName { get; set; }
        public string? DriverName { get; set; }
        public string PickupAddress { get; set; }
        public string DropoffAddress { get; set; }
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string TrashType { get; set; }
        public decimal EstimatedWeight { get; set; }
        public string SpecialInstructions { get; set; }
        public decimal? ProposedPrice { get; set; }
        public decimal? FinalPrice { get; set; }
    }

    public class CreateRequestDTO
    {
        [Required]
        public int DriverId { get; set; }

        [Required]
        public string PickupAddress { get; set; }



        [Required]
        public string DropoffAddress { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required]
        public string TrashType { get; set; }

        [Required]
        [Range(0.1, 10000)]
        public decimal EstimatedWeight { get; set; }

        public string? SpecialInstructions { get; set; }

        [Range(0.01, 10000)]
        public decimal? ProposedPrice { get; set; }
    }


    public class UpdateRequestDTO
    {
        public string? PickupAddress { get; set; }
        public string? DropoffAddress { get; set; }
        public double? PickupLatitude { get; set; }
        public double? PickupLongitude { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public string? TrashType { get; set; }
        public decimal? EstimatedWeight { get; set; }
        public string? SpecialInstructions { get; set; }
        public decimal? ProposedPrice { get; set; }
        public int? TransportCategoryId { get; set; }
    }

    public class RequestStatusUpdateDTO
    {
        [Required]
        public RequestStatus NewStatus { get; set; }

        public string? Reason { get; set; }

        public decimal? FinalPrice { get; set; }
    }

    public class RequestListItemDTO
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string? DriverName { get; set; }
        public string PickupAddress { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime ScheduledDate { get; set; }
        public decimal? ProposedPrice { get; set; }
        public string TrashType { get; set; }
    }
}
