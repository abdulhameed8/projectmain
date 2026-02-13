using AutoMapper;
using SaaS.Platform.API.Application.DTOs.NotificationTemplete;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for NotificationTempletes entity mappings
    /// </summary>
    public class NotificationTempletesProfile : Profile
    {
        public NotificationTempletesProfile()
        {
            

            // Create DTO to Entity
            CreateMap<CreateNotificationTempletesdto, NotificationTempletes>()
                .ForMember(dest => dest.TempleteId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateNotificationTempletesdto, NotificationTempletes>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}