using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for BankAccount-specific operations
    /// </summary>
    public class BankAccountsRepository : GenericRepository<BankAccounts>, IBankAccountsRepository
    {
        public BankAccountsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BankAccounts>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BankAccounts>> GetActiveBankAccountsAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId &&  c.AccountStatus == "Active")
                .ToListAsync();
        }

       
        public async Task<IEnumerable<BankAccounts>> SearchBankAccountsAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    (
                     c.AccountName != null && c.AccountName.ToLower().Contains(lowerSearchTerm)
                     ))
                .ToListAsync();
        }

        public async Task<(IEnumerable<BankAccounts> Items, int TotalCount)> GetBankAccountsPagedAsync(
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
                    (c.AccountName != null && c.AccountName.ToLower().Contains(lowerSearchTerm)) 
                    );
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.AccountStatus == status);
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