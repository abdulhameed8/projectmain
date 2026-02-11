using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Payment specific operations
    /// </summary>
    public interface IPaymentsRepository : IGenericRepository<Payments>
    {
        Task<IEnumerable<Payments>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Payments>> GetActivePaymentsAsync(Guid tenantId);
        Task<IEnumerable<Payments>> SearchPaymentsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Payments> Items, int TotalCount)> GetPaymentsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
             string? status = null,
            int pageNumber = 1,
            int pageSize = 10);

    }
}
