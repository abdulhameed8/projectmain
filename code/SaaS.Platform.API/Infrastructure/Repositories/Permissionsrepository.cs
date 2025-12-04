using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Permission-specific operations
    /// </summary>
    public class PermissionRepository : GenericRepository<Permissions>, IPermissionRepository
    {
        public PermissionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Permissions?> GetByPermissionCodeAsync(string permissionCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.PermissionCode == permissionCode);
        }






        public async Task<bool> IsPermissionCodeUniqueAsync(string permissionCode, Guid? excludePermissionId = null)
        {
            var query = _dbSet.Where(c => c.PermissionCode == permissionCode);

            if (excludePermissionId.HasValue)
            {
                query = query.Where(c => c.PermissionId != excludePermissionId.Value);
            }

            return !await query.AnyAsync();
        }

    }
}
