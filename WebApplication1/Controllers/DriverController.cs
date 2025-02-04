using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Common.Exceptions;
using WebApplication1.Models.Domain;
using WebApplication1.Models.Dtos.UserDto;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly ILogger<DriverController> _logger;

        public DriverController(
            IDriverService driverService,
            ILogger<DriverController> logger)
        {
            _driverService = driverService;
            _logger = logger;
        }

        [HttpGet("active")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<IEnumerable<AvailableDriverDTO>>> GetActiveDrivers()
        {
            try
            {
                var drivers = await _driverService.GetActiveDriversAsync();
                return Ok(drivers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active drivers");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("by-location")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<IEnumerable<AvailableDriverDTO>>> GetDriversByLocation(
            [FromQuery] decimal latitude,
            [FromQuery] decimal longitude,
            [FromQuery] int radius = 20)
        {
            try
            {
                var drivers = await _driverService.GetDriversByLocationAsync(
                    latitude, longitude, radius);
                return Ok(drivers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting drivers by location");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut("location")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationDTO location)
        {
            try
            {
                var driverId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                await _driverService.UpdateLocationAsync(driverId, location);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating driver location");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("location/{driverId}")]
        [Authorize]
        public async Task<ActionResult<UpdateLocationDTO>> GetDriverLocation(int driverId)
        {
            try
            {
                var location = await _driverService.GetLocationAsync(driverId);
                if (location == null)
                    return NotFound($"No location found for driver {driverId}");

                return Ok(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting driver location");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("my-location")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult<UpdateLocationDTO>> GetMyLocation()
        {
            try
            {
                var driverId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var location = await _driverService.GetLocationAsync(driverId);
                if (location == null)
                    return NotFound("No location found");

                return Ok(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting driver location");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("location/address")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> UpdateLocationByAddress([FromBody] UpdateLocationByAddressDTO request)
        {
            var driverId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            try
            {
                
                await _driverService.UpdateLocationByAddressAsync(driverId, request.Address);

                return Ok(new
                {
                    message = "Ubicación actualizada correctamente",
                    address = request.Address
                });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating driver location by address for driver {DriverId}", driverId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
