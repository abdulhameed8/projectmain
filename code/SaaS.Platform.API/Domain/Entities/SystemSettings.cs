using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class SystemSettings : BaseEntity
    {
        public Guid SettingId { get; set; }
        public Guid TenantId { get; set; }
        public string? SettingKey { get; set; }
        public string? SettingValue { get; set; }
        public string? Datatype { get; set; }
        public string? Category { get; set; }
        public string? Descriptions { get; set; }
        public bool IsEncrypted { get; set; }
        public new DateTime ModifiedDate { get; set; }
        public new Guid ModifiedBy { get; set; }
    }
}
