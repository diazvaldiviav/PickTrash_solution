using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Dtos.UserDto;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userService;
        private readonly ILogger<AuthController> _logger;

        public UserController(IUserServices userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }




        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("Obteniendo lista de todos los usuarios");
                var users = await _userService.GetAllUsersAsync();

                if (!users.Any())
                    return NotFound("No se encontraron usuarios registrados");

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de usuarios");
                return StatusCode(500, "Error interno del servidor");
            }
        }





        [HttpGet("clients")]
        [ProducesResponseType(typeof(IEnumerable<ClientDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllClients()
        {
            try
            {
                _logger.LogInformation("Obteniendo lista de todos los clientes");
                var clients = await _userService.GetAllClientsAsync();

                if (!clients.Any())
                    return NotFound("No se encontraron clientes registrados");

                return Ok(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de clientes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("drivers")]
        [ProducesResponseType(typeof(IEnumerable<DriverDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllDrivers()
        {
            try
            {
                _logger.LogInformation("Obteniendo lista de todos los conductores");
                var drivers = await _userService.GetAllDriversAsync();

                if (!drivers.Any())
                    return NotFound("No se encontraron conductores registrados");

                return Ok(drivers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de conductores");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
