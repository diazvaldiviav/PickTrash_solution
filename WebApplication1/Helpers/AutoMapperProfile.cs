using AutoMapper;
using WebApplication1.Models.Domain;
using WebApplication1.Models.Dtos.Request;
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


            //Mapper para los Request

            CreateMap<Request, RequestDTO>()
            .ForMember(dest => dest.ClientName,
               opt => opt.MapFrom(src => src.ClientName))  // Usar el campo que ya tiene el nombre
              .ForMember(dest => dest.DriverName,
               opt => opt.MapFrom(src => src.DriverName)); // Usar el campo que ya tiene el nombre


            CreateMap<CreateRequestDTO, Request>();

            CreateMap<UpdateRequestDTO, Request>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Request, RequestListItemDTO>()
                .ForMember(dest => dest.ClientName,
                    opt => opt.MapFrom(src => $"{src.Client.User.Name} {src.Client.User.LastName}"))
                .ForMember(dest => dest.DriverName,
                    opt => opt.MapFrom(src => src.Driver != null
                        ? $"{src.Driver.User.Name} {src.Driver.User.LastName}"
                        : null));



            //driver mapper
            CreateMap<Driver, AvailableDriverDTO>()
           .ForMember(dest => dest.FullName,
               opt => opt.MapFrom(src => $"{src.User.Name} {src.User.LastName}"))
           .ForMember(dest => dest.VehicleTypes,
               opt => opt.MapFrom(src => src.DriverVehicles
                   .Select(dv => dv.Vehicle.TransportCategory.Name)))
           .ForMember(dest => dest.PhoneNumber,
               opt => opt.MapFrom(src => src.User.PhoneNumber));
        }


    }
}
