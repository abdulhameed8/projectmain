using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Domain.Entities.Roles.cs;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Roles-specific operations
    /// </summary>
    public class SystemSettingsRepository : GenericRepository<SystemSettings>, ISystemSettingsRepository
    {
        public SystemSettingsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SystemSettings>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<SystemSettings>> GetActiveSystemSettingsAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId )
                .ToListAsync();
        }


        public async Task<IEnumerable<SystemSettings>> SearchSystemSettingsAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId )
                    
                     .ToListAsync();
        }

        public async Task<(IEnumerable<SystemSettings> Items, int TotalCount)> GetSystemSettingsPagedAsync(
           Guid tenantId,
           string? searchTerm = null,
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