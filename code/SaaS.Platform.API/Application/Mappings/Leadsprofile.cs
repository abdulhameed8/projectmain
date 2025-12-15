using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Leads;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Leads entity mappings
    /// </summary>
    public class LeadsProfile : Profile
    {
        public LeadsProfile()
        {

            // Create DTO to Entity
            CreateMap<CreateLeadsdto, Leads>()
                .ForMember(dest => dest.LeadId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());
               

            // Update DTO to Entity
            CreateMap<UpdateLeadsdto, Leads>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}