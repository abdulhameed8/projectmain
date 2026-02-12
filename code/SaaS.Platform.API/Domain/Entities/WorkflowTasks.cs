using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class WorkflowTasks : BaseEntity
    {
        public Guid WorkflowTaskId { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public Guid CamundaTaskId { get;  set; }
        public string? TaskName { get; set; }
        public string? TaskKey { get; set; }
        public Guid AssignedUserId { get; set; }
        public Guid AssignedRoleId { get; set; }
        public string? Status{ get; set; } 
        public int Priority  { get; set; }
        public DateOnly DueDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public Guid CompletedBy { get; set; }
        public string? FormData { get; set; }
        public string? Comments { get; set; }
        public new DateTime? CreatedDate { get; set; }
    }

}
