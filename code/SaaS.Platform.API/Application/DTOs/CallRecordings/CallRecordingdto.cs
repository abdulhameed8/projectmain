namespace SaaS.Platform.API.Application.DTOs.CallRecordings
{
    public class CreateCallRecordingdto
    {
        public Guid RecordingId { get; set; }
        public Guid CallRecordId { get; set; }
        public string? RecordingUrl { get; set; }
        public int RecordingDurationSecond { get; set; }
        public string? RecordingFormat { get; set; }
        public int FileSizeByte { get; set; }
        public string? StoragePath { get; set; }
        public int IsTranscribed { get; set; }
        public string? TranscriptionText { get; set; }
        public DateTime RetentionDate { get; set; }
        public  DateTime? CreatedDate { get; set; }

    }
    public class UpdateCallRecordingdto
    {
        public Guid RecordingId { get; set; }
        public Guid CallRecordId { get; set; }
        public string? RecordingUrl { get; set; }
        public int RecordingDurationSecond { get; set; }
        public string? RecordingFormat { get; set; }
        public int FileSizeByte { get; set; }
        public string? StoragePath { get; set; }
        public int IsTranscribed { get; set; }
        public string? TranscriptionText { get; set; }
        public DateTime RetentionDate { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
    public class CallRecordingdto
    {
        public Guid RecordingId { get; set; }
        public Guid CallRecordId { get; set; }
        public string? RecordingUrl { get; set; }
        public int RecordingDurationSecond { get; set; }
        public string? RecordingFormat { get; set; }
        public int FileSizeByte { get; set; }
        public string? StoragePath { get; set; }
        public int IsTranscribed { get; set; }
        public string? TranscriptionText { get; set; }
        public DateTime RetentionDate { get; set; }
        public new DateTime? CreatedDate { get; set; }

    }
}
