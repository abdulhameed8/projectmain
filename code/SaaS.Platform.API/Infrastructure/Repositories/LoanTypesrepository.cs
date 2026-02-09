using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for LoanTypes-specific operations
    /// </summary>
    public class LoanTypesRepository : GenericRepository<LoanTypes>, ILoanTypesRepository
    {
        public LoanTypesRepository (ApplicationDbContext context) : base(context)
        {
        }

        public async Task<LoanTypes?> GetByLoanTypesCodeAsync(Guid tenantId, string loanTypeCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.LoanTypeCode == loanTypeCode);
        }

        public async Task<IEnumerable<LoanTypes>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<LoanTypes>> GetActiveLoanTypesAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive)
                .ToListAsync();
        }

        public async Task<bool> IsLoanTypesCodeUniqueAsync(Guid tenantId, string loanTypeCode, Guid? excludeLoanTypeId = null)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId && c.LoanTypeCode == loanTypeCode);

            if (excludeLoanTypeId.HasValue)
            {
                query = query.Where(c => c.LoanTypeId != excludeLoanTypeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<LoanTypes>> SearchLoanTypesAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    (c.LoanTypeName != null && c.LoanTypeName.ToLower().Contains(lowerSearchTerm) ||
                     c.LoanTypeCode.ToLower().Contains(lowerSearchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<LoanTypes> Items, int TotalCount)> GetLoanTypesPagedAsync(
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
                    (c.LoanTypeName != null && c.LoanTypeName.ToLower().Contains(lowerSearchTerm)) ||
                     c.LoanTypeCode.ToLower().Contains(lowerSearchTerm));
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