using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Customer-specific operations
    /// </summary>
    public interface IOpportunityRepository : IGenericRepository<Opportunities>
    {
        Task<Opportunities?> GetByOpportunityCodeAsync(Guid tenantId, string opportunityCode);
        Task<IEnumerable<Opportunities>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Opportunities>> GetActiveOpportunityAsync(Guid tenantId);
        
        Task<bool> IsOpportunityCodeUniqueAsync(Guid tenantId, string opportunityCode, Guid? excludeOpportunityId = null);
        Task<IEnumerable<Opportunities>> SearchOpportunityAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Opportunities> Items, int TotalCount)> GetOpportunityPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}