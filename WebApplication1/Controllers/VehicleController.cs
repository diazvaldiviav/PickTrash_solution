using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Common.Exceptions;
using WebApplication1.Models.Domain;
using WebApplication1.Models.Dtos.Vehicle;
using WebApplication1.Services.Interfaces;
using AutoMapper;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Driver")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleServices _vehicleService;
        private readonly ILogger<VehicleController> _logger;
        private readonly IMapper _mapper;

        public VehicleController(
            IVehicleServices vehicleService,
            ILogger<VehicleController> logger,
            IMapper mapper
            )
        {
            _vehicleService = vehicleService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(VehicleResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterVehicle([FromBody] CreateVehicleDTO createVehicleDto)
        {
            try
            {
                var driverId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var response = await _vehicleService.RegisterVehicleAsync(createVehicleDto, driverId);

                return CreatedAtAction(
                    nameof(RegisterVehicle),
                    new { id = response.Id },
                    response);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al registrar vehículo");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
