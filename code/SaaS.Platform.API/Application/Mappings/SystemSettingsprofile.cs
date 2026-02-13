using AutoMapper;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Application.DTOs.SystemSettings;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for SettingSystem entity mappings
    /// </summary>
    public class SystemSettingsProfile : Profile
    {
        public SystemSettingsProfile()
        {

            // Create DTO to Entity
            CreateMap<CreateSystemSettingsdto, SystemSettings>()
                .ForMember(dest => dest.SettingId, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());

            // Update DTO to Entity
            CreateMap<UpdateSystemSettingsdto, SystemSettings>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}