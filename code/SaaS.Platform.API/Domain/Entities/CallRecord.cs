using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class CallRecord  : BaseEntity
    {
        public Guid CallRecordId { get; set; }
        public Guid TenantId { get; set; }
        public Guid CallId { get; set; }
        public string? CallType { get; set; }
        public string? CallDirection { get; set; }
        public string? CallerNumber { get; set; }
        public string? CalledNumber { get; set; }
        public Guid? AgentUserId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid QueueId { get; set; }
        public DateTime CallStartTime { get; set; }
        public DateTime CallEndTime { get; set; }
        public int? CallDurationSeconds { get; set; }
        public int? WaitTimeSeconds { get; set; }
        public int? TalkTimeSeconds { get; set; }
        public int? HoldTimeSeconds { get; set; }
        public string? CallStatus { get; set; }
        public Guid CallDispositionId { get; set; }
        public Guid TransferredToUserId { get; set; }
        public string? TransferCount { get; set; }
        public string? RecordingUrl { get; set; }
        public string? CallNotes { get; set; }
        public string? IVRPath { get; set; }
        public Decimal CallCost { get; set; }
        public  new required DateTime CreatedDate { get; set; }
    }
}
