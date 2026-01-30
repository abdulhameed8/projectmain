namespace SaaS.Platform.API.Application.DTOs.IVRMenusOptions
{
    public class CreateIVRMenusOptionsdto
    {
        public Guid MenuOptionId { get; set; }
        public Guid IVRMenuId { get; set; }
        public string? OptionKey { get; internal set; }
        public string? OptionDescription { get; set; }
        public string? ActionType { get; set; }
        public string? ActionValue { get; set; }
        public int SortOrder { get; set; }
        public  DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
    }
    public class UpdateIVRMenusOptionsdto
    {
        public Guid MenuOptionId { get; set; }
        public Guid IVRMenuId { get; set; }
        public string? OptionKey { get; internal set; }
        public string? OptionDescription { get; set; }
        public string? ActionType { get; set; }
        public string? ActionValue { get; set; }
        public bool? IsActive { get; set; }
        public int SortOrder { get; set; }
        public  DateTime? CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
    public class IVRMenusOptionsdto
    {
        public Guid MenuOptionId { get; set; }
        public Guid IVRMenuId { get; set; }
        public string? OptionKey { get; internal set; }
        public string? OptionDescription { get; set; }
        public string? ActionType { get; set; }
        public string? ActionValue { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public  DateTime? CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
}
