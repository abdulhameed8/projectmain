namespace SaaS.Platform.API.Application.DTOs
{
    public class CreatePaymentMethodsdto
    {
        public Guid PaymentMethodId { get; set; }
        public Guid TenantId { get; set; }
        public string? MethodType { get; set; }
        public string? CardholderName { get; set; }
        public string? Last4Digit { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }

    }
    public class UpdatePaymentMethodsdto

    {
        public Guid PaymentMethodId { get; set; }
        public Guid TenantId { get; set; }
        public string? MethodType { get; set; }
        public string? CardholderName { get; set; }
        public string? Last4Digit { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public bool IsDefault { get; set; }
        public bool? IsActive { get; set; } 
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
    }
    public class PaymentMethodsdto
    {
        public Guid PaymentMethodId { get; set; }
        public Guid TenantId { get; set; }
        public string? MethodType { get; set; }
        public string? CardholderName { get; set; }
        public string? Last4Digit { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }

    }

}
