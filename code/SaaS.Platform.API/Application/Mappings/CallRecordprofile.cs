using AutoMapper;
using SaaS.Platform.API.Application.DTOs.CallRecord;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for CallRecord entity mappings
    /// </summary>
    public class CallRecordProfile : Profile
    {
        public CallRecordProfile()
        {

            // Create DTO to Entity
            CreateMap<CreateCallRecorddto, CallRecord>()
                .ForMember(dest => dest.CallRecordId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());
                
            // Update DTO to Entity
            CreateMap<UpdateCallRecorddto, CallRecord>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}