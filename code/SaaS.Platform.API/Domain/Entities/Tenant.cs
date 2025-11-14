using SaaS.Platform.API.Domain.Common;


namespace SaaS.Platform.API.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public Guid TenantId { get; set; }
        public  string? TenantName { get; set; }
        public string TenantCode { get; set; } = string.Empty;
        public required string SubscriptionPlanID { get; set; }

        public string TenantStatus { get; set; } = "Active"; // Active, Inactive, Blocked, Others
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Mobile { get; set; }
        
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public bool IsActive { get; set; } = true;
        public string? SubscriptionStartDate { get; set; }
        public required string SubscriptionEndDate { get; set; } 
       
        public required string MaxUsers { get; set; }
        public required string MaxStorageGB { get; set; }
        public new DateTime CreatedDate { get; set; } 
        public new Guid? CreatedBy { get; set; }
        public new DateTime ModifiedDate { get; set; }

        public new Guid? ModifiedBy { get; set; }
        

       
    }
}
    

