using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Customer-specific operations
    /// </summary>
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetByCustomerCodeAsync(Guid tenantId, string customerCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.CustomerCode == customerCode);
        }

        public async Task<IEnumerable<Customer>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetActiveCustomersAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive && c.CustomerStatus == "Active")
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Where(c => c.Email == email)
                .ToListAsync();
        }

        public async Task<bool> IsCustomerCodeUniqueAsync(Guid tenantId, string customerCode, Guid? excludeCustomerId = null)
        {
            var query = _dbSet.Where(c => c.TenantId == tenantId && c.CustomerCode == customerCode);

            if (excludeCustomerId.HasValue)
            {
                query = query.Where(c => c.CustomerId != excludeCustomerId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    (c.FirstName != null && c.FirstName.ToLower().Contains(lowerSearchTerm) ||
                     c.LastName != null && c.LastName.ToLower().Contains(lowerSearchTerm) ||
                     c.CompanyName != null && c.CompanyName.ToLower().Contains(lowerSearchTerm) ||
                     c.Email != null && c.Email.ToLower().Contains(lowerSearchTerm) ||
                     c.CustomerCode.ToLower().Contains(lowerSearchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<Customer> Items, int TotalCount)> GetCustomersPagedAsync(
            Guid tenantId,
            string? searchTerm = null,
            string? status = null,
            string? segment = null,
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
                    c.CustomerCode.ToLower().Contains(lowerSearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.CustomerStatus == status);
            }

            if (!string.IsNullOrWhiteSpace(segment))
            {
                query = query.Where(c => c.CustomerSegment == segment);
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