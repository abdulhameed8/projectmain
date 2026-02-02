using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class BankAccounts : BaseEntity
    {
        public Guid AccountId { get; set; }
        public Guid TenantId { get; set; }
        public string? AccountNumber { get; set; }
        public Guid CustomerId { get; set; }
        public Guid AccountTypeId { get; set; }
        public string? AccountName { get; set; }
        public string? CurrencyCode { get; set; } = string.Empty;
        public decimal CurrentBalance  { get; set; }
        public decimal AvailableBalance  { get; set; }
        public decimal HoldAmount { get; set; }
        public DateOnly OpeningDate  { get; set; }
        public DateOnly ClosingDate { get; set; }
        public string? AccountStatus { get; set; }
        public Guid BranchId  { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MinimumBalance { get; set; }
        public decimal OverdraftLimit { get; set; }
        public bool IsJointAccount { get; set; } 
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        public new DateTime ModifiedDate { get; set; }
        public new Guid ModifiedBy { get; set; }



    }
}
   