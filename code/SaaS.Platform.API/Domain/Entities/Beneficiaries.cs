using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Beneficiaries : BaseEntity
    {
        public Guid BeneficiaryId { get; set; }
        public Guid TenantId { get; set; }
        public Guid CustomerId { get; set; }
        public string? BeneficiaryName { get; set; }
        public string? BeneficiaryAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string BankCode { get; set; } = string.Empty;
        public string? IFSCCode { get; set; }
        public string? SwiftCode { get; set; }
        public string? BeneficiaryType { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; } = true;
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        public new DateTime ModifiedDate { get; set; }
        public new Guid ModifiedBy { get; set; }



    }
}

