using WebApplication1.Models.Domain;
using WebApplication1.Models.Enums;

namespace WebApplication1.Data.Repositories.Interfaces
{
    public interface IRequestRepository : IGenericRepository<Request>
    {
        // Consultas específicas para Request
        Task<IEnumerable<Request>> GetRequestsByClientIdAsync(int clientId);
        Task<IEnumerable<Request>> GetRequestsByDriverIdAsync(int driverId);
        Task<IEnumerable<Request>> GetRequestsByStatusAsync(int driverId, RequestStatus status);
        Task<IEnumerable<Request>> GetPendingRequestsInAreaAsync(double latitude, double longitude, double radiusInKm);

        // Validaciones de negocio
        Task<bool> HasOverlappingRequestsAsync(int driverId, DateTime scheduledDate, TimeSpan duration);
        Task<bool> CanModifyRequestAsync(int requestId); // Regla de 48 horas

        // Operaciones de estado
        Task UpdateStatusAsync(int requestId, RequestStatus newStatus, string changedBy, string? reason = null);
    }
}
