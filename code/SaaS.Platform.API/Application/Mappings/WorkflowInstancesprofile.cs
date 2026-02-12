using AutoMapper;
using SaaS.Platform.API.Application.DTOs.WorkflowInstances;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for WorkflowInstances entity mappings
    /// </summary>
    public class WorkflowInstancessProfile : Profile
    {
        public WorkflowInstancessProfile()
        {


            // Create DTO to Entity
            CreateMap<CreateWorkflowInstancesdto, WorkflowInstances>()
                .ForMember(dest => dest.WorkflowInstanceId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());
                

            // Update DTO to Entity
            CreateMap<UpdateWorkflowInstancesdto, WorkflowInstances>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}