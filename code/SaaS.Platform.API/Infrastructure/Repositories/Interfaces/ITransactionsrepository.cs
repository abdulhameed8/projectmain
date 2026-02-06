using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Transaction-specific operations
    /// </summary>
    public interface ITransactionsRepository : IGenericRepository<Transactions>
    {
        Task<IEnumerable<Transactions>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Transactions>> GetActiveTransactionsAsync(Guid tenantId);
        Task<IEnumerable<Transactions>> SearchTransactionsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Transactions> Items, int TotalCount)> GetTransactionsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);

    }
}