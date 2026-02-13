namespace SaaS.Platform.API.Application.DTOs.EmailConfig
{
    public class CreateEmailConfigurationsdto
    {
        public Guid EmailConfigId { get; set; }
        public Guid TenantId { get; set; }
        public string? SMTPHost { get; set; }
        public int SMTPPort { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool EnableSSL { get; set; }
        public string? FromEmail { get; set; }
        public string? FromName { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
    public class UpdateEmailConfigurationsdto
    {
        public Guid EmailConfigId { get; set; }
        public Guid TenantId { get; set; }
        public string? SMTPHost { get; set; }
        public int SMTPPort { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool EnableSSL { get; set; }
        public string? FromEmail { get; set; }
        public string? FromName { get; set; }
        public bool? IsActive { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        
    }
    public class EmailConfigurationsdto
    {
        public Guid EmailConfigId { get; set; }
        public Guid TenantId { get; set; }
        public string? SMTPHost { get; set; }
        public int SMTPPort { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool EnableSSL { get; set; }
        public string? FromEmail { get; set; }
        public string? FromName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        public  DateTime ModifiedDate { get; set; }
        public  Guid ModifiedBy { get; set; }
    }
}
