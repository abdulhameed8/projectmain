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
    }
}