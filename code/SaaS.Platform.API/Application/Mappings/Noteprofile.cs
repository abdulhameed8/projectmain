using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Notes;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Customer entity mappings
    /// </summary>
    public class NoteProfile : Profile
    {
        public NoteProfile()
        {
           

            // Create DTO to Entity
            CreateMap<CreateNotedto, Notes>()
                .ForMember(dest => dest.NoteId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsPrivate, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateNotedto, Notes>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}