namespace SaaS.Platform.API.Application.DTOs.Cards
{
    public class CreateCardsdto
    {
        public Guid CardId { get; set; }
        public Guid TenantId { get; set; }
        public Guid AccountId { get; set; }
        public Guid CustomerId { get; set; }
        public string? CardNumber { get; set; }
        public string? CardType { get; set; }
        public string? CardNetwork { get; set; }
        public string? NameOnCard { get; set; }
        public DateOnly IssueDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public string? CVV { get; set; }
        public string? CardStatus { get; set; }
        public decimal DailyLimit { get; set; }
        public decimal MonthlyLimit { get; set; }
        public bool IsPinSet { get; set; }
        public bool IsContactLess { get; set; }
        public DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
       

    }
    public class UpdateCardsdto
    {
        public Guid CardId { get; set; }
        public Guid TenantId { get; set; }
        public Guid AccountId { get; set; }
        public Guid CustomerId { get; set; }
        public string? CardNumber { get; set; }
        public string? CardType { get; set; }
        public string? CardNetwork { get; set; }
        public string? NameOnCard { get; set; }
        public DateOnly IssueDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public string? CVV { get; set; }
        public string? CardStatus { get; set; }
        public decimal DailyLimit { get; set; }
        public decimal MonthlyLimit { get; set; }
        public bool IsPinSet { get; set; }
        public bool IsContactLess { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  Guid? CreatedBy { get; set; }
       
    }
    public class Cardsdto
    {
        public Guid CardId { get; set; }
        public Guid TenantId { get; set; }
        public Guid AccountId { get; set; }
        public Guid CustomerId { get; set; }
        public string? CardNumber { get; set; }
        public string? CardType { get; set; }
        public string? CardNetwork { get; set; }
        public string? NameOnCard { get; set; }
        public DateOnly IssueDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public string? CVV { get; set; }
        public string? CardStatus { get; set; }
        public decimal DailyLimit { get; set; }
        public decimal MonthlyLimit { get; set; }
        public bool IsPinSet { get; set; }
        public bool IsContactLess { get; set; }
        public new DateTime CreatedDate { get; set; }
        public new Guid? CreatedBy { get; set; }
        public new DateTime ModifiedDate { get; set; }
        public new Guid ModifiedBy { get; set; }

    }
}
