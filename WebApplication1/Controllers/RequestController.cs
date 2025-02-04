using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Common.Exceptions;
using WebApplication1.Models.Domain;
using WebApplication1.Models.Dtos.Request;
using WebApplication1.Models.Enums;
using WebApplication1.Services.Interfaces;
using WebApplication1.Data.Repositories.Interfaces;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly ILogger<RequestController> _logger;
        private readonly IDriverRepository _driverRepository;

        public RequestController(
            IRequestService requestService,
            ILogger<RequestController> logger,
            IDriverRepository driverRepository

            )
        {
            _requestService = requestService;
            _logger = logger;
            _driverRepository = driverRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RequestDTO>>> GetAll()
        {
            try
            {
                var requests = await _requestService.GetAllRequestsAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all requests");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<RequestDTO>> GetById(int id)
        {
            try
            {
                var request = await _requestService.GetRequestByIdAsync(id);
                return Ok(request);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting request {RequestId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<RequestDTO>> Create([FromBody] CreateRequestDTO createDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var request = await _requestService.CreateRequestAsync(createDto, userId);
                return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating request");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("client/my-requests")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<IEnumerable<RequestDTO>>> GetClientRequests()
        {
            try
            {
                var clientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var requests = await _requestService.GetClientRequestsAsync(clientId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client requests");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("driver/available")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult<IEnumerable<RequestDTO>>> GetAvailableRequests(
            [FromQuery] double latitude,
            [FromQuery] double longitude)
        {
            try
            {
                var driverId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var requests = await _requestService.GetAvailableRequestsForDriverAsync(
                    driverId, latitude, longitude);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available requests");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/accept")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult<RequestDTO>> AcceptRequest(int id)
        {
            try
            {
                var driverId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var request = await _requestService.AcceptRequestAsync(id, driverId);
                return Ok(request);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting request {RequestId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/start")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult<RequestDTO>> StartService(int id)
        {
            try
            {
                var request = await _requestService.StartServiceAsync(id);
                return Ok(request);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting service for request {RequestId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/complete")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult<RequestDTO>> CompleteService(int id)
        {
            try
            {
                var request = await _requestService.CompleteServiceAsync(id);
                return Ok(request);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing service for request {RequestId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/cancel")]
        [Authorize]
        public async Task<ActionResult<RequestDTO>> CancelRequest(
            int id,
            [FromBody] string reason)
        {
            try
            {
                var request = await _requestService.CancelRequestAsync(id, reason);
                return Ok(request);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling request {RequestId}", id);
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpGet("driver/my-requests")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult<IEnumerable<RequestDTO>>> GetMyRequests(
        [FromQuery] RequestStatus? status = null)
        {
            // Obtener el userId del token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            try
            {
                
                // Obtener el driver asociado al usuario
                var driver = await _driverRepository.GetByIdAsync(userId);
                if (driver == null)
                {
                    return NotFound("No se encontró el perfil de conductor para este usuario");
                }

                // Obtener las solicitudes del conductor
                var requests = await _requestService.GetDriverRequestsAsync(driver.Id, status);

                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting driver requests for user {UserId}", userId);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
