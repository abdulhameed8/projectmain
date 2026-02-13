using AutoMapper;
using SaaS.Platform.API.Application.DTOs.EmailConfig;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for EmailConfig entity mappings
    /// </summary>
    public class EmailConfigurationsProfile : Profile
    {
        public EmailConfigurationsProfile()
        {
            

            // Create DTO to Entity
            CreateMap<CreateEmailConfigurationsdto, EmailConfigurations>()
                .ForMember(dest => dest.EmailConfigId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateEmailConfigurationsdto, EmailConfigurations>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}