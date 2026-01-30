namespace SaaS.Platform.API.Application.DTOs.IVRMenus
{
    public class CreateIVRMenusdto
    {
        public Guid IVRMenusId { get; set; }
        public Guid IVRFlowId { get; set; }
        public string? MenuName { get; internal set; }
        public Guid ParentMenuId { get; set; }
        public int MenuLevel { get; set; }
        public Guid PromptId { get; set; }
        public int TimeOutSeconds { get; set; }
        public int MaxRetries { get; set; }
        public string? InvalidInputAction { get; set; }
        public int SortOrder { get; set; }
        public  DateTime? CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }

    }
    public class UpdateIVRMenusdto
    {
        public Guid IVRMenusId { get; set; }
        public Guid IVRFlowId { get; set; }
        public string? MenuName { get; internal set; }
        public Guid ParentMenuId { get; set; }
        public int MenuLevel { get; set; }
        public Guid PromptId { get; set; }
        public int TimeOutSeconds { get; set; }
        public int MaxRetries { get; set; }
        public string? InvalidInputAction { get; set; }
        public int SortOrder { get; set; }
        public  DateTime? CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }

    }
    public class IVRMenusdto
    {
        public Guid IVRMenusId { get; set; }
        public Guid IVRFlowId { get; set; }
        public string? MenuName { get; internal set; }
        public Guid ParentMenuId { get; set; }
        public int MenuLevel { get; set; }
        public Guid PromptId { get; set; }
        public int TimeOutSeconds { get; set; }
        public int MaxRetries { get; set; }
        public string? InvalidInputAction { get; set; }
        public int SortOrder { get; set; }
        public  DateTime? CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }

    }
}
