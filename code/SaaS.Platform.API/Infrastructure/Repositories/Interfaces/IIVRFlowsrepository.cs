using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for IVRFlows-specific operations
    /// </summary>
    public interface IIVRFlowsRepository : IGenericRepository<IVRFlows>
    {
        Task<IVRFlows?> GetByIVRFlowCodeAsync(Guid tenantId, string flowCode);
        Task<IEnumerable<IVRFlows>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<IVRFlows>> GetActiveIVRFlowsAsync(Guid tenantId);
        Task<IEnumerable<IVRFlows>> SearchIVRFlowsAsync(Guid tenantId, string searchTerm);
        Task<bool> IsIVRFlowCodeUniqueAsync(Guid tenantId, string flowCode, Guid? excludeIVRFlowId = null);
        Task<(IEnumerable<IVRFlows> Items, int TotalCount)> GetIVRFlowsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
       
    }
}