using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for cards-specific operations
    /// </summary>
    public interface ICardsRepository : IGenericRepository<Cards>
    {
        Task<IEnumerable<Cards>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Cards>> GetActiveCardsAsync(Guid tenantId);
        Task<IEnumerable<Cards>> SearchCardsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Cards> Items, int TotalCount)> GetCardsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}