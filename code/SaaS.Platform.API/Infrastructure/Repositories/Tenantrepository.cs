using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Tenant-specific operations
    /// </summary>
    public class TenantRepository : GenericRepository<Tenant>, ITenantRepository
    {
        public TenantRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Tenant?> GetByTenantCodeAsync(Guid tenantId, string tenantCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.TenantCode == tenantCode);
        }

        public async Task<IEnumerable<Tenant>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tenant>> GetActiveTenantsAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive && c.TenantStatus == "Active")
                .ToListAsync();
        }

        public async Task<IEnumerable<Tenant>> GetByContactEmailAsync(string Contactemail)
        {
            return await _dbSet
                .Where(c => c.ContactEmail == Contactemail)
                .ToListAsync();
        }

        public async Task<bool> IsTenantCodeUniqueAsync(Guid tenantId, string tenantCode, Guid? excludeTenantId = null)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId && c.TenantCode == tenantCode);

            if (excludeTenantId.HasValue)
            {
                query = query.Where(c => c.TenantId != excludeTenantId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Tenant>> SearchTenantsAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    (c.TenantName != null && c.TenantName.ToLower().Contains(lowerSearchTerm) ||
                     c.ContactEmail != null && c.ContactEmail.ToLower().Contains(lowerSearchTerm) ||
                     c.TenantCode.ToLower().Contains(lowerSearchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<Tenant> Items, int TotalCount)> GetTenantPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(c =>
                    (c.TenantName != null && c.TenantName.ToLower().Contains(lowerSearchTerm)) ||
                    
                    (c.ContactEmail != null && c.ContactEmail.ToLower().Contains(lowerSearchTerm)) ||
                    c.TenantCode.ToLower().Contains(lowerSearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.TenantStatus == status);
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