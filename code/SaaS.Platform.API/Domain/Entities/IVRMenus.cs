using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class IVRMenus : BaseEntity
    {
        public Guid IVRMenuId { get; set; }
        public Guid IVRFlowId { get; set; }
        public string? MenuName { get; internal set; }
        public Guid ParentMenuId { get; set; }
        public int MenuLevel { get; set; }
        public Guid PromptId { get; set; }
        public int TimeOutSeconds { get; set; }
        public int MaxRetries { get; set; } 
        public string? InvalidInputAction { get; set; }
        public int SortOrder { get; set; }
        public new DateTime? CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        



    }
}



