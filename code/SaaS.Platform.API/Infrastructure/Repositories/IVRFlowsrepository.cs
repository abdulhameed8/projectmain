using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for IVRFlows-specific operations
    /// </summary>
    public class IVRFlowsRepository : GenericRepository<IVRFlows>, IIVRFlowsRepository
    {
        public IVRFlowsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IVRFlows?> GetByIVRFlowCodeAsync(Guid tenantId, string flowCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.FlowCode == flowCode);
        }

        public async Task<IEnumerable<IVRFlows>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<IVRFlows>> GetActiveIVRFlowsAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive )
                .ToListAsync();
        }

       
        public async Task<bool> IsIVRFlowCodeUniqueAsync(Guid tenantId, string flowCode, Guid? excludeIVRFlowId = null)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId && c.FlowCode == flowCode);

            if (excludeIVRFlowId.HasValue)
            {
                query = query.Where(c => c.TenantId != excludeIVRFlowId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<IVRFlows>> SearchIVRFlowsAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    ( c.FlowName != null && c.FlowName.ToLower().Contains(lowerSearchTerm) ||
                     c.FlowCode.ToLower().Contains(lowerSearchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<IVRFlows> Items, int TotalCount)> GetIVRFlowsPagedAsync(
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
                    (c.FlowName != null && c.FlowName.ToLower().Contains(lowerSearchTerm)) ||
                    c.FlowCode.ToLower().Contains(lowerSearchTerm));
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