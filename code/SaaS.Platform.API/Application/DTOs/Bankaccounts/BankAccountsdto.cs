namespace SaaS.Platform.API.Application.DTOs.Bankaccounts
{
    public class CreateBankAccountsdto
    {
        public Guid AccountId { get; set; }
        public Guid TenantId { get; set; }
        public string? AccountNumber { get; set; }
        public Guid CustomerId { get; set; }
        public Guid AccountTypeId { get; set; }
        public string? AccountName { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal CurrentBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal HoldAmount { get; set; }
        public DateOnly OpeningDate { get; set; }
        public DateOnly ClosingDate { get; set; }
        public string? AccountStatus { get; set; }
        public Guid BranchId { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MinimumBalance { get; set; }
        public decimal OverdraftLimit { get; set; }
        public bool IsJointAccount { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
       

    }
    public class UpdateBankaccountsdto
    {
        public Guid AccountId { get; set; }
        public Guid TenantId { get; set; }
        public string? AccountNumber { get; set; }
        public Guid CustomerId { get; set; }
        public Guid AccountTypeId { get; set; }
        public string? AccountName { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal CurrentBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal HoldAmount { get; set; }
        public DateOnly OpeningDate { get; set; }
        public DateOnly ClosingDate { get; set; }
        public string? AccountStatus { get; set; }
        public Guid BranchId { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MinimumBalance { get; set; }
        public decimal OverdraftLimit { get; set; }
        public bool IsJointAccount { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
       

    }
    public class BankAccountsdto
    {
        public Guid AccountId { get; set; }
        public Guid TenantId { get; set; }
        public string? AccountNumber { get; set; }
        public Guid CustomerId { get; set; }
        public Guid AccountTypeId { get; set; }
        public string? AccountName { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal CurrentBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal HoldAmount { get; set; }
        public DateOnly OpeningDate { get; set; }
        public DateOnly ClosingDate { get; set; }
        public string? AccountStatus { get; set; }
        public Guid BranchId { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MinimumBalance { get; set; }
        public decimal OverdraftLimit { get; set; }
        public bool IsJointAccount { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
        public  DateTime ModifiedDate { get; set; }
        public  Guid ModifiedBy { get; set; }


    }
}
