using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Notifications : BaseEntity
    {
        public Guid NotificationId { get; set; }
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public Guid TempleteId { get; set; } 
        public string? NotificationType { get; set; } = "Individual";
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? RecipientEmail { get; set; }
        public string? RecipientPhone { get; set; }
        public string? Status { get; set; }
        public DateOnly SentDate { get; set; }
        public DateOnly ReadDate { get; set; }
        public string? ErrorMassage { get; set; }
        public int RetryCount { get; set; }
        public int Priority { get; set; }
        public string? RelatedEntityType { get; set; }
       public Guid RelatedEntityId { get; set; }
        public new DateTime CreatedDate { get; set; }
        
    }
}
