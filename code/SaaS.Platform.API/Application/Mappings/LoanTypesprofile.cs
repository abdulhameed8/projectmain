using AutoMapper;
using SaaS.Platform.API.Application.DTOs.LoanTypes;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for LOanTyps entity mappings
    /// </summary>
    public class LoanTypesProfile : Profile
    {
        public LoanTypesProfile()
        {


            // Create DTO to Entity
            CreateMap<CreateLoanTypesdto, LoanTypes>()
                .ForMember(dest => dest.LoanTypeId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateLoanTypesdto, Charges>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}