using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Customer-specific operations
    /// </summary>
    public class InvoicesRepository : GenericRepository<Invoices>, IInvoicesRepository
    {
        public InvoicesRepository(ApplicationDbContext context) : base(context)
        {
        }

       

        public async Task<IEnumerable<Invoices>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoices>> GetActiveInvoicesAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId &&  c.Status == "Active")
                .ToListAsync();
        }

           public async Task<IEnumerable<Invoices>> SearchInvoicesAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId )
                .ToListAsync();
        }

        public async Task<(IEnumerable<Invoices> Items, int TotalCount)> GetInvoicesPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId);

            

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.Status == status);
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