using AutoMapper;
using SaaS.Platform.API.Application.DTOs.WorkflowHistory;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for WorkflowHistory entity mappings
    /// </summary>
    public class WorkflowHistoryProfile : Profile
    {
        public WorkflowHistoryProfile()
        {


            // Create DTO to Entity
            CreateMap<CreateWorkflowHistorydto, WorkflowHistory>()
                .ForMember(dest => dest.HistoryId, opt => opt.Ignore());
                

            // Update DTO to Entity
            CreateMap<UpdateWorkflowHistorydto, WorkflowHistory>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
