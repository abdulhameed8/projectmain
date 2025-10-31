using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Customer-specific operations
    /// </summary>
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetByCustomerCodeAsync(Guid tenantId, string customerCode);
        Task<IEnumerable<Customer>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Customer>> GetActiveCustomersAsync(Guid tenantId);
        Task<IEnumerable<Customer>> GetByEmailAsync(string email);
        Task<bool> IsCustomerCodeUniqueAsync(Guid tenantId, string customerCode, Guid? excludeCustomerId = null);
        Task<IEnumerable<Customer>> SearchCustomersAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Customer> Items, int TotalCount)> GetCustomersPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            string? status = null,
            string? segment = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}