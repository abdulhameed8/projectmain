using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Invoice-specific operations
    /// </summary>
    public interface IInvoicesRepository : IGenericRepository<Invoices>
    {
        Task<IEnumerable<Invoices>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Invoices>> GetActiveInvoicesAsync(Guid tenantId);
        Task<IEnumerable<Invoices>> SearchInvoicesAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Invoices> Items, int TotalCount)> GetInvoicesPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
             string? status = null,
            int pageNumber = 1,
            int pageSize = 10);

    }
}