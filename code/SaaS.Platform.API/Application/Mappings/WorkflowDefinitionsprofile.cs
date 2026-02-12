using AutoMapper;
using SaaS.Platform.API.Application.DTOs.WorkflowDefinitions;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for WorkflowDefinitions entity mappings
    /// </summary>
    public class WorkflowDefinitionsProfile : Profile
    {
        public WorkflowDefinitionsProfile()
        {
            

            // Create DTO to Entity
            CreateMap<CreateWorkflowDefinitionsdto, WorkflowDefinitions>()
                .ForMember(dest => dest.WorkflowDefinitionId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Update DTO to Entity
            CreateMap<UpdateWorkflowDefinitionsdto, WorkflowDefinitions>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
