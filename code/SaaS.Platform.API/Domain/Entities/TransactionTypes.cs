using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class TransactionTypes : BaseEntity
    {
        public Guid TransactionId { get; set; }
        public Guid TenantId { get; set; }
        public string? TypeName { get; set; }
        public string? TypeCode { get; set; } = string.Empty; 
        public bool IsDebit { get; set; }
        public bool RequiresApproval { get; set; }
        public Guid ChargeTypeId { get; set; }
        public bool IsActive { get; set; } = true;
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }

    }
}