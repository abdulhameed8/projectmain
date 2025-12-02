namespace SaaS.Platform.API.Application.DTOs.SubscriptionsPlan
{
    public class CreateSubscriptionsPlandto
    {
        public Guid SubscriptionPlanId { get; set; }
        public string? PlanName { get; set; }
        public  string PlanCode { get; set; } = string.Empty;
        public required string Description { get; set; }
        public required string BillingCycle { get; set; }
        public required string CurrencyCode { get; set; }
        public required string MaxUsers { get; set; }
        public required string MaxStorageGB { get; set; }
        public required string Features { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

    }


    /// <summary>
    /// DTO for updating an existing SubscriptionsPlan
    /// </summary>
    public class UpdateSubscriptionsPlandto
    {
        public Guid SubscriptionPlanId { get; set; }
        public string? PlanName { get; set; }
        public string PlanCode { get; set; } = string.Empty;
        public required string Description { get; set; }
        public required string BillingCycle { get; set; }
        public required string CurrencyCode { get; set; }
        public required string MaxUsers { get; set; }
        public required string MaxStorageGB { get; set; }
        public required string Features { get; set; }
        public bool? IsActive { get; set; } 
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Guid? ModifiedBy { get; set; }


    }

    /// <summary>
    /// DTO for SubscriptionsPlan response
    /// </summary>
    public class SubscriptionsPlanDto
    {
        public Guid SubscriptionPlanId { get; set; }
        public string? PlanName { get; set; }
        public string PlanCode { get; set; } = string.Empty;
        public required string Description { get; set; }
        public required string BillingCycle { get; set; }
        public required string CurrencyCode { get; set; }
        public bool? IsActive { get; set; } 
        public required string MaxUsers { get; set; }
        public required string MaxStorageGB { get; set; }
        public required string Features { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

    }
}
