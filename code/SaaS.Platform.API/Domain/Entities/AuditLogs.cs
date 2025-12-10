using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class AuditLogs : BaseEntity
    {
        public Guid AuditLogId { get; set; }
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        
        public string? EntityName { get; set; }
        public Guid EntityId { get; set; }

        public string? OldValues  {get; set;}

        public string? NewValues {get; set;}
       public Guid IpAddress { get; set; }
      public required string UserAgent { get; set; }
    }
}
