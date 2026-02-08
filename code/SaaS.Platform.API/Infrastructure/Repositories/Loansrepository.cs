using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Loans-specific operations
    /// </summary>
    public class LoansRepository : GenericRepository<Loans>, ILoansRepository
    {
        public LoansRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<IEnumerable<Loans>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loans>> GetActiveLoansAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.LoanStatus == "Active")
                .ToListAsync();
        }



        public async Task<IEnumerable<Loans>> SearchLoansAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId
                    )
                .ToListAsync();
        }

        public async Task<(IEnumerable<Loans> Items, int TotalCount)> GetLoansPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId);

            // Apply filters


            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.LoanStatus == status);
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