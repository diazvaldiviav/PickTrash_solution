using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common.Exceptions;
using WebApplication1.Models.Dtos.UserDto;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthServices authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerDto)
        {
            try
            {

            _logger.LogInformation(
             "Intento de registro para usuario {Username} con rol {Role}",
             registerDto.UserName,
             registerDto.Role);


            var response = await _authService.RegisterAsync(registerDto);

            _logger.LogInformation(
             "Registro exitoso para usuario {Username} con ID {UserId} como usuario",
             response.UserName,
             response.UserId);


            return CreatedAtAction(
                    nameof(Register),
                    new { id = response.UserId },
                    response
                );
            }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(
                ex,
                "Error en el registro para usuario {Username}: {Message}",
                registerDto.UserName,
                ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error inesperado en el registro para usuario {Username}",
                registerDto.UserName);
            return StatusCode(500, "Error interno del servidor");
        }


    }


        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
        try
        {
            _logger.LogInformation(
                "Intento de login para usuario {Username} con rol {Role}",
                loginDto.UserName,
                loginDto.Role);

            var response = await _authService.LoginAsync(loginDto);

            _logger.LogInformation(
                "Login exitoso para usuario {Username} con ID {UserId}",
                response.UserName,
                response.UserId);

            return Ok(response);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(
                ex,
                "Error en el login para usuario {Username}: {Message}",
                loginDto.UserName,
                ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error inesperado en el login para usuario {Username}",
                loginDto.UserName);
            return StatusCode(500, "Error interno del servidor");
        }
    }

   
}
