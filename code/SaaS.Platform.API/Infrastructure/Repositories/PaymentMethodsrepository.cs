using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for PaymentMethod-specific operations
    /// </summary>
    public class PaymentMethodsRepository : GenericRepository<PaymentMethods>, IPaymentMethodsRepository
    {
        public PaymentMethodsRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<IEnumerable<PaymentMethods>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentMethods>> GetActivePaymentMethodsAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive )
                .ToListAsync();
        }

        
        public async Task<IEnumerable<PaymentMethods>> SearchPaymentMethodsAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId 
                    )
                .ToListAsync();
        }

        public async Task<(IEnumerable<PaymentMethods> Items, int TotalCount)> GetPaymentMethodsPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId);

            

           

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
