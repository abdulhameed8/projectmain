using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class CallRecordings : BaseEntity
    {
        public Guid RecordingId { get; set; }
        public Guid CallRecordId { get; set; }
        public string? RecordingUrl { get; set; }
        public int RecordingDurationSecond { get; set; }
        public string? RecordingFormat { get; set; }
        public int FileSizeByte { get; set; }
        public string? StoragePath { get; set; }
        public int IsTranscribed { get; set; }
        public string? TranscriptionText  { get; set; }
        public  DateTime RetentionDate{ get; set; }
        public new DateTime? CreatedDate{ get; set; }
        
    }
}
