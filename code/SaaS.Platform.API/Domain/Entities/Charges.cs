using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Charges : BaseEntity
    {
        public Guid ChargeId { get; set; }
        public Guid TenantId { get; set; }
        public string? ChargeName { get; set; }
        public string? ChargeCode { get; set; } = string.Empty;
        public string? ChargeType { get; set; } = "Individual";
        public decimal Amount { get; set; }
        public decimal Percentage{ get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public string? ApplicableOn { get; set; }
        public bool IsActive { get; set; } = true;
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }

    }
}
