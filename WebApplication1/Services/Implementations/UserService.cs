using AutoMapper;
using WebApplication1.Data.Repositories.Interfaces;
using WebApplication1.Models.Dtos.UserDto;
using WebApplication1.Models.Enums;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Implementations
{
    public class UserService: IUserServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public UserService(
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

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<UserDTO>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw;
            }
        }


        //luego crear UserServices.cs

        public async Task<IEnumerable<ClientDTO>> GetAllClientsAsync()
        {
            var users = await _userRepository.GetUsersByRoleAsync(UserRole.Client);
            return _mapper.Map<IEnumerable<ClientDTO>>(users);
        }

        public async Task<IEnumerable<DriverDTO>> GetAllDriversAsync()
        {
            var users = await _userRepository.GetUsersByRoleAsync(UserRole.Driver);
            return _mapper.Map<IEnumerable<DriverDTO>>(users);
        }
    }
}
