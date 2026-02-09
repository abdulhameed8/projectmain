using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class LoanRepayments : BaseEntity
    {
        public Guid RepaymentId { get; set; }
        public Guid LoanId { get; set; }
        public int EMINumber { get; set; }
        public DateOnly DueDate { get; set; }
        public DateTime PaidDate { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal LateFee { get; set; }
        public string? PaymentStatus { get; set; }
        public Guid TransactionId { get; set; } 
        public new DateTime CreatedDate { get; set; }
    }
}
