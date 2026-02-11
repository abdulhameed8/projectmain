using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    public interface ISubscriptionsRepository : IGenericRepository<Subscriptions>
    {
        Task<IEnumerable<Subscriptions>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Subscriptions>> GetActiveSubscriptionsAsync(Guid tenantId);
        Task<IEnumerable<Subscriptions>> SearchSubscriptionsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Subscriptions> Items, int TotalCount)> GetSubscriptionsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
             string? status = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}
