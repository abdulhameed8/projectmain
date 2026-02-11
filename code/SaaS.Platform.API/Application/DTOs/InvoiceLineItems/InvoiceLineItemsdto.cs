namespace SaaS.Platform.API.Application.DTOs.InvoiceLineItems
{
    public class CreateInvoiceLineItemsdto
    {
        public Guid LineItemId { get; set; }
        public Guid InvoiceId { get; set; }
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineTotal { get; set; }
    }
    public class UpdateInvoiceLineItemsdto
    {
        public Guid LineItemId { get; set; }
        public Guid InvoiceId { get; set; }
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineTotal { get; set; }
    }
    public class InvoiceLineItemsdto
    {
        public Guid LineItemId { get; set; }
        public Guid InvoiceId { get; set; }
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineTotal { get; set; }
    }
}
