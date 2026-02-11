namespace SaaS.Platform.API.Application.DTOs.Payment
{
    public class CreatePaymentsdto
    {
        public Guid PaymentId { get; set; }
        public Guid TenantId { get; set; }
        public Guid InvoiceId { get; set; }
        public string? PaymentNumber { get; set; }
        public DateOnly PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string? TransactionReference { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
    public class UpdatePaymentsdto
    {
        public Guid PaymentId { get; set; }
        public Guid TenantId { get; set; }
        public Guid InvoiceId { get; set; }
        public string? PaymentNumber { get; set; }
        public DateOnly PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string? TransactionReference { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
    public class Paymentsdto
    {
        public Guid PaymentId { get; set; }
        public Guid TenantId { get; set; }
        public Guid InvoiceId { get; set; }
        public string? PaymentNumber { get; set; }
        public DateOnly PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string? TransactionReference { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
}
