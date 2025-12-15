namespace SaaS.Platform.API.Application.DTOs.Leads
{
    public class CreateLeadsdto
    {
        public Guid LeadId { get; set; }
        public Guid TenantId { get; set; }
        public string LeadsCode { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? LeadSource { get; set; }
        public string? LeadStatus { get; set; }
        public string? LeadScore { get; set; }
        public string? Industry { get; set; }
        public string? EstimatedValue { get; set; }
        public Guid? AssignedUserId { get; set; }
        public Guid ConvertedToCustomerId { get; set; }
        public Guid ConvertedDate { get; set; }
        public string? Notes { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        
    }

    public class UpdateLeadsdto
    {
        public Guid LeadId { get; set; }
        public Guid TenantId { get; set; }
        public string LeadsCode { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? LeadSource { get; set; }
        public string? LeadStatus { get; set; }
        public string? LeadScore { get; set; }
        public string? Industry { get; set; }
        public string? EstimatedValue { get; set; }
        public Guid? AssignedUserId { get; set; }
        public Guid ConvertedToCustomerId { get; set; }
        public Guid ConvertedDate { get; set; }
        public string? Notes { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        
    }

    public class Leadsdto
    {
        public Guid LeadId { get; set; }
        public Guid TenantId { get; set; }
        public string LeadsCode { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? LeadSource { get; set; }
        public string? LeadStatus { get; set; }
        public string? LeadScore { get; set; }
        public string? Industry { get; set; }
        public string? EstimatedValue { get; set; }
        public Guid? AssignedUserId { get; set; }
        public Guid ConvertedToCustomerId { get; set; }
        public Guid ConvertedDate { get; set; }
        public string? Notes { get; set; }
        public  DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public  DateTime? ModifiedDate { get; set; }
        public  Guid? ModifiedBy { get; set; }
    }
}
