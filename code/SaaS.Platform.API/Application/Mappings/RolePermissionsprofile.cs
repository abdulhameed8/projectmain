using AutoMapper;
using SaaS.Platform.API.Application.DTOs.RolePermissions;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for RolePermissions entity mappings
    /// </summary>
    public class RolePermissionsProfile : Profile
    {
        public RolePermissionsProfile()
        {

            // Create DTO to Entity
            CreateMap<CreateRolePermissionsdto, RolePermissions>()

                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
                

            // Update DTO to Entity
            CreateMap<UpdateRolePermissionsdto, RolePermissions>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}