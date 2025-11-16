using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Tenant-specific operations
    /// </summary>
    public interface ITenantRepository : IGenericRepository<Tenant>
    {
        Task<Tenant?> GetByTenantCodeAsync(Guid tenantId, string tenantCode);
        Task<IEnumerable<Tenant>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Tenant>> GetActiveTenantsAsync(Guid tenantId);
        Task<IEnumerable<Tenant>> GetByContactEmailAsync(string Contactemail);
        Task<bool> IsTenantCodeUniqueAsync(Guid tenantId, string tenantCode, Guid? excludeTenantId = null);
        Task<IEnumerable<Tenant>> SearchTenantsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Tenant> Items, int TotalCount)> GetTenantsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}