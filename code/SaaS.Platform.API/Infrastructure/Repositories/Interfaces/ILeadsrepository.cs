using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Leads-specific operations
    /// </summary>
    public interface ILeadsRepository : IGenericRepository<Leads>
    {
        Task<Leads?> GetByLeadsCodeAsync(Guid tenantId, string leadsCode);
        Task<IEnumerable<Leads>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Leads>> GetActiveLeadsAsync(Guid tenantId);
        Task<IEnumerable<Leads>> GetByEmailAsync(string email);
        Task<bool> IsLeadsCodeUniqueAsync(Guid tenantId, string leadsCode, Guid? excludeLeadId = null);
        Task<IEnumerable<Leads>> SearchLeadsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Leads> Items, int TotalCount)> GetLeadsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}