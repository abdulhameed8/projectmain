namespace SaaS.Platform.API.Application.DTOs.WorkflowHistory
{
    public class CreateWorkflowHistorydto
    {
        public Guid HistoryId { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public Guid WorkflowTaskId { get; set; }
        public string? Action { get; set; }
        public Guid ActorUserId { get; set; }
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public string? Comments { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
    public class UpdateWorkflowHistorydto
    {
        public Guid HistoryId { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public Guid WorkflowTaskId { get; set; }
        public string? Action { get; set; }
        public Guid ActorUserId { get; set; }
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public string? Comments { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
    public class WorkflowHistorydto {
        public Guid HistoryId { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public Guid WorkflowTaskId { get; set; }
        public string? Action { get; set; }
        public Guid ActorUserId { get; set; }
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public string? Comments { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
