namespace SaaS.Platform.API.Application.DTOs.AccountTypes
{
    public class CreateAccountTypesdto
    {
        public Guid AccountTypeId { get; set; }
        public Guid TenantId { get; set; }
        public string? AccountTypeName { get; set; }
        public string? AccountTypeCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal MinimumBalance { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MonthlyFee { get; set; }
        public bool AllowsOverdraft { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
    public class UpdateAccountTypesdto
    {
        public Guid AccountTypeId { get; set; }
        public Guid TenantId { get; set; }
        public string? AccountTypeName { get; set; }
        public string? AccountTypeCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal MinimumBalance { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MonthlyFee { get; set; }
        public bool AllowsOverdraft { get; set; }
        public bool? IsActive { get; set; } 
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
    }
    public class AccountTypesdto
    {
        public Guid AccountTypeId { get; set; }
        public Guid TenantId { get; set; }
        public string? AccountTypeName { get; set; }
        public string? AccountTypeCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal MinimumBalance { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MonthlyFee { get; set; }
        public bool AllowsOverdraft { get; set; }
        public bool IsActive { get; set; } 
        public  DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
