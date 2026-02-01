using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for AccountTypes-specific operations
    /// </summary>
    public class AccountTypesRepository : GenericRepository<AccountTypes>, IAccountTypesRepository
    {
        public AccountTypesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<AccountTypes?> GetByAccountTypesCodeAsync(Guid tenantId, string accountTypeCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.AccountTypeCode == accountTypeCode);
        }

        public async Task<IEnumerable<AccountTypes>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<AccountTypes>> GetActiveAccountTypesAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive )
                .ToListAsync();
        }

        public async Task<bool> IsAccountTypesCodeUniqueAsync(Guid tenantId, string accountTypeCode, Guid? excludeAccountTypeId = null)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId && c.AccountTypeCode == accountTypeCode);

            if (excludeAccountTypeId.HasValue)
            {
                query = query.Where(c => c.AccountTypeId != excludeAccountTypeId.Value);
            }

            return !await query.AnyAsync();
        }



        public async Task<IEnumerable<AccountTypes>> SearchAccountTypesAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    (
                     c.AccountTypeName != null && c.AccountTypeName.ToLower().Contains(lowerSearchTerm) ||
                     c.AccountTypeCode.ToLower().Contains(lowerSearchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<AccountTypes> Items, int TotalCount)> GetAccountTypesPagedAsync(
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
                    (c.AccountTypeName != null && c.AccountTypeName.ToLower().Contains(lowerSearchTerm)) ||
                    (c.AccountTypeCode.ToLower().Contains(lowerSearchTerm)));
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