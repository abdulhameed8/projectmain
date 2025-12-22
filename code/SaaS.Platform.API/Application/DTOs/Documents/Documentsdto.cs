using System.Numerics;

namespace SaaS.Platform.API.Application.DTOs.Documents
{
    public class CreateDocumentsdto
    {
        public Guid DocumentId { get; set; }
        public Guid TenantId { get; set; }
        public string? RelatedEntityType { get; set; }
        public Guid RelatedEntityId { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public BigInteger FileSizeBytes { get; set; }
        public string? StoragePath { get; set; }
        public string? FileUrl { get; set; }
        public string? MimeType { get; set; }
        public Guid UploadedBy { get; set; }
        public  DateTime CreatedDate { get; set; }
    }

    public class UpdateDocumentsdto
    {
        public Guid DocumentId { get; set; }
        public Guid TenantId { get; set; }
        public string? RelatedEntityType { get; set; }
        public Guid RelatedEntityId { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public BigInteger FileSizeBytes { get; set; }
        public string? StoragePath { get; set; }
        public string? FileUrl { get; set; }
        public string? MimeType { get; set; }
        public Guid UploadedBy { get; set; }
        public  DateTime CreatedDate { get; set; }
    }

    public class Documentsdto
    {
        public Guid DocumentId { get; set; }
        public Guid TenantId { get; set; }
        public string? RelatedEntityType { get; set; }
        public Guid RelatedEntityId { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public BigInteger FileSizeBytes { get; set; }
        public string? StoragePath { get; set; }
        public string? FileUrl { get; set; }
        public string? MimeType { get; set; }
        public Guid UploadedBy { get; set; }
        public  DateTime CreatedDate { get; set; }
    }
}
