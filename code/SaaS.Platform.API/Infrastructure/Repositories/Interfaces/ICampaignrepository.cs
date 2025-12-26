using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Campaign-specific operations
    /// </summary>
    public interface ICampaignRepository : IGenericRepository<Campaigns>
    {
        Task<IEnumerable<Campaigns>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Campaigns>> GetActiveCampaignsAsync(Guid tenantId);
        Task<IEnumerable<Campaigns>> SearchCampaignsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<Campaigns> Items, int TotalCount)> GetCampaignsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}