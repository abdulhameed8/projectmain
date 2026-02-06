namespace SaaS.Platform.API.Application.DTOs.Transaction
{
    public class CreateTransactionsdto
    {
        public Guid TransactionId { get; set; }
        public Guid TenantId { get; set; }
        public string? TransactionNumber { get; set; }
        public Guid AccountId { get; set; }
        public Guid TransactionTypeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime ValueDate { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionDirection { get; set; }
        public decimal BalanceAfter { get; set; }
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
        public Guid RelatedTransactionId { get; set; }
        public Guid ToAccountId { get; set; }
        public Guid BeneficiaryId { get; set; }
        public string? ChannelType { get; set; }
        public string TransactionStatus { get; set; } = "Active"; // Active, Inactive, Blocked
        public Guid? IsReversed { get; set; }
        public string? ReversalReason { get; set; }
        public string? AuthorizationCode { get; set; }
        public Guid ProcessedBy { get; set; }
        public DateTime createdDate { get; set; }

    }
    public class UpdateTransactionsdto
    {
        public Guid TransactionId { get; set; }
        public Guid TenantId { get; set; }
        public string? TransactionNumber { get; set; }
        public Guid AccountId { get; set; }
        public Guid TransactionTypeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime ValueDate { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionDirection { get; set; }
        public decimal BalanceAfter { get; set; }
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
        public Guid RelatedTransactionId { get; set; }
        public Guid ToAccountId { get; set; }
        public Guid BeneficiaryId { get; set; }
        public string? ChannelType { get; set; }
        public string TransactionStatus { get; set; } = "Active"; // Active, Inactive, Blocked
        public Guid? IsReversed { get; set; }
        public string? ReversalReason { get; set; }
        public string? AuthorizationCode { get; set; }
        public Guid ProcessedBy { get; set; }
        public DateTime createdDate { get; set; }

    }
    public class Transactionsdto
    {
        public Guid TransactionId { get; set; }
        public Guid TenantId { get; set; }
        public string? TransactionNumber { get; set; }
        public Guid AccountId { get; set; }
        public Guid TransactionTypeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime ValueDate { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionDirection { get; set; }
        public decimal BalanceAfter { get; set; }
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
        public Guid RelatedTransactionId { get; set; }
        public Guid ToAccountId { get; set; }
        public Guid BeneficiaryId { get; set; }
        public string? ChannelType { get; set; }
        public string TransactionStatus { get; set; } = "Active"; // Active, Inactive, Blocked
        public Guid? IsReversed { get; set; }
        public string? ReversalReason { get; set; }
        public string? AuthorizationCode { get; set; }
        public Guid ProcessedBy { get; set; }
        public DateTime createdDate { get; set; }

    }
}
