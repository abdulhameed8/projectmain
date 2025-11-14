using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for User-specific operations
    /// </summary>
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

       
        public async Task<IEnumerable<User>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && c.IsActive )
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Where(c => c.Email == email)
                .ToListAsync();
        }



        public async Task<IEnumerable<User>> SearchUsersAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.TenantId == tenantId &&
                    (c.FirstName != null && c.FirstName.ToLower().Contains(lowerSearchTerm) ||
                     c.LastName != null && c.LastName.ToLower().Contains(lowerSearchTerm) ||
                     c.Email != null && c.Email.ToLower().Contains(lowerSearchTerm) 
                     ))
                .ToListAsync();
        }


        public async Task<(IEnumerable<User> Items, int TotalCount)> GetUsersPagedAsync(
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
                    (c.Email != null && c.Email.ToLower().Contains(lowerSearchTerm)));
                    
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