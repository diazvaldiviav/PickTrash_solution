using AutoMapper;
using WebApplication1.Models.Domain;
using WebApplication1.Models.Dtos.TransportCategory;
using WebApplication1.Models.Dtos.UserDto;
using WebApplication1.Models.Dtos.Vehicle;

namespace WebApplication1.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {

            //mapper para autenticacion
            CreateMap<RegisterUserDTO, User>();
            CreateMap<User, AuthResponseDTO>();

            //mapper para usuarios
            CreateMap<User, ClientDTO>()
          .ForMember(dest => dest.DefaultAddress,
              opt => opt.MapFrom(src => src.Client != null ? src.Client.DefaultAddress : string.Empty));


            //mapper para usuarios driver
            CreateMap<User, DriverDTO>()
                .ForMember(dest => dest.IsAvailable,
                    opt => opt.MapFrom(src => src.Driver != null ? src.Driver.IsAvailable : false))
                .ForMember(dest => dest.Rating,
                    opt => opt.MapFrom(src => src.Driver != null ? src.Driver.Rating : 0));


            //mapper para usuarios
            CreateMap<User, UserDTO>()
           .ForMember(dest => dest.DefaultAddress,
               opt => opt.MapFrom(src => src.Client != null ? src.Client.DefaultAddress : null))
           .ForMember(dest => dest.IsAvailable,
               opt => opt.MapFrom(src => src.Driver != null ? src.Driver.IsAvailable : (bool?)null))
           .ForMember(dest => dest.Rating,
               opt => opt.MapFrom(src => src.Driver != null ? src.Driver.Rating : (decimal?)null));



            //mapper para vehiculos
            CreateMap<CreateVehicleDTO, Vehicle>()
          .ForMember(dest => dest.TransportCategory, opt => opt.Ignore());

            CreateMap<Vehicle, VehicleResponseDTO>()
                .ForMember(dest => dest.TransportCategoryName,
                    opt => opt.MapFrom(src => src.TransportCategory.Name));


            //mapper para categorias de transporte
            CreateMap<TransportCategory, TransportCategoryDTO>();
        }


    }
}
