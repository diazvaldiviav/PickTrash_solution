using WebApplication1.Data.Repositories.Interfaces;
using WebApplication1.Services.Interfaces;
using AutoMapper;
using WebApplication1.Common.Exceptions;
using WebApplication1.Models.Domain;
using WebApplication1.Models.Dtos.UserDto;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Models.Enums;
using WebApplication1.Data.Repositories.Implementations;


namespace WebApplication1.Services.Implementations
{
    public class AuthServices : IAuthServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public AuthServices(
            IUserRepository userRepository,
            IMapper mapper,
            IConfiguration configuration,
            IClientRepository clientRepository,
            IDriverRepository driverRepository,
            ILogger<AuthServices> logger
            )
        {
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _driverRepository = driverRepository;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }


        //logica para registrar el usuario
        public async Task<AuthResponseDTO> RegisterAsync(RegisterUserDTO dto)
        {
            // Validar que el rol sea válido para la app (solo Cliente o Driver)
            if (dto.Role != UserRole.Client && dto.Role != UserRole.Driver)
            {
                throw new BadRequestException("Rol no válido para registro en la app");
            }

            // Validar datos únicos
            if (!await _userRepository.IsUsernameUniqueAsync(dto.UserName))
                throw new BadRequestException("El nombre de usuario ya está registrado");

            if (!await _userRepository.IsPhoneNumberUniqueAsync(dto.PhoneNumber))
                throw new BadRequestException("El número de teléfono ya está registrado");

            // Crear usuario
            var user = new User
            {
                Email = dto.Email,
                UserName = dto.UserName,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Role,
                PasswordHash = HashPassword(dto.Password)
            };

            // Registrar usuario
            var registeredUser = await _userRepository.RegisterUserAsync(user);

            switch (registeredUser.Role)
            {
                case UserRole.Client:
                    var client = new Client
                    {
                        UserId = registeredUser.Id,
                        DefaultAddress = string.Empty // o null si permites nulos
                    };
                    await _clientRepository.RegisterClientAsync(client);
                    _logger.LogInformation(
                    "Registro exitoso para usuario {Username} con ID {UserId} como cliente",
                   registeredUser.UserName,
                   registeredUser.Id
                   );

                    break;

                case UserRole.Driver:
                    var driver = new Driver
                    {
                        UserId = registeredUser.Id,
                        IsAvailable = true, // valor por defecto
                        Rating = 0 // valor inicial
                                   // otros campos específicos del conductor
                    };


                    await _driverRepository.RegisterDriverAsync(driver);
                    _logger.LogInformation(
                    "Registro exitoso para usuario {Username} con ID {UserId} como cliente",
                    registeredUser.UserName,
                    registeredUser.Id
                    );
                    break;

                case UserRole.Admin:
                    // Los admins no necesitan perfil adicional por ahora
                    break;

                default:
                    throw new BadRequestException("Rol no válido");
            }

            // Generar token
            var token = GenerateJwtToken(registeredUser);

            return new AuthResponseDTO
            {
                UserId = registeredUser.Id,
                Email = registeredUser.Email,
                UserName = registeredUser.UserName,
                Role = registeredUser.Role,
                Token = token
            };
        }


        //logica para autenticar el usuario
        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            // Buscar usuario por username
            var user = await _userRepository.GetByUsernameAsync(dto.UserName);
            if (user == null)
                throw new BadRequestException("Credenciales inválidas");

            // Verificar que el rol coincida con el de la app
            if (user.Role != dto.Role)
            {
                string appType = dto.Role == UserRole.Client ? "Cliente" : "Driver";
                throw new BadRequestException($"Este usuario no está registrado como {appType}");
            }

            // Verificar contraseña
            if (!VerifyPassword(dto.Password, user.PasswordHash))
                throw new BadRequestException("Credenciales inválidas");

            // Generar token
            var token = GenerateJwtToken(user);

            // Crear respuesta
            return new AuthResponseDTO
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Role = user.Role,
                Token = token
            };
        }


        public string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

    }
}
