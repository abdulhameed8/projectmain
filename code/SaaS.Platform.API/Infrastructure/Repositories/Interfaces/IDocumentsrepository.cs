using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Documents-specific operations
    /// </summary>
    public interface IDocumentsRepository : IGenericRepository<Documents>
    {
        Task<IEnumerable<Documents>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Documents>> GetActiveDocumentsAsync(Guid tenantId);
        Task<IEnumerable<Documents>> SearchDocumentsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Documents> Items, int TotalCount)> GetDocumentsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}