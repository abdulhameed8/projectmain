using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class IVRMenusOptions : BaseEntity
    {
        public Guid MenuOptionId { get; set; }
        public Guid IVRMenuId { get; set; }
        public string? OptionKey { get; internal set; }
        public string? OptionDescription { get; set; }
        public string? ActionType { get; set; }
        public string? ActionValue { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }
        public new DateTime? CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }




    }
}



