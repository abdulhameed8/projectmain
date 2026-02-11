using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for PaymentMethod-specific operations
    /// </summary>
    public interface IPaymentMethodsRepository : IGenericRepository<PaymentMethods>
    {
        Task<IEnumerable<PaymentMethods>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<PaymentMethods>> GetActivePaymentMethodsAsync(Guid tenantId);
        Task<IEnumerable<PaymentMethods>> SearchPaymentMethodsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<PaymentMethods> Items, int TotalCount)> GetPaymentMethodsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);

    }
}