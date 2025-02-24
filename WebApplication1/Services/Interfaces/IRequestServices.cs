using WebApplication1.Models.Dtos.Request;
using WebApplication1.Models.Enums;

namespace WebApplication1.Services.Interfaces
{
    public interface IRequestService
    {
        // Operaciones básicas
        Task<RequestDTO> GetRequestByIdAsync(int id);
        Task<IEnumerable<RequestDTO>> GetAllRequestsAsync();
        Task<RequestDTO> CreateRequestAsync(CreateRequestDTO createDto, int clientId);
        Task<RequestDTO> UpdateRequestAsync(int id, UpdateRequestDTO updateDto);

        // Operaciones específicas por rol
        Task<IEnumerable<RequestDTO>> GetClientRequestsAsync(int clientId);
        Task<IEnumerable<RequestDTO>> GetDriverRequestsAsync(int driverId);
        Task<IEnumerable<RequestDTO>> GetAvailableRequestsForDriverAsync(int driverId, double latitude, double longitude);

        // Operaciones de estado
        Task<RequestDTO> AcceptRequestAsync(int requestId, int driverId);
        Task<RequestDTO> StartServiceAsync(int requestId, double driverLat, double driverLng);
        Task<RequestDTO> CompleteServiceAsync(int requestId);
        Task<RequestDTO> CancelRequestAsync(int requestId, string reason);

        // Validaciones
        Task<bool> CanDriverAcceptRequestAsync(int driverId, int requestId);
        Task<bool> CanModifyRequestAsync(int requestId);
        Task<IEnumerable<RequestDTO>> GetDriverRequestsAsync(int driverId, RequestStatus? status = null);
    }
}
