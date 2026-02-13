using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Notifications-specific operations
    /// </summary>
    public interface INotificationsRepository : IGenericRepository<Notifications>
    {
        Task<IEnumerable<Notifications>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Notifications>> GetActiveNotificationsAsync(Guid tenantId);
        Task<IEnumerable<Notifications>> SearchNotificationsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Notifications> Items, int TotalCount)> GetNotificationsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 10);

    }
}