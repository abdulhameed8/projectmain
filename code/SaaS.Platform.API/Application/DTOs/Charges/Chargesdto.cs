namespace SaaS.Platform.API.Application.DTOs.Charges
{
    public class CreateChargesdto
    {
        public Guid ChargeId { get; set; }
        public Guid TenantId { get; set; }
        public string? ChargeName { get; set; }
        public string? ChargeCode { get; set; } = string.Empty;
        public string? ChargeType { get; set; } = "Individual";
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public string? ApplicableOn { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }

    }
    public class UpdateChargesdto
    {
        public Guid ChargeId { get; set; }
        public Guid TenantId { get; set; }
        public string? ChargeName { get; set; }
        public string? ChargeCode { get; set; } = string.Empty;
        public string? ChargeType { get; set; } = "Individual";
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public string? ApplicableOn { get; set; }
        public bool? IsActive { get; set; } 
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }

    }
    public class Chargesdto {
        public Guid ChargeId { get; set; }
        public Guid TenantId { get; set; }
        public string? ChargeName { get; set; }
        public string? ChargeCode { get; set; } = string.Empty;
        public string? ChargeType { get; set; } = "Individual";
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public string? ApplicableOn { get; set; }
        public bool IsActive { get; set; } 
        public DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }

}
