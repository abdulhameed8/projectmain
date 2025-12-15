using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Opportunity-specific operations
    /// </summary>
    public class OpportunityRepository : GenericRepository<Opportunities>, IOpportunityRepository
    {
        public OpportunityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Opportunities?> GetByOpportunityCodeAsync(Guid tenantId, string opportuintyCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.OpportunityCode == opportuintyCode);
        }

        public async Task<IEnumerable<Opportunities>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Opportunities>> GetActiveOpportunityAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId )
                .ToListAsync();
        }

       
        public async Task<bool> IsOpportunityCodeUniqueAsync(Guid tenantId, string opportunityCode, Guid? excludeOpportunityId = null)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId && c.OpportunityCode == opportunityCode);

            if (excludeOpportunityId.HasValue)
            {
                query = query.Where(c => c.CustomerId != excludeOpportunityId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Opportunities>> SearchOpportunityAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    
                    ( c.OpportunityName != null && c.OpportunityName.ToLower().Contains(lowerSearchTerm) ||
                     c.OpportunityCode.ToLower().Contains(lowerSearchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<Opportunities> Items, int TotalCount)> GetOpportunityPagedAsync(
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
                    (c.OpportunityName != null && c.OpportunityName.ToLower().Contains(lowerSearchTerm)) ||
                    c.OpportunityCode.ToLower().Contains(lowerSearchTerm));
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