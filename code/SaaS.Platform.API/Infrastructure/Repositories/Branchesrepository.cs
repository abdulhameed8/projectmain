using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Branch-specific operations
    /// </summary>
    public class BranchesRepository : GenericRepository<Branches>, IBranchesRepository
    {
        public BranchesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Branches?> GetByBranchesCodeAsync(Guid tenantId, string branchCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.BranchCode == branchCode);
        }

        public async Task<IEnumerable<Branches>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Branches>> GetActiveBranchesAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive )
                .ToListAsync();
        }

        public async Task<IEnumerable<Branches>> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Where(c => c.Email == email)
                .ToListAsync();
        }

        public async Task<bool> IsBranchesCodeUniqueAsync(Guid tenantId, string branchCode, Guid? excludeBranchId = null)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId && c.BranchCode == branchCode);

            if (excludeBranchId.HasValue)
            {
                query = query.Where(c => c.BranchId != excludeBranchId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Branches>> SearchBranchesAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    ( c.BranchName != null && c.BranchName.ToLower().Contains(lowerSearchTerm) ||
                     c.Email != null && c.Email.ToLower().Contains(lowerSearchTerm) ||
                     c.BranchCode.ToLower().Contains(lowerSearchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<Branches> Items, int TotalCount)> GetBranchesPagedAsync(
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
                    (
                    (c.BranchName != null && c.BranchName.ToLower().Contains(lowerSearchTerm)) ||
                    (c.Email != null && c.Email.ToLower().Contains(lowerSearchTerm)) ||
                    c.BranchCode.ToLower().Contains(lowerSearchTerm)));
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