using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Geolocalization.Services.Interfaces;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrackingController : ControllerBase
    {
        private readonly ITrackingService _trackingService;
        private readonly IRequestService _requestService;
        private readonly ILogger<TrackingController> _logger;

        public TrackingController(
            ITrackingService trackingService,
            IRequestService requestService,
            ILogger<TrackingController> logger)
        {
            _trackingService = trackingService;
            _requestService = requestService;
            _logger = logger;
        }

        [HttpPost("start/{requestId}")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> StartTracking(int requestId, [FromBody] LocationDTO location)
        {
            try
            {
                _logger.LogInformation("Starting tracking for request {RequestId}", requestId);

                // Verificar que el request existe y pertenece al driver
                var request = await _requestService.GetRequestByIdAsync(requestId);
                if (request == null)
                {
                    return NotFound("Request not found");
                }

                // Verificar que el driver está autorizado para este request
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                if (request.DriverId != userId)
                {
                    return Forbid("Not authorized to track this request");
                }

                await _trackingService.StartTrackingAsync(
                    requestId,
                    location.Latitude,
                    location.Longitude
                );

                return Ok(new { message = "Tracking started" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting tracking for request {RequestId}", requestId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{requestId}/location")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> UpdateLocation(int requestId, [FromBody] LocationDTO location)
        {
            try
            {
                _logger.LogInformation("Updating location for request {RequestId}", requestId);

                // Verificar autorización
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var request = await _requestService.GetRequestByIdAsync(requestId);

                if (request == null || request.DriverId != userId)
                {
                    return Forbid();
                }

                await _trackingService.UpdateDriverLocationAsync(
                    requestId,
                    location.Latitude,
                    location.Longitude
                );

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating location for request {RequestId}", requestId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("stop/{requestId}")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> StopTracking(int requestId)
        {
            try
            {
                _logger.LogInformation("Stopping tracking for request {RequestId}", requestId);

                // Verificar autorización
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var request = await _requestService.GetRequestByIdAsync(requestId);

                if (request == null || request.DriverId != userId)
                {
                    return Forbid();
                }

                await _trackingService.StopTrackingAsync(requestId);
                return Ok(new { message = "Tracking stopped" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping tracking for request {RequestId}", requestId);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class LocationDTO
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
