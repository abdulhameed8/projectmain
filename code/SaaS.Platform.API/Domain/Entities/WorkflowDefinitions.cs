using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class WorkflowDefinitions : BaseEntity
    {
        public Guid WorkflowDefinitionId { get; set; }
        public Guid TenantId { get; set; }
        public string? WorkflowName { get; internal set; }
        public string? WorkflowKey { get; set; }
        public int Version { get; set; }
        public string? BpmnXML { get; set; }
        public string? Category { get; set; }
        public bool IsActive { get; set; } = true;
        public new DateTime? CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        public new DateTime? ModifiedDate { get; set; }
        public new Guid? ModifiedBy { get; set; }
    }
}
