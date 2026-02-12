using AutoMapper;
using SaaS.Platform.API.Application.DTOs.WorkflowTask;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for WorkflowTask entity mappings
    /// </summary>
    public class WorkflowTaskProfile : Profile
    {
        public WorkflowTaskProfile()
        {


            // Create DTO to Entity
            CreateMap<CreateWorkflowTaskdto, WorkflowTasks>()
                .ForMember(dest => dest.WorkflowTaskId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());


            // Update DTO to Entity
            CreateMap<UpdateWorkflowTaskdto, WorkflowTasks>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}