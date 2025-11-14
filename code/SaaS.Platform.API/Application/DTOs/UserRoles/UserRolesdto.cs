namespace SaaS.Platform.API.Application.DTOs.UserRoles
{
    public class CreateUserRolesdto
    {
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }

  }
     // update userRolesdto
    public class UpdateUserRolesdto
    {
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }


    }

    // Dto for userRoles Response

    public class UserRolesdto
    {
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }

    }
}