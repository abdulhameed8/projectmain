using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class PaymentMethods : BaseEntity
    {
        public Guid PaymentMethodId { get; set; }
        public Guid TenantId { get; set; }
        public string? MethodType { get; set; }
        public string? CardholderName { get; set; }
        public string? Last4Digit { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
       

    }
}
