using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class EmailConfigurations : BaseEntity
    {
        public Guid EmailConfigId { get; set; }
        public Guid TenantId { get; set; }
        public string? SMTPHost { get; set; }
        public int SMTPPort { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool EnableSSL { get; set; }
        public string? FromEmail { get; set; }
        public string? FromName  { get; set; }
        public bool IsActive { get; set; } = true;
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        public new DateTime ModifiedDate { get; set; }
        public new Guid ModifiedBy { get; set; }
    }
}
