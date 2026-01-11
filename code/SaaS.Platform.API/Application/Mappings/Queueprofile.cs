using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Queues;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Queue entity mappings
    /// </summary>
    public class QueueProfile : Profile
    {
        public QueueProfile()
        {
            
            // Create DTO to Entity
            CreateMap<CreateQueuedto, Queues>()
                .ForMember(dest => dest.QueueId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateQueuedto, Queues>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
