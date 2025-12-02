namespace SaaS.Platform.API.Application.DTOs.Roles
{
    public class CreateRolesdto
    {
        public Guid RoleId { get; set; }
        public Guid TenantId { get; set; }
        public string? RoleName { get; set; }
        public string? RoleDescription { get; set; }
        public bool IsSystemRole { get; set; }
        public bool IsActive { get; set; } = true;
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }

        

    }

    public class UpdateRolesdto
    {
        public Guid RoleId { get; set; }
        public Guid TenantId { get; set; }
        public string? RoleName { get; set; }
        public string? RoleDescription { get; set; }
        public bool IsSystemRole { get; set; }
        public bool? IsActive { get; set; } 
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }

       

    }

    public class Rolesdto
    {
        public Guid RoleId { get; set; }
        public Guid TenantId { get; set; }
        public string? RoleName { get; set; }
        public string? RoleDescription { get; set; }
        public bool IsSystemRole { get; set; }
        public bool IsActive { get; set; } 
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }

        public  DateTime? ModifiedDate { get; set; }

        public  Guid? ModifiedBy { get; set; }


    }
}
