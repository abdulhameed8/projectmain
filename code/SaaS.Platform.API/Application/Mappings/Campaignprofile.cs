using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Campaigns;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Campaigns entity mappings
    /// </summary>
    public class CampaignProfile : Profile
    {
        public CampaignProfile()
        {


            // Create DTO to Entity
            CreateMap<CreateCampaigndto, Campaigns>()
                .ForMember(dest => dest.CampaignId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());
                

            // Update DTO to Entity
            CreateMap<UpdateCampaigndto, Campaigns>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}