namespace SaaS.Platform.API.Application.DTOs.LoanTypes
{
    public class CreateLoanTypesdto
    {
        public Guid LoanTypeId { get; set; }
        public Guid TenantId { get; set; }
        public string? LoanTypeName { get; set; }
        public string? LoanTypeCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public int MinTenureAmount { get; set; }
        public int MaxTenureAmount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal ProcessingFeePercentage { get; set; }
        public bool RequiresCollateral { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
    public class UpdateLoanTypesdto
    {
        public Guid LoanTypeId { get; set; }
        public Guid TenantId { get; set; }
        public string? LoanTypeName { get; set; }
        public string? LoanTypeCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public int MinTenureAmount { get; set; }
        public int MaxTenureAmount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal ProcessingFeePercentage { get; set; }
        public bool RequiresCollateral { get; set; }
        public bool? IsActive { get; set; } 
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
    public class LoanTypesdto
    {
        public Guid LoanTypeId { get; set; }
        public Guid TenantId { get; set; }
        public string? LoanTypeName { get; set; }
        public string? LoanTypeCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public int MinTenureAmount { get; set; }
        public int MaxTenureAmount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal ProcessingFeePercentage { get; set; }
        public bool RequiresCollateral { get; set; }
        public bool IsActive { get; set; } 
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
}
