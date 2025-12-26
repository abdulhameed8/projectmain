namespace SaaS.Platform.API.Application.DTOs.Campaigns
{
    public class CreateCampaigndto
    {
        public Guid CampaignId { get; set; }
        public Guid TenantId { get; set; }
        public string CampaignType { get; set; } = "Individual"; // Individual, Corporate
        public string? CampaignName { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public required string Budget { get; set; }
        public required string ActualCost { get; set; }
        public required string ExpectedRevenue { get; set; }
        public string? Status { get; set; }
        public string? TargetAudience { get; set; }
        public  DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public  DateTime ModifiedDate { get; set; }

        public  Guid? ModifiedBy { get; set; }
    }

    public class UpdateCampaigndto
    {
        public Guid CampaignId { get; set; }
        public Guid TenantId { get; set; }
        public string CampaignType { get; set; } = "Individual"; // Individual, Corporate
        public string? CampaignName { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public required string Budget { get; set; }
        public required string ActualCost { get; set; }
        public required string ExpectedRevenue { get; set; }
        public string? Status { get; set; }
        public string? TargetAudience { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        
    }

    public class Campaigndto
    {
        public Guid CampaignId { get; set; }
        public Guid TenantId { get; set; }
        public string CampaignType { get; set; } = "Individual"; // Individual, Corporate
        public string? CampaignName { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public required string Budget { get; set; }
        public required string ActualCost { get; set; }
        public required string ExpectedRevenue { get; set; }
        public string? Status { get; set; }
        public string? TargetAudience { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        public  DateTime ModifiedDate { get; set; }

        public  Guid? ModifiedBy { get; set; }
    }
}
