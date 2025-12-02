using Microsoft.AspNetCore.Http.HttpResults;
using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities.Roles.cs
{
    public class Roles : BaseEntity
    {
     
        public Guid RoleId { get; set; }
        public Guid TenantId { get; set; }
        public string? RoleName { get; set; } 
        public string? RoleDescription { get; set; }
        public bool IsSystemRole { get; set; }
        public bool IsActive { get; set; } = true;
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }

        public new DateTime? ModifiedDate { get; set; }

        public new Guid? ModifiedBy { get; set; }



    }
}
