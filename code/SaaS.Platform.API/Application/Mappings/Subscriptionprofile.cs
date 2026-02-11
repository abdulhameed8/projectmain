using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Subscriptions;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Subscriptions entity mappings
    /// </summary>
    public class SubscriptionsProfile : Profile
    {
        public SubscriptionsProfile()
        {

            // Create DTO to Entity
            CreateMap<CreateSubscriptionsdto, Subscriptions>()
                .ForMember(dest => dest.SubscriptionId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());

            // Update DTO to Entity
            CreateMap<UpdateSubscriptionsdto, Subscriptions>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}