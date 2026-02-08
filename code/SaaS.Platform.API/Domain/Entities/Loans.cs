using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    public class Loans : BaseEntity
    {
        public Guid LoanId { get; set; }
        public Guid TenantId { get; set; }
        public Guid AccountId { get; set; }
        public Guid CustomerId { get; set; }
        public string? LoanNumber { get; set; }
        public Guid LoanTypeId { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal EMIAmount { get; set; }
        public DateOnly StartDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public DateTime DisbursmantDate { get; set; }
        public decimal OutstandingPrincipal { get; set; }
        public decimal Outstandinginterest { get; set; }
        public Decimal TotalOutStanding { get; set; }
        public string? LoanStatus { get; set; }
        public string? Purpose { get; set; }
        public string? CollateralDescription { get; set; }
        public Guid ApprovedBy { get; set; }
        public DateOnly ApprovedDate { get; set; }
        public  new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        public new DateTime ModifiedDate { get; set; }
        public  new Guid ModifiedBy { get; set; }
    }
}
