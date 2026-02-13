using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for SystemSettings-specific operations
    /// </summary>
    public interface ISystemSettingsRepository : IGenericRepository<SystemSettings>
    {
        Task<IEnumerable<SystemSettings>> GetByTenantIdAsync(Guid tenantId);

        Task<IEnumerable<SystemSettings>> GetActiveSystemSettingsAsync(Guid tenantId);
        Task<IEnumerable<SystemSettings>> SearchSystemSettingsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<SystemSettings> Items, int TotalCount)> GetSystemSettingsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}