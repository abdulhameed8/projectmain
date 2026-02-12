namespace SaaS.Platform.API.Application.DTOs.WorkflowTask
{
    public class CreateWorkflowTaskdto
    {
        public Guid WorkflowTaskId { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public Guid CamundaTaskId { get; set; }
        public string? TaskName { get; set; }
        public string? TaskKey { get; set; }
        public Guid AssignedUserId { get; set; }
        public Guid AssignedRoleId { get; set; }
        public string? Status { get; set; }
        public int Priority { get; set; }
        public DateOnly DueDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public Guid CompletedBy { get; set; }
        public string? FormData { get; set; }
        public string? Comments { get; set; }
        public  DateTime? CreatedDate { get; set; }
    }
    public class UpdateWorkflowTaskdto
    {
        public Guid WorkflowTaskId { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public Guid CamundaTaskId { get; set; }
        public string? TaskName { get; set; }
        public string? TaskKey { get; set; }
        public Guid AssignedUserId { get; set; }
        public Guid AssignedRoleId { get; set; }
        public string? Status { get; set; }
        public int Priority { get; set; }
        public DateOnly DueDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public Guid CompletedBy { get; set; }
        public string? FormData { get; set; }
        public string? Comments { get; set; }
        public  DateTime? CreatedDate { get; set; }
    }
    public class WorkflowTaskdto
    {
        public Guid WorkflowTaskId { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public Guid CamundaTaskId { get; set; }
        public string? TaskName { get; set; }
        public string? TaskKey { get; set; }
        public Guid AssignedUserId { get; set; }
        public Guid AssignedRoleId { get; set; }
        public string? Status { get; set; }
        public int Priority { get; set; }
        public DateOnly DueDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public Guid CompletedBy { get; set; }
        public string? FormData { get; set; }
        public string? Comments { get; set; }
        public  DateTime? CreatedDate { get; set; }
    }
}
