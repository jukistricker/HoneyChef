using AutoMapper;
using IOITCore.Entities;
using IOITCore.Models.ViewModels;

namespace IOITCore.Models.Mappings
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<User, GetUserByUserName>()
                .ForMember(dest => dest.UserCreatedId, otp => otp.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.UserUpdatedId, otp => otp.MapFrom(src => src.UpdatedById));
        }
    }
}
