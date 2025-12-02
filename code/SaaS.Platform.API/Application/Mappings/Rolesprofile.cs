using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Roles;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Domain.Entities.Roles.cs;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Roles entity mappings
    /// </summary>
    public class RolesProfile : Profile
    {
        public RolesProfile()
        {
            
            // Create DTO to Entity
            CreateMap<CreateRolesdto, Roles>()
                .ForMember(dest => dest.RoleId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateRolesdto, Roles>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}