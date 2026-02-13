namespace SaaS.Platform.API.Application.DTOs.SystemSettings
{
    public class CreateSystemSettingsdto
    {
        public Guid SettingId { get; set; }
        public Guid TenantId { get; set; }
        public string? SettingKey { get; set; }
        public string? SettingValue { get; set; }
        public string? Datatype { get; set; }
        public string? Category { get; set; }
        public string? Descriptions { get; set; }
        public bool IsEncrypted { get; set; }
    }
    public class UpdateSystemSettingsdto
    {
        public Guid SettingId { get; set; }
        public Guid TenantId { get; set; }
        public string? SettingKey { get; set; }
        public string? SettingValue { get; set; }
        public string? Datatype { get; set; }
        public string? Category { get; set; }
        public string? Descriptions { get; set; }
        public bool IsEncrypted { get; set; }
        
    }
    public class SystemSettingsdto
    {
        public Guid SettingId { get; set; }
        public Guid TenantId { get; set; }
        public string? SettingKey { get; set; }
        public string? SettingValue { get; set; }
        public string? Datatype { get; set; }
        public string? Category { get; set; }
        public string? Descriptions { get; set; }
        public bool IsEncrypted { get; set; }
        public  DateTime ModifiedDate { get; set; }
        public  Guid ModifiedBy { get; set; }
    }
}
