using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Customer-specific operations
    /// </summary>
    public class NotificationTempletesRepository : GenericRepository<NotificationTempletes>, INotificationTempletesRepository
    {
        public NotificationTempletesRepository(ApplicationDbContext context) : base(context)
        {
        }

      
        public async Task<IEnumerable<NotificationTempletes>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<NotificationTempletes>> GetActiveNotificationTempletesAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive )
                .ToListAsync();
        }

      
        public async Task<IEnumerable<NotificationTempletes>> SearchNotificationTempletesAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    (c.TempleteName != null && c.TempleteName.ToLower().Contains(lowerSearchTerm) ||
                     c.TempleteCode.ToLower().Contains(lowerSearchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<NotificationTempletes> Items, int TotalCount)> GetNotificationTempletesPagedAsync(
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
                     (c.TempleteName != null && c.TempleteName.ToLower().Contains(lowerSearchTerm)) ||
                    c.TempleteCode.ToLower().Contains(lowerSearchTerm));
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