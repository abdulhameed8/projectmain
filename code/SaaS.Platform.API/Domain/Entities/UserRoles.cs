using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class UserRoles : BaseEntity
    {
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }

    }
}
