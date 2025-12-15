using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Opportunities;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Opportunities entity mappings
    /// </summary>
    public class OpportunityProfile : Profile
    {
        public OpportunityProfile()
        {


            // Create DTO to Entity
            CreateMap<CreateOpportunitydto, Opportunities>()
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());

            // Update DTO to Entity
            CreateMap<UpdateOpportunitydto, Opportunities>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}