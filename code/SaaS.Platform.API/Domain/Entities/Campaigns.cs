using SaaS.Platform.API.Domain.Common;
using System.Runtime.CompilerServices;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Campaigns : BaseEntity
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
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        public new DateTime ModifiedDate { get; set; }

        public new Guid? ModifiedBy { get; set; }


    }
}
