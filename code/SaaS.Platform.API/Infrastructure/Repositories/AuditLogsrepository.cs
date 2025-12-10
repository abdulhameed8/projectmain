using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for AuditLogs-specific operations
    /// </summary>
    public class AuditLogsRepository : GenericRepository<AuditLogs>, IAuditLogsRepository
    {
        public AuditLogsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AuditLogs>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLogs>> GetActiveAuditLogsAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId )
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLogs>> SearchAuditLogsAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                 ( c.EntityName != null && c.EntityName.ToLower().Contains(lowerSearchTerm)))
                  .ToListAsync();
        }

        public async Task<(IEnumerable<AuditLogs> Items, int TotalCount)> GetAuditLogsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(c =>

                    (c.EntityName != null && c.EntityName.ToLower().Contains(lowerSearchTerm)));
                    }

           

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
