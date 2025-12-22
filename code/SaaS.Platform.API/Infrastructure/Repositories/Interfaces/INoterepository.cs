using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for  Note-specific operations
    /// </summary>
    public interface INoteRepository : IGenericRepository<Notes>
    {
        Task<IEnumerable<Notes>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Notes>> GetActiveNotesAsync(Guid tenantId);
        Task<IEnumerable<Notes>> SearchNotesAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Notes> Items, int TotalCount)> GetNotesPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}