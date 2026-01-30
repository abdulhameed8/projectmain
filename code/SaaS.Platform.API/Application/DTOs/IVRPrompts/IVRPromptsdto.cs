namespace SaaS.Platform.API.Application.DTOs.IVRPrompts
{
    public interface CreateIVRPromptsdto
    {
        public Guid PromptId { get; set; }
        public Guid TenantId { get; set; }
        public string? PromptName { get; set; }
        public string? PromptType { get; set; }
        public string? FlowText { get; set; }
        public string? AudioUrl { get; set; }
        public string? Language { get; set; }
        public int DurationSeconds { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        

    }
    public class UpdateIVRPromptsdto
    {
        
        public string? PromptName { get; set; }
        public string? PromptType { get; set; }
        public string? FlowText { get; set; }
        public string? AudioUrl { get; set; }
        public string? Language { get; set; }
        public int DurationSeconds { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        

    }
    public class IVRPromptsdto
    {
        public Guid PromptId { get; set; }
        public Guid TenantId { get; set; }
        public string? PromptName { get; set; }
        public string? PromptType { get; set; }
        public string? FlowText { get; set; }
        public string? AudioUrl { get; set; }
        public string? Language { get; set; }
        public int DurationSeconds { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }

    }
}
