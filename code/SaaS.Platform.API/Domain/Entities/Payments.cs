using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Payments : BaseEntity
    {
        public Guid PaymentId { get; set; }
        public Guid TenantId { get; set; }
        public Guid InvoiceId { get; set; }
        public string? PaymentNumber { get; set; }
        public DateOnly  PaymentDate{ get; set; }
        public decimal Amount { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string? TransactionReference { get; set; }
        public string? Status{ get; set; } 
        public string? Notes { get; set; }
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }

    }
}
