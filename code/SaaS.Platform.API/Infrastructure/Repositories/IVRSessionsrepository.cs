using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for IVRSessions-specific operations
    /// </summary>
    public class IVRSessionsRepository : GenericRepository<IVRSessions>, IIVRSessionsRepository
    {
        public IVRSessionsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<IVRSessions>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<IVRSessions>> GetActiveIVRSessionsAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId )
                .ToListAsync();
        }


        public async Task<IEnumerable<IVRSessions>> SearchIVRSessionsAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId 
                    )
                .ToListAsync();
        }

        public async Task<(IEnumerable<IVRSessions> Items, int TotalCount)> GetIVRSessionsPagedAsync(
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
