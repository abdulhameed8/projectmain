using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for WorkflowDefinition-specific operations
    /// </summary>
    public class WorkflowInstancesRepository : GenericRepository<WorkflowInstances>, IWorkflowInstancesRepository
    {
        public WorkflowInstancesRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<IEnumerable<WorkflowInstances>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkflowInstances>> GetActiveWorkflowInstancesAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId )
                .ToListAsync();
        }


        public async Task<IEnumerable<WorkflowInstances>> SearchWorkflowInstancesAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId
                    )
                .ToListAsync();
        }

        public async Task<(IEnumerable<WorkflowInstances> Items, int TotalCount)> GetWorkflowInstancesPagedAsync(
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