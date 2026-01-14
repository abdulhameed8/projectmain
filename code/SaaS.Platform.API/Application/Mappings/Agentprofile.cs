using AutoMapper;
using SaaS.Platform.API.Application.DTOs.Agents;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Agent entity mappings
    /// </summary>
    public class AgentProfile : Profile
    {
        public AgentProfile()
        {
            
            // Create DTO to Entity
            CreateMap<CreateAgentQueuesdto, Agents>()
                .ForMember(dest => dest.AgentId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateAgentQueuesdto, Agents>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}