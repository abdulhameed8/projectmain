using AutoMapper;
using SaaS.Platform.API.Application.DTOs;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Beneficiary entity mappings
    /// </summary>
    public class Beneficiariesprofile : Profile
    {
        public Beneficiariesprofile()
        {
            
            // Create DTO to Entity
            CreateMap<CreateBeneficiariesdto, Beneficiaries>()
                .ForMember(dest => dest.BeneficiaryId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateBeneficiariesdto, Beneficiaries>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}