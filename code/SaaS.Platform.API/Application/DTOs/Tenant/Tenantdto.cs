namespace SaaS.Platform.API.Application.DTOs.Tenant
{
    public class CreateTenantdto
    {
        public Guid TenantId { get; set; }
        public required string TenantName { get; set; }
        public string TenantCode { get; set; } = string.Empty;
        public required string SubscriptionPlanID { get; set; }

        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Mobile { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        
        public string? SubscriptionStartDate { get; set; }
        public required string SubscriptionEndDate { get; set; }

        public required string MaxUsers { get; set; }
        public required string MaxStorageGB { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public string TenantStatus { get; set; } = "Active";
    }

    /// <summary>
    /// DTO for updating an existing Tenant
    /// </summary>
    public class UpdateTenantDto
    {
        public string? TenantName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Mobile { get; set; }

        public string? TenantStatus { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public bool? IsActive { get; set; }
        public string? SubscriptionStartDate { get; set; }
        public required string SubscriptionEndDate { get; set; }

        public required string MaxUsers { get; set; }
        public required string MaxStorageGB { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        


    }
    /// <summary>
    /// DTO for Tenant response
    /// </summary>
    public class TenantDto
    {
        public Guid TenantId { get; set; }
        public required string TenantName { get; set; }
        public string TenantCode { get; set; } = string.Empty;
        public required string SubscriptionPlanID { get; set; }

        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Mobile { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public bool IsActive { get; set; }
        public string? SubscriptionStartDate { get; set; }
        public required string SubscriptionEndDate { get; set; }

        public string TenantStatus { get; set; } = string.Empty;
        public required string MaxUsers { get; set; }
        public required string MaxStorageGB { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }


    }

}
