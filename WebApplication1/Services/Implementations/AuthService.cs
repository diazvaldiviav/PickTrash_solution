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
    public class AuthService : IAuthServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public AuthService(
            IUserRepository userRepository,
            IMapper mapper,
            IConfiguration configuration,
            IClientRepository clientRepository,
            IDriverRepository driverRepository,
            ILogger<AuthService> logger
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
            try
            {
                _logger.LogInformation("Iniciando registro para usuario {Username} con rol {Role}",
               dto.UserName, dto.Role);

                // Validaciones según el rol
                if (!await _userRepository.IsUsernameUniqueForRoleAsync(dto.UserName, dto.Role))
                {
                    string roleStr = dto.Role == UserRole.Client ? "cliente" : "conductor";
                    throw new BadRequestException($"El nombre de usuario ya está registrado como {roleStr}");
                }

                if (!await _userRepository.IsPhoneNumberUniqueForRoleAsync(dto.PhoneNumber, dto.Role))
                {
                    string roleStr = dto.Role == UserRole.Client ? "cliente" : "conductor";
                    throw new BadRequestException($"El número de teléfono ya está registrado como {roleStr}");
                }

                if (!await _userRepository.IsEmailUniqueForRoleAsync(dto.Email, dto.Role))
                {
                    string roleStr = dto.Role == UserRole.Client ? "cliente" : "conductor";
                    throw new BadRequestException($"El email ya está registrado como {roleStr}");
                }

                // Verificar si el usuario ya existe con otro rol
                var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    _logger.LogInformation("Usuario existente encontrado con otro rol. Verificando compatibilidad...");

                    // Verificar que no intente registrarse con el mismo rol
                    if (existingUser.Role == dto.Role)
                    {
                        string roleStr = dto.Role == UserRole.Client ? "cliente" : "conductor";
                        throw new BadRequestException($"Ya existe una cuenta registrada como {roleStr} con este email");
                    }

                    // Verificar que solo pueda registrarse como cliente o conductor
                    if (dto.Role != UserRole.Client && dto.Role != UserRole.Driver)
                    {
                        throw new BadRequestException("Solo se permite registro como cliente o conductor");
                    }
                }


                // Crear nuevo usuario siempre
                var user = _mapper.Map<User>(dto);
                user.PasswordHash = HashPassword(dto.Password);

                // Registrar el nuevo usuario
                var registeredUser = await _userRepository.RegisterUserAsync(user);

                if (registeredUser == null)
                    throw new Exception("Error al registrar el usuario en la base de datos");

                // Crear perfil según rol
                switch (dto.Role)
                {
                    case UserRole.Client:
                        var client = new Client
                        {
                            UserId = registeredUser.Id,
                            DefaultAddress = string.Empty
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
                            IsAvailable = true,
                            Rating = 0
                        };
                        await _driverRepository.RegisterDriverAsync(driver);
                        _logger.LogInformation(
                            "Registro exitoso para usuario {Username} con ID {UserId} como conductor",
                            registeredUser.UserName,
                            registeredUser.Id
                        );
                        break;
                }
                // Generar token
                var token = GenerateJwtToken(registeredUser);

                return new AuthResponseDTO
                {
                    UserId = registeredUser.Id,
                    Email = registeredUser.Email,
                    UserName = registeredUser.UserName,
                    Role = dto.Role,
                    Token = token
                };

            } catch (Exception ex)


        {
        _logger.LogError(ex, "Error en el registro para usuario {Username} con rol {Role}", 
            dto?.UserName ?? "unknown", dto?.Role);
        throw;
        }




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
