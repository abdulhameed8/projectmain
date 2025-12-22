using SaaS.Platform.API.Domain.Common;
using System.Numerics;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Documents : BaseEntity
    {
        public Guid DocumentId { get; set; }
        public Guid TenantId { get; set; }
        public string?  RelatedEntityType { get; set;}
        public Guid RelatedEntityId { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public BigInteger FileSizeBytes { get; set; }
        public string? StoragePath { get; set; }
        public string? FileUrl { get; set; }
        public string? MimeType  { get; set; }
        public Guid UploadedBy { get; set; }
        public new DateTime CreatedDate { get; set; }
    }
}
