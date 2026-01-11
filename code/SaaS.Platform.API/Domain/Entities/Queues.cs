using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Queues : BaseEntity
    {
        public Guid QueueId { get; set; }
        public Guid TenantId { get; set; }
        public string QueueCode { get; set; } = string.Empty;
        public string? QueueName { get; set; }
        public string? Description { get; set; }
        public int? Priority { get; set; }
        public int? MAxWaitTimeSecond { get; set; }
        public string? RoutingStrategy { get; set; }
        public string? WelcomeMessageURL { get; set; }
        public string? MusicOnHoldURL { get; set; }
        public int? MaxQueueSize { get; set; }
        public bool IsActive { get; set; } = true;
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        public new DateTime ModifiedDate { get; set; }
        public new Guid? ModifiedBy { get; set; }


    }
}
