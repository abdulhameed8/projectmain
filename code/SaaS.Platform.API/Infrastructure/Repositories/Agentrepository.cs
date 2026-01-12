using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Agent-specific operations
    /// </summary>
    public class AgentRepository : GenericRepository<Agents>, IAgentRepository
    {
        public AgentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Agents?> GetByAgentCodeAsync(Guid tenantId, string agentCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.AgentCode == agentCode);
        }

        public async Task<IEnumerable<Agents>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Agents>> GetActiveAgentsAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive && c.AgentStatus == "Active")
                .ToListAsync();
        }

        
        public async Task<bool> IsAgentCodeUniqueAsync(Guid tenantId, string agentCode, Guid? excludeTenantId = null)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId && c.AgentCode == agentCode);

            if (excludeTenantId.HasValue)
            {
                query = query.Where(c => c.TenantId != excludeTenantId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Agents>> SearchAgentsAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    (c.AgentCode.ToLower().Contains(lowerSearchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<Agents> Items, int TotalCount)> GetAgentsPagedAsync(
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
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(c =>
                    (c.AgentCode.ToLower().Contains(lowerSearchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.AgentStatus == status);
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