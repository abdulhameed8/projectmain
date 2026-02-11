using AutoMapper;
using SaaS.Platform.API.Application.DTOs;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for PaymentMethod entity mappings
    /// </summary>
    public class PaymentMethodsProfile : Profile
    {
        public PaymentMethodsProfile()
        {
           

            // Create DTO to Entity
            CreateMap<CreatePaymentMethodsdto, PaymentMethods>()
                .ForMember(dest => dest.PaymentMethodId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdatePaymentMethodsdto, PaymentMethods>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}