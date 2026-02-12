using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class WorkflowHistory : BaseEntity
    {
        public Guid HistoryId { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public Guid WorkflowTaskId { get; set; }
        public string? Action { get; set; }
        public Guid ActorUserId { get; set; }
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public string? Comments { get; set; }
        public  DateTime? TimeStamp { get; set; }
        }
}
