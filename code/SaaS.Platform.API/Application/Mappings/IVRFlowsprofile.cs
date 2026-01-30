using AutoMapper;
using SaaS.Platform.API.Application.DTOs.IVRFlows;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for IVRFlows entity mappings
    /// </summary>
    public class IVRFlowsProfile : Profile
    {
        public IVRFlowsProfile()
        {
            

            // Create DTO to Entity
            CreateMap<CreateIVRFlowsdto, IVRFlows>()
                .ForMember(dest => dest.IVRFLowId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateIVRFlowsdto, IVRFlows>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}