using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class WorkflowInstances : BaseEntity
    {
        public Guid WorkflowInstanceId { get; set; }
        public Guid TenantId { get; set; }
        public Guid WorkflowDefinitionId { get;  set; }
        public Guid CamundaProcessInstanceId { get; set; }
        public string? EntityType { get; set; }
        public Guid EntityId { get; set; }
        public string? Status { get; set; }
        public DateOnly StartDate { get; set; } 
        public  DateOnly EndDate { get; set; }
        public Guid StartedBy { get; set; }
        public string? Variables { get; set; }
        public new DateTime CreatedDate { get; set; }
    }
}

