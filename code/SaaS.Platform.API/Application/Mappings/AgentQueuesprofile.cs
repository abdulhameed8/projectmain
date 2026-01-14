using AutoMapper;
using SaaS.Platform.API.Application.DTOs.AgentsQueues;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for AgentQueues entity mappings
    /// </summary>
    public class AgentQueuesProfile : Profile
    {
        public AgentQueuesProfile()
        {
            

            // Create DTO to Entity
            CreateMap<CreateAgentsQueuesdto, AgentQueues>()
                .ForMember(dest => dest.AgentQueuesId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateAgentsQueuesdto, AgentQueues>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}