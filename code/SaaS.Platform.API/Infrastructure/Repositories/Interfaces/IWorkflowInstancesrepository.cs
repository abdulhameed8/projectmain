using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for WorkflowInstances-specific operations
    /// </summary>
    public interface IWorkflowInstancesRepository : IGenericRepository<WorkflowInstances>
    {
        Task<IEnumerable<WorkflowInstances>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<WorkflowInstances>> GetActiveWorkflowInstancesAsync(Guid tenantId);
        Task<IEnumerable<WorkflowInstances>> SearchWorkflowInstancesAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<WorkflowInstances> Items, int TotalCount)> GetWorkflowInstancesPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);

    }
}