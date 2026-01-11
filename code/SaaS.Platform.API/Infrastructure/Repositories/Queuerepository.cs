using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Queue-specific operations
    /// </summary>
    public class QueueRepository : GenericRepository<Queues>, IQueueRepository
    {
        public QueueRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Queues?> GetByQueueCodeAsync(Guid tenantId, string queueCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.QueueCode == queueCode);
        }

        public async Task<IEnumerable<Queues>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Queues>> GetActiveQueuesAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive )
                .ToListAsync();
        }


        public async Task<bool> IsQueuesCodeUniqueAsync(Guid tenantId, string queueCode, Guid? excludeQueueId = null)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId && c.QueueCode == queueCode);

            if (excludeQueueId.HasValue)
            {
                query = query.Where(c => c.QueueId != excludeQueueId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Queues>> SearchQueuesAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    (c.QueueName != null && c.QueueName.ToLower().Contains(lowerSearchTerm) ||
                     c.QueueCode.ToLower().Contains(lowerSearchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<Queues> Items, int TotalCount)> GetQueuesPagedAsync(
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
                    (
                    (c.QueueName != null && c.QueueName.ToLower().Contains(lowerSearchTerm)) ||
                    c.QueueCode.ToLower().Contains(lowerSearchTerm)));
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