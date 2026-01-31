using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public interface IVRSessions 
    {
        public Guid SessionId { get; set; }
        public Guid TenantId { get; set; }
        public Guid  CallRecordId { get;  set; }
        public Guid IVRFlowId { get; set; }
        public string? CallerNumber { get; set; }
        public DateTime SessionStartDate { get; set; }
        public DateTime SessionEndDate { get; set; }
        public int TotalDurationSeconds { get; set; }
        public string? MenuPathJson { get; set; }
        public string? InputsJson { get; set; }
        public string? FinalAction { get; set; }
        public  Guid? CreatedDate { get; set; }
    }
}
