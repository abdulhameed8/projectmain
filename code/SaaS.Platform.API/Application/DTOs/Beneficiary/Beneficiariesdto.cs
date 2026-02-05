namespace SaaS.Platform.API.Application.DTOs
{
    public class CreateBeneficiariesdto
    {
        public Guid BeneficiaryId { get; set; }
        public Guid TenantId { get; set; }
        public Guid CustomerId { get; set; }
        public string? BenificiaryName { get; set; }
        public string? BenificiaryAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string BankCode { get; set; } = string.Empty;
        public string? IFSCCode { get; set; }
        public string? SwiftCode { get; set; }
        public string? BeneficiaryType { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }

    }
    public class UpdateBeneficiariesdto
    {
        public Guid BeneficiaryId { get; set; }
        public Guid TenantId { get; set; }
        public Guid CustomerId { get; set; }
        public string? BenificiaryName { get; set; }
        public string? BenificiaryAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string BankCode { get; set; } = string.Empty;
        public string? IFSCCode { get; set; }
        public string? SwiftCode { get; set; }
        public string? BeneficiaryType { get; set; }
        public bool IsVerified { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }

    }
    public class Beneficiariesdto
    {
        public Guid BeneficiaryId { get; set; }
        public Guid TenantId { get; set; }
        public Guid CustomerId { get; set; }
        public string? BenificiaryName { get; set; }
        public string? BenificiaryAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string BankCode { get; set; } = string.Empty;
        public string? IFSCCode { get; set; }
        public string? SwiftCode { get; set; }
        public string? BeneficiaryType { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid ModifiedBy { get; set; }
    }
}