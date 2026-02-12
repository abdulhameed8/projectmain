namespace SaaS.Platform.API.Application.DTOs.WorkflowDefinitions
{
    public class CreateWorkflowDefinitionsdto
    {
        public Guid WorkflowDefinitionId { get; set; }
        public Guid TenantId { get; set; }
        public string? WorkflowName { get; internal set; }
        public string? WorkflowKey { get; set; }
        public int Version { get; set; }
        public string? BpmnXML { get; set; }
        public string? Category { get; set; }
        public  DateTime? CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        
    }
    public class UpdateWorkflowDefinitionsdto
    {
        public Guid WorkflowDefinitionId { get; set; }
        public Guid TenantId { get; set; }
        public string? WorkflowName { get; internal set; }
        public string? WorkflowKey { get; set; }
        public int Version { get; set; }
        public string? BpmnXML { get; set; }
        public string? Category { get; set; }
        public bool? IsActive { get; set; } 
        public  DateTime? CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        
    }
    public class WorkflowDefinitionsdto
    {
        public Guid WorkflowDefinitionId { get; set; }
        public Guid TenantId { get; set; }
        public string? WorkflowName { get; internal set; }
        public string? WorkflowKey { get; set; }
        public int Version { get; set; }
        public string? BpmnXML { get; set; }
        public string? Category { get; set; }
        public bool IsActive { get; set; } 
        public  DateTime? CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        public  DateTime? ModifiedDate { get; set; }
        public  Guid? ModifiedBy { get; set; }
    }
}
