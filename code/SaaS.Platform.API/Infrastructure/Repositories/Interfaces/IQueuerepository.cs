using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Queue-specific operations
    /// </summary>
    public interface IQueueRepository : IGenericRepository<Queues>
    {
        Task<Queues?> GetByQueueCodeAsync(Guid tenantId, string queueCode);
        Task<IEnumerable<Queues>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Queues>> GetActiveQueuesAsync(Guid tenantId);
        Task<IEnumerable<Queues>> SearchQueuesAsync(Guid tenantId, string searchTerm);
        Task<bool> IsQueuesCodeUniqueAsync(Guid tenantId, string queueCode, Guid? excludeTenantId = null);
        Task<(IEnumerable<Queues> Items, int TotalCount)> GetQueuesPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}