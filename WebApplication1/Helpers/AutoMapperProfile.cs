using AutoMapper;
using WebApplication1.Models.Domain;
using WebApplication1.Models.Dtos.UserDto;

namespace WebApplication1.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterUserDTO, User>();
            CreateMap<User, AuthResponseDTO>();
            // Agrega aquí más mapeos según sea necesario
        }
    }
}
