using Microsoft.AspNetCore.Http.HttpResults;
using SaaS.Platform.API.Domain.Common;
using SaaS.Platform.API.Domain.Entities.Roles.cs;

namespace SaaS.Platform.API.Domain.Entities
{
    public class RolePermissions : BaseEntity
    {
        public Guid RolePermissionId { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        public new DateTime CreatedDate { get; set; }

        public new Guid? CreatedBy { get; set; }

    }
}
