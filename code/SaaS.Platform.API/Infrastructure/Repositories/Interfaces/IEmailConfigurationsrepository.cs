using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Transaction-specific operations
    /// </summary>
    public interface IEmailConfigurationsRepository : IGenericRepository<EmailConfigurations>
    {
        Task<IEnumerable<EmailConfigurations>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<EmailConfigurations>> GetActiveEmailConfigurationsAsync(Guid tenantId);
        Task<IEnumerable<EmailConfigurations>> SearchEmailConfigurationsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<EmailConfigurations> Items, int TotalCount)> GetEmailConfigurationsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}