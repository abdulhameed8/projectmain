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
    public class RolesRepository : GenericRepository<Roles>, IRolesRepository
    {
        public RolesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Roles>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Roles>> GetActiveRolesAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive )
                .ToListAsync();
        }


        public async Task<IEnumerable<Roles>> SearchRolesAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    (c.RoleName != null && c.RoleName.ToLower().Contains(lowerSearchTerm)))
                     .ToListAsync();
        }

        public async Task<(IEnumerable<Roles> Items, int TotalCount)> GetRolesPagedAsync(
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
                (c.RoleName != null && c.RoleName.ToLower().Contains(lowerSearchTerm)));


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