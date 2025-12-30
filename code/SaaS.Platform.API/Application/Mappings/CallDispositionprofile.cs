using AutoMapper;
using SaaS.Platform.API.Application.DTOs.CallDispositions;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for CallDispositions entity mappings
    /// </summary>
    public class CallDispositionProfile : Profile
    {
        public CallDispositionProfile()
        {
            
            // Create DTO to Entity
            CreateMap<CreateCallDispositiondto, CallDispositions>()
                .ForMember(dest => dest.CallDispositionId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())                
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateCallDispositiondto, CallDispositions>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}