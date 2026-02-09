namespace SaaS.Platform.API.Application.DTOs.LoanRepayments
{
    public class CreateLoanRepaymentsdto
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
        public  DateTime CreatedDate { get; set; }
    }
    public class UpdateLoanRepaymentsdto
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
        public  DateTime CreatedDate { get; set; }
    }
    public class LoanRepaymentsdto
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
        public  DateTime CreatedDate { get; set; }
    }
}
