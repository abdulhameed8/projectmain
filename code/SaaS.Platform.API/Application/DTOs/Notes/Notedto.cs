namespace SaaS.Platform.API.Application.DTOs.Notes
{
    public class CreateNotedto
    {
        public Guid TenantId { get; set; }
        public string? EntityType { get; set; }
        public Guid EntityId { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
    public class UpdateNotedto
    {
        
        public string? EntityType { get; set; }
        public Guid EntityId { get; set; }
        public bool? IsPrivate { get; set; } 
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
    public class Notedto
    {
        public Guid NoteId { get; set; }
        public Guid TenantId { get; set; }
        public string? EntityType { get; set; }
        public Guid EntityId { get; set; }
        public bool IsPrivate { get; set; } 
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public  Guid? ModifiedBy { get; set; }
    }
}
