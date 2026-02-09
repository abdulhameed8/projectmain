using AutoMapper;
using SaaS.Platform.API.Application.DTOs.LoanRepayments;
using SaaS.Platform.API.Application.DTOs.Loans;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for LoanRepayment entity mappings
    /// </summary>
    public class LoanRepaymentsProfile : Profile
    {
        public LoanRepaymentsProfile()
        {

            // Create DTO to Entity
            CreateMap<CreateLoanRepaymentsdto, LoanRepayments>()
                .ForMember(dest => dest.RepaymentId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());
                

            // Update DTO to Entity
            CreateMap<UpdateLoanRepaymentsdto, LoanRepayments>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}