using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for BankAccount-specific operations
    /// </summary>
    public interface IBankAccountsRepository : IGenericRepository<BankAccounts>
    {
        Task<IEnumerable<BankAccounts>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<BankAccounts>> GetActiveBankAccountsAsync(Guid tenantId);
        Task<IEnumerable<BankAccounts>> SearchBankAccountsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<BankAccounts> Items, int TotalCount)> GetBankAccountsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
             string? status = null,
            int pageNumber = 1,
            int pageSize = 10);

    }
}