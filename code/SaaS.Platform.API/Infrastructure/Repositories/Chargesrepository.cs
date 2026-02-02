using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Charges-specific operations
    /// </summary>
    public class ChargesRepository : GenericRepository<Charges>, IChargesRepository
    {
        public ChargesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Charges?> GetByChargesCodeAsync(Guid tenantId, string chargeCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.ChargeCode == chargeCode);
        }

        public async Task<IEnumerable<Charges>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Charges>> GetActiveChargesAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive )
                .ToListAsync();
        }

        public async Task<bool> IsChargesCodeUniqueAsync(Guid tenantId, string chargeCode, Guid? excludeChargeId = null)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId && c.ChargeCode == chargeCode);

            if (excludeChargeId.HasValue)
            {
                query = query.Where(c => c.ChargeId != excludeChargeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Charges>> SearchChargesAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    (c.ChargeName != null && c.ChargeName.ToLower().Contains(lowerSearchTerm) ||
                     c.ChargeCode.ToLower().Contains(lowerSearchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<Charges> Items, int TotalCount)> GetChargesPagedAsync(
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
                    (c.ChargeName != null && c.ChargeName.ToLower().Contains(lowerSearchTerm)) ||
                     c.ChargeCode.ToLower().Contains(lowerSearchTerm));
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