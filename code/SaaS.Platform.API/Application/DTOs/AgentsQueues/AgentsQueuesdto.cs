namespace SaaS.Platform.API.Application.DTOs.AgentsQueues
{
    public class CreateAgentsQueuesdto
    {
        public Guid AgentQueuesId { get; set; }
        public Guid AgentId { get; set; }
        public Guid QueueId { get; set; }
        public int? Priority { get; set; }
        public DateTime CeatedDate { get; set; }
    }
    public class UpdateAgentsQueuesdto
    {
        public Guid AgentQueuesId { get; set; }
        public Guid AgentId { get; set; }
        public Guid QueueId { get; set; }
        public int? Priority { get; set; }
        public bool? IsActive { get; set; } 
        public DateTime CeatedDate { get; set; }
    }
    public class AgentQueuesdto
    {
        public Guid AgentQueuesId { get; set; }
        public Guid AgentId { get; set; }
        public Guid QueueId { get; set; }
        public int? Priority { get; set; }
        public bool IsActive { get; set; } 
        public DateTime CeatedDate { get; set; }
    }
}
