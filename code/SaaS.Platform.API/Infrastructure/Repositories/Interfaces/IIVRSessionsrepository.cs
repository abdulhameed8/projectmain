using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for IVRSessions-specific operations
    /// </summary>
    public interface IIVRSessionsRepository : IGenericRepository<IVRSessions>
    {
        Task<IEnumerable<IVRSessions>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<IVRSessions>> GetActiveIVRSessionsAsync(Guid tenantId);
        Task<IEnumerable<IVRSessions>> SearchIVRSessionsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<IVRSessions> Items, int TotalCount)> GetIVRSessionsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);

    }
}