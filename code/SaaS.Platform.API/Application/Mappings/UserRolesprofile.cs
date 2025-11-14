using AutoMapper;
using SaaS.Platform.API.Application.DTOs.UserRoles;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Tenant entity mappings
    /// </summary>
    public class UserRolesProfile : Profile
    {
        public UserRolesProfile()
        {


            // Create DTO to Entity
            CreateMap<CreateUserRolesdto, UserRoles>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
                
            // Update DTO to Entity
            CreateMap<UpdateUserRolesdto, UserRoles>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}