using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class NotificationTempletes : BaseEntity
    {
        public Guid TempleteId { get; set; }
        public Guid TenantId { get; set; }
        public string? TempleteName { get; set; }
        public string? TempleteCode { get; set; } = string.Empty;
        public string? NotificationType { get; set; } = "Individual";
        public string? Subject { get; set; }
        public string? BodyTemplete { get; set; }
        public string? Variable { get; set; }
        public bool IsActive { get; set; } = true;
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        public new DateTime ModifiedDate { get; set; }
        public new Guid ModifiedBy { get; set; }
    }
}
