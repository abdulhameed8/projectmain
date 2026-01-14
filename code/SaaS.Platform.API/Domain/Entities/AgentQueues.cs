using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{

    public class AgentQueues : BaseEntity
    {
        public Guid AgentQueuesId { get; set; }
        public Guid AgentId { get; set; }
        public Guid QueueId { get; set; }
        public int? Priority { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CeatedDate { get; set; }

    }
}