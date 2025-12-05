namespace SaaS.Platform.API.Application.DTOs.RolePermissions
{
    public class CreateRolePermissionsdto 
    {
        public Guid RolePermissionId { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? CreatedBy { get; set; }

    }

    public class UpdateRolePermissionsdto
    {
        public Guid RolePermissionId { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid? CreatedBy { get; set; }

    }

    public class RolePermissionsdto
    {
        public Guid RolePermissionId { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

    }
}
