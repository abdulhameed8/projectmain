using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for CallRecord-specific operations
    /// </summary>
    public class CallRecordRepository : GenericRepository<CallRecord>, ICallRecordRepository
    {
        public CallRecordRepository(ApplicationDbContext context) : base(context)
        {
        }

        
        public async Task<IEnumerable<CallRecord>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CallRecord>> GetActiveCallRecordsAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.CallStatus == "Active")
                .ToListAsync();
        }

        

        public async Task<IEnumerable<CallRecord>> SearchCallRecordsAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId
                    )
                .ToListAsync();
        }

        public async Task<(IEnumerable<CallRecord> Items, int TotalCount)> GetCallRecordsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId);

            
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.CallStatus == status);
            }

           

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}