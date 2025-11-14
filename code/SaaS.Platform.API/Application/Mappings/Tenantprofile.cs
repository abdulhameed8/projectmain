using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Tenant;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Tenant entity mappings
    /// </summary>
    public class TenantProfile : Profile
    {
        public TenantProfile()
        {
            // Entity to DTO
            CreateMap<Tenant, TenantDto>()
                .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.TenantName));

            // Create DTO to Entity
            CreateMap<CreateTenantdto, Tenant>()
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateTenantDto, Tenant>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}