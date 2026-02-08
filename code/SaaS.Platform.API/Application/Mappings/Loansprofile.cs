using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Loans;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Loan entity mappings
    /// </summary>
    public class LoansProfile : Profile
    {
        public LoansProfile()
        {

            // Create DTO to Entity
            CreateMap<CreateLoansdto, Loans>()
                .ForMember(dest => dest.LoanId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());

            // Update DTO to Entity
            CreateMap<UpdateLoansdto, Loans>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}