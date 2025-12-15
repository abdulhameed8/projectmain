using Microsoft.AspNetCore.Http.HttpResults;
using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Leads : BaseEntity
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
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        public new DateTime ModifiedDate { get; set; }
        public new Guid? ModifiedBy { get; set; }

    }
}
