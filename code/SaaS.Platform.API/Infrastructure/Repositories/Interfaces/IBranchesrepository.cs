using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Branches-specific operations
    /// </summary>
    public interface IBranchesRepository : IGenericRepository<Branches>
    {
        Task<Branches?> GetByBranchesCodeAsync(Guid tenantId, string branchCode);
        Task<IEnumerable<Branches>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Branches>> GetActiveBranchesAsync(Guid tenantId);
        Task<IEnumerable<Branches>> GetByEmailAsync(string email);
        Task<IEnumerable<Branches>> SearchBranchesAsync(Guid tenantId, string searchTerm);
        Task<bool> IsBranchesCodeUniqueAsync(Guid tenantId, string branchCode, Guid? excludebranchId = null);
        Task<(IEnumerable<Branches> Items, int TotalCount)> GetBranchesPagedAsync(
            Guid tenantId,
            string? searchTerm = null,     
            int pageNumber = 1,
            int pageSize = 10);
    }
}