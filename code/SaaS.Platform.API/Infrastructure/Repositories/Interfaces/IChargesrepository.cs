using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Charges-specific operations
    /// </summary>
    public interface IChargesRepository : IGenericRepository<Charges>
    {
        Task<Charges?> GetByChargesCodeAsync(Guid tenantId, string chargeCode);
        Task<IEnumerable<Charges>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Charges>> GetActiveChargesAsync(Guid tenantId);
        Task<IEnumerable<Charges>> SearchChargesAsync(Guid tenantId, string searchTerm);
        Task<bool> IsChargesCodeUniqueAsync(Guid tenantId, string chargeCode, Guid? excludeChargeId = null);
        Task<(IEnumerable<Charges> Items, int TotalCount)> GetChargesPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);

    }
}