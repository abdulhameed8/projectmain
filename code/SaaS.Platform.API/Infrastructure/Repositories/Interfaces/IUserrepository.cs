using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Customer-specific operations
    /// </summary>
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<IEnumerable<User>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<User>> GetActiveUsersAsync(Guid tenantId);
        Task<IEnumerable<User>> GetByEmailAsync(string email);
        Task<IEnumerable<User>> SearchUsersAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<User> Items, int TotalCount)> GetUsersPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}