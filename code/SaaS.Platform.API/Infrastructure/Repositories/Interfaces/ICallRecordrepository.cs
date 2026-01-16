using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for CallRecord-specific operations
    /// </summary>
    public interface ICallRecordRepository : IGenericRepository<CallRecord>
    {
        Task<IEnumerable<CallRecord>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<CallRecord>> GetActiveCallRecordsAsync(Guid tenantId);
        Task<IEnumerable<CallRecord>> SearchCallRecordsAsync(Guid tenantId, string searchTerm);
        Task<(IEnumerable<CallRecord> Items, int TotalCount)> GetCallRecordsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
             string? status = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}