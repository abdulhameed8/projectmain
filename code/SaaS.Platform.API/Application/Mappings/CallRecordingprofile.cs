using AutoMapper;
using SaaS.Platform.API.Application.DTOs.CallRecordings;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for CallRecording entity mappings
    /// </summary>
    public class CallRecordingProfile : Profile
    {
        public CallRecordingProfile()
        {

            // Create DTO to Entity
            CreateMap<CreateCallRecordingdto, CallRecordings>()
                .ForMember(dest => dest.RecordingId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());

            // Update DTO to Entity
            CreateMap<UpdateCallRecordingdto, CallRecordings>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}