using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for NotificationTempletes-specific operations
    /// </summary>
    public interface INotificationTempletesRepository : IGenericRepository<NotificationTempletes>
    {
        Task<IEnumerable<NotificationTempletes>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<NotificationTempletes>> GetActiveNotificationTempletesAsync(Guid tenantId);
        Task<IEnumerable<NotificationTempletes>> SearchNotificationTempletesAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<NotificationTempletes> Items, int TotalCount)> GetNotificationTempletesPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);

    }
}