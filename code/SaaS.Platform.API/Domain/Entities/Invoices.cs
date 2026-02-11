using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Invoices : BaseEntity
    {
        public Guid InvoiceId { get; set; }
        public Guid TenantId { get; set; }
        public Guid SubscriptionId { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal SubTotalDecimal { get; set; } 
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount{ get; set; }
        public string? Status { get; set; } 
        public DateOnly PaymentDate { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string? Notes { get; set; }
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        

    }
}
