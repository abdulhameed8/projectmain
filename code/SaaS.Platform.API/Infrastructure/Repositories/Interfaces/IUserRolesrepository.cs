using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Customer-specific operations
    /// </summary>
    public interface IUserRolesRepository : IGenericRepository<UserRoles>
    {
        Task<IEnumerable<UserRoles>> GetByUserIdAsync(Guid userId);

        Task<IEnumerable<UserRoles>> SearchUsersRolesAsync(Guid userId, string searchTerm);
        Task<(IEnumerable<UserRoles> Items, int TotalCount)> GetUsersRolesPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}