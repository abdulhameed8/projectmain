namespace SaaS.Platform.API.Application.DTOs.NotificationTemplete
{
    public class CreateNotificationTempletesdto
    {
        public Guid TempleteId { get; set; }
        public Guid TenantId { get; set; }
        public string? TempleteName { get; set; }
        public string? TempleteCode { get; set; } = string.Empty;
        public string? NotificationType { get; set; } = "Individual";
        public string? Subject { get; set; }
        public string? BodyTemplete { get; set; }
        public string? Variable { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        
    }
    public class UpdateNotificationTempletesdto
    {
        public Guid TempleteId { get; set; }
        public Guid TenantId { get; set; }
        public string? TempleteName { get; set; }
        public string? TempleteCode { get; set; } = string.Empty;
        public string? NotificationType { get; set; } = "Individual";
        public string? Subject { get; set; }
        public string? BodyTemplete { get; set; }
        public string? Variable { get; set; }
        public bool? IsActive { get; set; } 
        public DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        
    }
    public class NotificationTempletesdto
    {
        public Guid TempleteId { get; set; }
        public Guid TenantId { get; set; }
        public string? TempleteName { get; set; }
        public string? TempleteCode { get; set; } = string.Empty;
        public string? NotificationType { get; set; } = "Individual";
        public string? Subject { get; set; }
        public string? BodyTemplete { get; set; }
        public string? Variable { get; set; }
        public bool IsActive { get; set; } 
        public  DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public  Guid ModifiedBy { get; set; }
    }
}
