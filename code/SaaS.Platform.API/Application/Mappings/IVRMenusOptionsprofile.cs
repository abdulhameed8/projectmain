using AutoMapper;
using SaaS.Platform.API.Application.DTOs.IVRMenusOptions;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for IVRMenusOptions entity mappings
    /// </summary>
    public class IVRMenusOPtionsProfile : Profile
    {
        public IVRMenusOPtionsProfile()
        {
            

            // Create DTO to Entity
            CreateMap<CreateIVRMenusOptionsdto, IVRMenusOptions>()
                .ForMember(dest => dest.MenuOptionId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateIVRMenusOptionsdto, IVRMenusOptions>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}