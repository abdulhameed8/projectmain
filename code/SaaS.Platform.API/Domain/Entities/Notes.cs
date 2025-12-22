using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Notes : BaseEntity
    {
        public Guid NoteId { get; set; }
        public Guid TenantId { get; set; }
        public string? EntityType { get; set; }
        public Guid EntityId { get; set; }
        public bool IsPrivate { get; set; } = true;
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        public new DateTime ModifiedDate { get; set; }

        public new Guid? ModifiedBy { get; set; }

    }
}
