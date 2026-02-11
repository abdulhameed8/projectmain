namespace SaaS.Platform.API.Application.DTOs.Subscriptions
{
    public class CreateSubscriptionsdto
    {
        public Guid SubscriptionId { get; set; }
        public Guid TenantId { get; set; }
        public Guid SubscriptionPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; }
        public string? BillingCycle { get; set; }
        public decimal Amount { get; set; }
        public DateOnly NextBillingDate { get; set; }
        public bool AutoRenew { get; set; }
        public DateTime CancellationDate { get; set; }
        public string? CancellationReason { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        public  DateTime ModifiedDate { get; set; }
        public  Guid ModifiedBy { get; set; }

    }
    public class UpdateSubscriptionsdto
    {
        public Guid SubscriptionId { get; set; }
        public Guid TenantId { get; set; }
        public Guid SubscriptionPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; }
        public string? BillingCycle { get; set; }
        public decimal Amount { get; set; }
        public DateOnly NextBillingDate { get; set; }
        public bool AutoRenew { get; set; }
        public DateTime CancellationDate { get; set; }
        public string? CancellationReason { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        public  DateTime ModifiedDate { get; set; }
        public  Guid ModifiedBy { get; set; }

    }
    public class Subscriptionsdto
    {
        public Guid SubscriptionId { get; set; }
        public Guid TenantId { get; set; }
        public Guid SubscriptionPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; }
        public string? BillingCycle { get; set; }
        public decimal Amount { get; set; }
        public DateOnly NextBillingDate { get; set; }
        public bool AutoRenew { get; set; }
        public DateTime CancellationDate { get; set; }
        public string? CancellationReason { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        public  DateTime ModifiedDate { get; set; }
        public  Guid ModifiedBy { get; set; }

    }
}
