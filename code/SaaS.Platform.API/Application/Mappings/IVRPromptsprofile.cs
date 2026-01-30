using AutoMapper;
using SaaS.Platform.API.Application.DTOs.IVRPrompts;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for IVRPrompts entity mappings
    /// </summary>
    public class IVRPromptsProfile : Profile
    {
        public IVRPromptsProfile()
        {


            // Create DTO to Entity
            CreateMap<CreateIVRPromptsdto, IVRPrompts>()
                .ForMember(dest => dest.PromptId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateIVRPromptsdto, IVRPrompts>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}