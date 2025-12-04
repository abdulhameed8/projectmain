using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Permissions;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Customer entity mappings
    /// </summary>
    public class PermissionsProfile : Profile
    {
        public PermissionsProfile()
        {
            
            // Create DTO to Entity
            CreateMap<CreatePermissionsdto, Permissions>()
                .ForMember(dest => dest.PermissionId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdatePermissionsdto, Permissions>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}