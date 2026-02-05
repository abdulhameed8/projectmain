namespace SaaS.Platform.API.Application.DTOs.TransactionTypes
{
    public class CreateTransactionTypesdto
    {
        public Guid TransactionId { get; set; }
        public Guid TenantId { get; set; }
        public string? TypeName { get; set; }
        public string? TypeCode { get; set; } = string.Empty;
        public bool IsDebit { get; set; }
        public bool RequiresApproval { get; set; }
        public Guid ChargeTypeId { get; set; }
        public bool IsActive { get; set; } = true;
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }

    }
    public class UpdateTransactionTypesdto
    {
        public Guid TransactionId { get; set; }
        public Guid TenantId { get; set; }
        public string? TypeName { get; set; }
        public string? TypeCode { get; set; } = string.Empty;
        public bool IsDebit { get; set; }
        public bool RequiresApproval { get; set; }
        public Guid ChargeTypeId { get; set; }
        public bool? IsActive { get; set; } 
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }

    }
    public class TransactionTypesdto
    {
        public Guid TransactionId { get; set; }
        public Guid TenantId { get; set; }
        public string? TypeName { get; set; }
        public string? TypeCode { get; set; } = string.Empty;
        public bool IsDebit { get; set; }
        public bool RequiresApproval { get; set; }
        public Guid ChargeTypeId { get; set; }
        public bool IsActive { get; set; } 
        public DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }

    }
}
