using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for CallDisposition-specific operations
    /// </summary>
    public interface ICallDispositionRepository : IGenericRepository<CallDispositions>
    {
        Task<CallDispositions?> GetByDispositionCodeAsync(Guid tenantId, string dispositionCode);
        Task<IEnumerable<CallDispositions>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<CallDispositions>> GetActiveCallDispositionsAsync(Guid tenantId);
        Task<IEnumerable<CallDispositions>> SearchCallDispositionsAsync(Guid tenantId, string searchTerm);
        Task<bool> IsDispositionCodeUniqueAsync(Guid tenantId, string dispositionCode, Guid? excludeTenantId = null);
        Task<(IEnumerable<CallDispositions> Items, int TotalCount)> GetCallDispositionsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}