using SaaS.Platform.API.Domain.Common;
using System.Reflection;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Permissions : BaseEntity
    {
      
        public Guid PermissionId { get; set; }
        public string? PermissionName { get; set; }
        public string? PermissionCode { get; set; }
        public string? Module { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
