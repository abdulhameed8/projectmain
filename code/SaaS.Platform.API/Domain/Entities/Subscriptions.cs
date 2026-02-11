using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Subscriptions : BaseEntity
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
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        public new DateTime ModifiedDate { get; set; }
        public new Guid ModifiedBy { get; set; }

    }
}