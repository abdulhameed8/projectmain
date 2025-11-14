using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for User-specific operations
    /// </summary>
    public class UserRolesRepository : GenericRepository<UserRoles>, IUserRolesRepository
    {
        public UserRolesRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<IEnumerable<UserRoles>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        



        public async Task<IEnumerable<UserRoles>> SearchUsersRolesAsync(Guid userId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.UserId == userId
                  )
                .ToListAsync();
        }


        public async Task<(IEnumerable<UserRoles> Items, int TotalCount)> GetUsersRolesPagedAsync(
            Guid userId,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _dbSet.Where(c => c.UserId == userId);

            // Apply filters
            





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
