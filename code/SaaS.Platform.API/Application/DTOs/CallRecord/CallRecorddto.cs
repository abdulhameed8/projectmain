namespace SaaS.Platform.API.Application.DTOs.CallRecord
{
    
        public class CreateCallRecorddto
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
            public DateTime CreatedDate { get; set; }
        }
        public class UpdateCallRecorddto
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
            public DateTime CreatedDate { get; set; }
        }
        public class CallRecorddto
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
            public DateTime CreatedDate { get; set; }
        }
    }



