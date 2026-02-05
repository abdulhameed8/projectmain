using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Beneficiary-specific operations
    /// </summary>
    public interface IBeneficiariesRepository : IGenericRepository<Beneficiaries>
    {
        Task<Beneficiaries?> GetByBankCodeAsync(Guid tenantId, string bankCode);
        Task<IEnumerable<Beneficiaries>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Beneficiaries>> GetActiveBeneficiariesAsync(Guid tenantId);
        Task<IEnumerable<Beneficiaries>> SearchBeneficiariesAsync(Guid tenantId, string searchTerm);
        Task<bool> IsBankCodeUniqueAsync(Guid tenantId, string bankCode, Guid? excludeBeneficiaryId = null);
        Task<(IEnumerable<Beneficiaries> Items, int TotalCount)> GetBeneficiariesPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);

    }
}