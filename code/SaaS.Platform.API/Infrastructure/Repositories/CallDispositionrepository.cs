using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for CallDispositions-specific operations
    /// </summary>
    public class CallDispositionRepository : GenericRepository<CallDispositions>, ICallDispositionRepository
    {
        public CallDispositionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<CallDispositions?> GetByDispositionCodeAsync(Guid tenantId, string dispositionCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.DispositionCode == dispositionCode);
        }

        public async Task<IEnumerable<CallDispositions>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CallDispositions>> GetActiveCallDispositionsAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive)
                .ToListAsync();
        }



        public async Task<bool> IsDispositionCodeUniqueAsync(Guid tenantId, string dispositionCode, Guid? excludeTenantId = null)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId && c.DispositionCode == dispositionCode);

            if (excludeTenantId.HasValue)
            {
                query = query.Where(c => c.CallDispositionId != excludeTenantId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<CallDispositions>> SearchCallDispositionsAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    (c.DispositionCode.ToLower().Contains(lowerSearchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<CallDispositions> Items, int TotalCount)> GetCallDispositionsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(c =>
                    (c.DispositionCode.ToLower().Contains(lowerSearchTerm)));
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
