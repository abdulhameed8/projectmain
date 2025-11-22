namespace SaaS.Platform.API.Application.DTOs.UserRoles
{
    public class CreateUserRolesdto
    {
        public Guid UserRoleId { get; set; }
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }

        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }

    }

    public class UpdateUserRolesdto
    {
        public Guid UserRoleId { get; set; }
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }

        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }

    }

    // Dto for User Response

    public class UserRolesdto
    {
        public Guid UserRoleId { get; set; }
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }

        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
