namespace SaaS.Platform.API.Application.DTOs.Permissions
{
    public class CreatePermissionsdto
    {
        public Guid PermissionId { get; set; }
        public string? PermissionName { get; set; }
        public string PermissionCode { get; set; } = string.Empty;
        public string? Module { get; set; }
        public string? Description { get; set; }
    }

    public class UpdatePermissionsdto
    {
        public Guid PermissionId { get; set; }
        public string? PermissionName { get; set; }
        public string PermissionCode { get; set; } = string.Empty;
        public string? Module { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; } 
    }
    
    public class Permissionsdto
    {
        public Guid PermissionId { get; set; }
        public string? PermissionName { get; set; }
        public string PermissionCode { get; set; } = string.Empty;
        public string? Module { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; } 
    }

}

