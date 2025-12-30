namespace SaaS.Platform.API.Application.DTOs.CallDispositions
{
    public class CreateCallDispositiondto
    {
        public Guid TenantId { get; set; }
        public string DispositionCode { get; set; } = string.Empty;
        public string? DispositionName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? RequiresFollowUp { get; set; }
        
    }
    public class UpdateCallDispositiondto
    {
        
        public string DispositionCode { get; set; } = string.Empty;
        public string? DispositionName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? RequiresFollowUp { get; set; }
        public bool? IsActive { get; set; } 
        
    }
    public class CallDispositiondto
    {
        public Guid CallDispositionId { get; set; }
        public Guid TenantId { get; set; }
        public string DispositionCode { get; set; } = string.Empty;
        public string? DispositionName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? RequiresFollowUp { get; set; }
        public bool IsActive { get; set; } 
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
}
