using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for AccountTypes-specific operations
    /// </summary>
    public interface IAccountTypesRepository : IGenericRepository<AccountTypes>
    {
        Task<AccountTypes?> GetByAccountTypesCodeAsync(Guid tenantId, string accountTypeCode);
        Task<IEnumerable<AccountTypes>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<AccountTypes>> GetActiveAccountTypesAsync(Guid tenantId);
        Task<IEnumerable<AccountTypes>> SearchAccountTypesAsync(Guid tenantId, string searchTerm);
        Task<bool> IsAccountTypesCodeUniqueAsync(Guid tenantId, string accountTypeCode, Guid? excludeAccountTypeId = null);
        Task<(IEnumerable<AccountTypes> Items, int TotalCount)> GetAccountTypesPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}