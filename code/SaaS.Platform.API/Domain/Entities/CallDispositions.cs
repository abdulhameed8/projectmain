using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class CallDispositions : BaseEntity
    {
        public Guid CallDispositionId { get; set; }
        public Guid TenantId { get; set; }
        public string DispositionCode { get; set; } = string.Empty;
        public string? DispositionName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? RequiresFollowUp { get; set; }
        public bool IsActive { get; set; } = true;
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
    }
}
