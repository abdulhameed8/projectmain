using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for AuditLogs-specific operations
    /// </summary>
    public interface IAuditLogsRepository : IGenericRepository<AuditLogs>
    {
        
        Task<IEnumerable<AuditLogs>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<AuditLogs>> GetActiveAuditLogsAsync(Guid tenantId);
        Task<IEnumerable<AuditLogs>> SearchAuditLogsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<AuditLogs> Items, int TotalCount)> GetAuditLogsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}
