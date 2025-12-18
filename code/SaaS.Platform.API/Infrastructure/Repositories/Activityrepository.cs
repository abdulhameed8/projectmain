using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;
using System.Diagnostics;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Activity-specific operations
    /// </summary>
    public class ActivityRepository : GenericRepository<Activities>, IActivityRepository
    {
        public ActivityRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<IEnumerable<Activities>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }


        public async Task<IEnumerable<Activities>> GetActiveActivityAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.ActivityStatus == "Active")
                .ToListAsync();
        }

        public async Task<IEnumerable<Activities>> SearchActivityAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId 
                    )
                .ToListAsync();
        }




        public async Task<(IEnumerable<Activities> Items, int TotalCount)> GetActivityPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId);

            // Apply filters

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
               
            }


            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.ActivityStatus == status);
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