using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Agent-specific operations
    /// </summary>
    public interface IAgentRepository : IGenericRepository<Agents>
    {
        Task<Agents?> GetByAgentCodeAsync(Guid tenantId, string agentCode);
        Task<IEnumerable<Agents>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Agents>> GetActiveAgentsAsync(Guid tenantId);
        Task<IEnumerable<Agents>> SearchAgentsAsync(Guid tenantId, string searchTerm);
        Task<bool> IsAgentCodeUniqueAsync(Guid tenantId, string agentCode, Guid? excludeTenantId = null);
        Task<(IEnumerable<Agents> Items, int TotalCount)> GetAgentsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
             string? status = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}