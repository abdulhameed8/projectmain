using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Leads-specific operations
    /// </summary>
    public class LeadsRepository : GenericRepository<Leads>, ILeadsRepository
    {
        public LeadsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Leads?> GetByLeadsCodeAsync(Guid tenantId, string leadsCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.LeadsCode == leadsCode);
        }

        public async Task<IEnumerable<Leads>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Leads>> GetActiveLeadsAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
                
        }

        public async Task<IEnumerable<Leads>> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Where(c => c.Email == email)
                .ToListAsync();
        }

        public async Task<bool> IsLeadsCodeUniqueAsync(Guid tenantId, string leadsCode, Guid? excludeLeadId = null)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId && c.LeadsCode == leadsCode);

            if (excludeLeadId.HasValue)
            {
                query = query.Where(c => c.LeadId != excludeLeadId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Leads>> SearchLeadsAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    (c.FirstName != null && c.FirstName.ToLower().Contains(lowerSearchTerm) ||
                     c.LastName != null && c.LastName.ToLower().Contains(lowerSearchTerm) ||
                     c.CompanyName != null && c.CompanyName.ToLower().Contains(lowerSearchTerm) ||
                     c.Email != null && c.Email.ToLower().Contains(lowerSearchTerm) ||
                     c.LeadsCode.ToLower().Contains(lowerSearchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<Leads> Items, int TotalCount)> GetLeadsPagedAsync(
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
                    (c.FirstName != null && c.FirstName.ToLower().Contains(lowerSearchTerm)) ||
                    (c.LastName != null && c.LastName.ToLower().Contains(lowerSearchTerm)) ||
                    (c.CompanyName != null && c.CompanyName.ToLower().Contains(lowerSearchTerm)) ||
                    (c.Email != null && c.Email.ToLower().Contains(lowerSearchTerm)) ||
                    c.LeadsCode.ToLower().Contains(lowerSearchTerm));
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