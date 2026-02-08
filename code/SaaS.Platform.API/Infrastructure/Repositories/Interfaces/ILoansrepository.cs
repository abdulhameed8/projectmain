using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for loans-specific operations
    /// </summary>
    public interface ILoansRepository : IGenericRepository<Loans>
    {
        Task<IEnumerable<Loans>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Loans>> GetActiveLoansAsync(Guid tenantId);
        Task<IEnumerable<Loans>> SearchLoansAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Loans> Items, int TotalCount)> GetLoansPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}