using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Charges;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Charges entity mappings
    /// </summary>
    public class ChargesProfile : Profile
    {
        public ChargesProfile()
        {
            

            // Create DTO to Entity
            CreateMap<CreateChargesdto, Charges>()
                .ForMember(dest => dest.ChargeId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateChargesdto, Charges>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}