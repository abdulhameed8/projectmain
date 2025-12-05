using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.Data;
using SaaS.Platform.API.Infrastructure.Repositories.Interfaces;

namespace SaaS.Platform.API.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for RolePermissions-specific operations
    /// </summary>
    public class RolePermissionsRepository : GenericRepository<RolePermissions>, IRolePermissionsrepository
    {
        public RolePermissionsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}