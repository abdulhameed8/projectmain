using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for TransactionTypes-specific operations
    /// </summary>
    public interface ITransactionTypesRepository : IGenericRepository<TransactionTypes>
    {
        Task<TransactionTypes?> GetByTypeCodeAsync(Guid tenantId, string typeCode);
        Task<IEnumerable<TransactionTypes>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<TransactionTypes>> GetActiveTransactionTypesAsync(Guid tenantId);
        Task<IEnumerable<TransactionTypes>> SearchTransactionTypesAsync(Guid tenantId, string searchTerm);
        Task<bool> IsTypeCodeUniqueAsync(Guid tenantId, string typeCode, Guid? excludeTenantId = null);
        Task<(IEnumerable<TransactionTypes> Items, int TotalCount)> GetTransactionTypesPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);

    }
}