using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for WorkflowDefinitions-specific operations
    /// </summary>
    public interface IWorkflowDefinitionsRepository : IGenericRepository<WorkflowDefinitions>
    {
        Task<IEnumerable<WorkflowDefinitions>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<WorkflowDefinitions>> GetActiveWorkflowDefinitionsAsync(Guid tenantId);
        Task<IEnumerable<WorkflowDefinitions>> SearchWorkflowDefinitionsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<WorkflowDefinitions> Items, int TotalCount)> GetWorkflowDefinitionsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);

    }
}