using SaaS.Platform.API.Domain.Entities;
using System.Threading.Tasks;

namespace SaaS.Platform.API.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Permissions-specific operations
    /// </summary>
    public interface IPermissionRepository : IGenericRepository<Permissions>
    {

        Task<Permissions?> GetByPermissionCodeAsync(string permissionCode);

        Task<bool> IsPermissionCodeUniqueAsync(string permissionCode, Guid? excludePermissionId = null);


    }
}