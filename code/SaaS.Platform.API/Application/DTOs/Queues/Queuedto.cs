namespace SaaS.Platform.API.Application.DTOs.Queues
{
    public class CreateQueuedto
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
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        
    }
    public class UpdateQueuedto
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
        public bool? IsActive { get; set; } 
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        
    }

    public class Queuedto
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
        public bool IsActive { get; set; } 
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        public  DateTime ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }

    }
}
