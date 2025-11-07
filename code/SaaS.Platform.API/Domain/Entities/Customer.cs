using SaaS.Platform.API.Domain.Common;

namespace SaaS.Platform.API.Domain.Entities
{
    /// <summary>
    /// Customer entity representing customers/contacts in the CRM module
    /// </summary>
    public class Customer : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Guid TenantId { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string CustomerType { get; set; } = "Individual"; // Individual, Corporate
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? TaxId { get; set; }
        public string? CustomerSegment { get; set; }
        public string CustomerStatus { get; set; } = "Active"; // Active, Inactive, Blocked, Others
        public Guid? AssignedUserId { get; set; }
        public string? CustomerSource { get; set; }
        public string? Tags { get; set; }
        public string PreferredLanguage { get; set; } = "en";
        public string? PreferredContactMethod { get; set; }
        public decimal CreditLimit { get; set; } = 0;
        public int? CreditScore { get; set; }
        public bool IsActive { get; set; } = true;

        // Computed property for full name
        public string FullName => CustomerType == "Individual"
            ? $"{FirstName} {LastName}".Trim()
            : CompanyName ?? string.Empty;
    }
}