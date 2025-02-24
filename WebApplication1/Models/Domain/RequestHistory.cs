using WebApplication1.Models.Enums;

namespace WebApplication1.Models.Domain
{
    public class RequestHistory
    {
        public int Id { get; set; }

        // Relación con Request
        public int RequestId { get; set; }

        // Información del cambio
        public RequestStatus PreviousStatus { get; set; }
        public RequestStatus NewStatus { get; set; }

        // Tracking
        public DateTime ChangedAt { get; set; }
        public string ChangedBy { get; set; }
        public string? ChangeReason { get; set; }
        public RequestStatus OldStatus { get; set; }

        // Información de precio (si hubo cambios)
        public decimal? PreviousPrice { get; set; }
        public decimal? NewPrice { get; set; }

        public Request Request { get; set; }
    }
}
