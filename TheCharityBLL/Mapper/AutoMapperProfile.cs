using TheCharityBLL.DTOs.UserDTOs;
using TheCharityDAL.Entities;
using AutoMapper;

namespace TheCharityBLL.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<User, UserDTO>()
                .ReverseMap();

            CreateMap<CreateUserDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

            CreateMap<UpdateUserDTO, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.LastStorageUpdate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.StorageOwned, opt => opt.MapFrom(src => src.StorageOwned));

            CreateMap<User, UserResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.RegistrationDate))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.StorageOwned, opt => opt.MapFrom(src => src.StorageOwned))
                .ForMember(dest => dest.LastStorageUpdate, opt => opt.MapFrom(src => src.LastStorageUpdate));

            CreateMap<UserResponseDTO, User>()
          .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
          .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
          .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
          .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
          .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.RegistrationDate))
          .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
          .ForMember(dest => dest.StorageOwned, opt => opt.MapFrom(src => src.StorageOwned))
          .ForMember(dest => dest.LastStorageUpdate, opt => opt.MapFrom(src => src.LastStorageUpdate));
        }
    }
}
