using AutoMapper;
using SaaS.Platform.API.Application.DTOs.SubscriptionsPlan;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for SubscriptionsPlan entity mappings
    /// </summary>
    public class SubscriptionsPlanProfile : Profile
    {
        public SubscriptionsPlanProfile()
        {
            
            // Create DTO to Entity
            CreateMap<CreateSubscriptionsPlandto, SubscriptionsPlan>()
                .ForMember(dest => dest.SubscriptionPlanId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateSubscriptionsPlandto, SubscriptionsPlan>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}