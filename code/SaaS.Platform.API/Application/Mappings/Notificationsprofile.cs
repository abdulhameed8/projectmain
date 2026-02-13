using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Notifications;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Notification entity mappings
    /// </summary>
    public class NotificationsProfile : Profile
    {
        public NotificationsProfile()
        {


            // Create DTO to Entity
            CreateMap<CreateNotificationsdto, Notifications>()
                .ForMember(dest => dest.TempleteId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());
                

            // Update DTO to Entity
            CreateMap<UpdateNotificationsdto, Notifications>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}