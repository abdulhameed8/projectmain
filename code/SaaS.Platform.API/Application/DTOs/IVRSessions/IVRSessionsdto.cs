namespace SaaS.Platform.API.Application.DTOs.IVRSessions
{
    public interface CreateIVRSessionsdto
    {
        public Guid SessionId { get; set; }
        public Guid TenantId { get; set; }
        public Guid CallRecordId { get; set; }
        public Guid IVRFlowId { get; set; }
        public string? CallerNumber { get; set; }
        public DateTime SessionStartDate { get; set; }
        public DateTime SessionEndDate { get; set; }
        public int TotalDurationSeconds { get; set; }
        public string? MenuPathJson { get; set; }
        public string? InputsJson { get; set; }
        public string? FinalAction { get; set; }
        public Guid? CreatedBy { get; set; }
    }
    public class UpdateIVRSessionsdto
    {
        public Guid SessionId { get; set; }
        public Guid TenantId { get; set; }
        public Guid CallRecordId { get; set; }
        public Guid IVRFlowId { get; set; }
        public string? CallerNumber { get; set; }
        public DateTime SessionStartDate { get; set; }
        public DateTime SessionEndDate { get; set; }
        public int TotalDurationSeconds { get; set; }
        public string? MenuPathJson { get; set; }
        public string? InputsJson { get; set; }
        public string? FinalAction { get; set; }
        public Guid? CreatedBy { get; set; }
    }
    public class IVRSessionsdto
    {
        public Guid SessionId { get; set; }
        public Guid TenantId { get; set; }
        public Guid CallRecordId { get; set; }
        public Guid IVRFlowId { get; set; }
        public string? CallerNumber { get; set; }
        public DateTime SessionStartDate { get; set; }
        public DateTime SessionEndDate { get; set; }
        public int TotalDurationSeconds { get; set; }
        public string? MenuPathJson { get; set; }
        public string? InputsJson { get; set; }
        public string? FinalAction { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
