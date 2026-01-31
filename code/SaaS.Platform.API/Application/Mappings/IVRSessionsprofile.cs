using AutoMapper;
using SaaS.Platform.API.Application.DTOs.IVRSessions;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for IVRPrompts entity mappings
    /// </summary>
    public class IVRSessionsProfile : Profile
    {
        public IVRSessionsProfile()
        {


            // Create DTO to Entity
            CreateMap<CreateIVRSessionsdto, IVRSessions>()
                .ForMember(dest => dest.SessionId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());
                

            // Update DTO to Entity
            CreateMap<UpdateIVRSessionsdto, IVRSessions>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}