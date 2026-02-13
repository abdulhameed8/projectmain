namespace SaaS.Platform.API.Application.DTOs.Notifications
{
    public class CreateNotificationsdto
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
        public DateTime CreatedDate { get; set; }
    }
    public class UpdateNotificationsdto
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
        public  DateTime CreatedDate { get; set; }
    }
    public class Notificationsdto
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
        public  DateTime CreatedDate { get; set; }
    }
}
