using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Activities;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Activity entity mappings
    /// </summary>
    public class ActivityProfile : Profile
    {
        public ActivityProfile()
        {

            // Create DTO to Entity
            CreateMap<CreateActivitydto, Activities>()
                .ForMember(dest => dest.ActivityId, opt => opt.Ignore());


            // Update DTO to Entity
            CreateMap<UpdateActivitydto, Activities>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}