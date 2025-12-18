using SaaS.Platform.API.Domain.Entities;
using System.Diagnostics;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Activity-specific operations
    /// </summary>
    public interface IActivityRepository : IGenericRepository<Activities>
    {
        Task<IEnumerable<Activities>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Activities>> GetActiveActivityAsync(Guid tenantId);
        Task<IEnumerable<Activities>> SearchActivityAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Activities> Items, int TotalCount)> GetActivityPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}