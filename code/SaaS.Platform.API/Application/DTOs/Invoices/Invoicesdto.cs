namespace SaaS.Platform.API.Application.DTOs.Invoices
{
    public class CreateInvoicesdto
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
        public decimal PaidAmount { get; set; }
        public string? Status { get; set; }
        public DateOnly PaymentDate { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string? Notes { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
    public class UpdateInvoicesdto
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
        public decimal PaidAmount { get; set; }
        public string? Status { get; set; }
        public DateOnly PaymentDate { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string? Notes { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
    public class Invoicesdto
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
        public decimal PaidAmount { get; set; }
        public string? Status { get; set; }
        public DateOnly PaymentDate { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string? Notes { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
}
