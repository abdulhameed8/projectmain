using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Payment;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Payment entity mappings
    /// </summary>
    public class PaymentsProfile : Profile
    {
        public PaymentsProfile()
        {


            // Create DTO to Entity
            CreateMap<CreatePaymentsdto, Payments>()
                .ForMember(dest => dest.PaymentId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            // Update DTO to Entity
            CreateMap<UpdatePaymentsdto, Payments>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}