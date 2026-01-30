using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class IVRPrompts : BaseEntity
    {
        public Guid PromptId { get; set; }
        public Guid TenantId { get; set; }
        public string? PromptName { get;  internal set; }
        public string? PromptType { get; set; }
        public string? FlowText { get; set; }
        public string? AudioUrl { get; set; }
        public string? Language { get; set; }
        public int DurationSeconds { get; set; }
        public   bool IsActive { get; set; } = true;
        public new DateTime? CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        public new DateTime? ModifiedDate { get; set; }
        public new Guid? ModifiedBy { get; set; }



    }
}



