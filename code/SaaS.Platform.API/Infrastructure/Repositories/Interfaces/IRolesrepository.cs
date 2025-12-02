using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Domain.Entities.Roles.cs;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Roles-specific operations
    /// </summary>
    public interface IRolesRepository : IGenericRepository<Roles>
    {
        Task<IEnumerable<Roles>> GetByTenantIdAsync(Guid tenantId);

        Task<IEnumerable<Roles>> GetActiveRolesAsync(Guid tenantId);
        Task<IEnumerable<Roles>> SearchRolesAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Roles> Items, int TotalCount)> GetRolesPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}