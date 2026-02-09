using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for LoanType-specific operations
    /// </summary>
    public interface ILoanTypesRepository : IGenericRepository<LoanTypes>
    {
        Task<LoanTypes?> GetByLoanTypesCodeAsync(Guid tenantId, string loanTypeCode);
        Task<IEnumerable<LoanTypes>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<LoanTypes>> GetActiveLoanTypesAsync(Guid tenantId);
        Task<IEnumerable<LoanTypes>> SearchLoanTypesAsync(Guid tenantId, string searchTerm);
        Task<bool> IsLoanTypesCodeUniqueAsync(Guid tenantId, string loanTypeCode, Guid? excludeLoanTypeId = null);
        Task<(IEnumerable<LoanTypes> Items, int TotalCount)> GetLoanTypesPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);

    }
}