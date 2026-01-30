using AutoMapper;
using SaaS.Platform.API.Application.DTOs.IVRMenus;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for IVRMenus entity mappings
    /// </summary>
    public class IVRMenusProfile : Profile
    {
        public IVRMenusProfile()
        {


            // Create DTO to Entity
            CreateMap<CreateIVRMenusdto, IVRMenus>()
                .ForMember(dest => dest.IVRMenuId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
                
            // Update DTO to Entity
            CreateMap<UpdateIVRMenusdto, IVRMenus>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}