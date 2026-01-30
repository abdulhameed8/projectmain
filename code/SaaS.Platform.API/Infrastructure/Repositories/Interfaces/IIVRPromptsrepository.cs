using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for IVRPrompts-specific operations
    /// </summary>
    public interface IIVRPromptsRepository : IGenericRepository<IVRPrompts>
    {
        
        Task<IEnumerable<IVRPrompts>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<IVRPrompts>> GetActiveIVRPromptsAsync(Guid tenantId);
        Task<IEnumerable<IVRPrompts>> SearchIVRPromptsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<IVRPrompts> Items, int TotalCount)> GetIVRPromptsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}
