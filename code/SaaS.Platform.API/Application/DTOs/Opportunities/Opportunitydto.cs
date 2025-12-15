namespace SaaS.Platform.API.Application.DTOs.Opportunities
{
    public class CreateOpportunitydto
    {
        public Guid OpportunityId { get; set; }
        public Guid TenantId { get; set; }
        public Guid CustomerId { get; set; }
        public string OpportunityCode { get; set; } = string.Empty;
        public string? OpportunityName { get; set; }
        public string? Description { get; set; }
        public string? OpportunityType { get; set; }
        public string? Stage { get; set; }
        public Guid Amount { get; set; }
        public DateTime ExpectedCloseDate { get; set; }
        public DateTime ActualCloseDate { get; set; }
        public Guid? AssignedUserId { get; set; }
        public string? LeadSource { get; set; }
        public string? LostReason { get; set; }
        public string? NextStepAction { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        
    }

    public class UpdateOpportunitydto
    {
        public Guid OpportunityId { get; set; }
        public Guid TenantId { get; set; }
        public Guid CustomerId { get; set; }
        public string OpportunityCode { get; set; } = string.Empty;
        public string? OpportunityName { get; set; }
        public string? Description { get; set; }
        public string? OpportunityType { get; set; }
        public string? Stage { get; set; }
        public Guid Amount { get; set; }
        public DateTime ExpectedCloseDate { get; set; }
        public DateTime ActualCloseDate { get; set; }
        public Guid? AssignedUserId { get; set; }
        public string? LeadSource { get; set; }
        public string? LostReason { get; set; }
        public string? NextStepAction { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        

    }

    public class Opportunitydto
    {
        public Guid OpportunityId { get; set; }
        public Guid TenantId { get; set; }
        public Guid CustomerId { get; set; }
        public string OpportunityCode { get; set; } = string.Empty;
        public string? OpportunityName { get; set; }
        public string? Description { get; set; }
        public string? OpportunityType { get; set; }
        public string? Stage { get; set; }
        public Guid Amount { get; set; }
        public DateTime ExpectedCloseDate { get; set; }
        public DateTime ActualCloseDate { get; set; }
        public Guid? AssignedUserId { get; set; }
        public string? LeadSource { get; set; }
        public string? LostReason { get; set; }
        public string? NextStepAction { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        public  DateTime ModifiedDate { get; set; }
        public  Guid? ModifiedBy { get; set; }

    }
}
