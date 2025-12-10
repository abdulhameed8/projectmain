namespace SaaS.Platform.API.Application.DTOs.AuditLogs
{
    public class CreateAuditLogsdto
    {
        public Guid AuditLogId { get; set; }
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }

        public string? EntityName { get; set; }
        public Guid EntityId { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }
        public Guid IpAddress { get; set; }
        public required string UserAgent { get; set; }
    }
    public class UpdateAuditLogsdto
    {
        public Guid AuditLogId { get; set; }
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }

        public string? EntityName { get; set; }
        public Guid EntityId { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }
        public Guid IpAddress { get; set; }
        public required string UserAgent { get; set; }
    }

    public class AuditLogsdto
    {
        public Guid AuditLogId { get; set; }
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }

        public string? EntityName { get; set; }
        public Guid EntityId { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }
        public Guid IpAddress { get; set; }
        public required string UserAgent { get; set; }
    }
} 
