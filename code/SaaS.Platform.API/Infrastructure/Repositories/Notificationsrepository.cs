using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Notification-specific operations
    /// </summary>
    public class NotificationsRepository : GenericRepository<Notifications>, INotificationsRepository
    {
        public NotificationsRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<IEnumerable<Notifications>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notifications>> GetActiveNotificationsAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId )
                .ToListAsync();
        }


        public async Task<IEnumerable<Notifications>> SearchNotificationsAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId )
                .ToListAsync();
        }

        public async Task<(IEnumerable<Notifications> Items, int TotalCount)> GetNotificationsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId);

           

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