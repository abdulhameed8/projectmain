namespace SaaS.Platform.API.Application.DTOs.Customer
{
    /// <summary>
    /// DTO for creating a new customer
    /// </summary>
    public class CreateCustomerDto
    {
        public Guid TenantId { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string CustomerType { get; set; } = "Individual";
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CompanyName { get; set; }
        public string Email { get; set; } = string.Empty;
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
        public string CustomerStatus { get; set; } = "Active";
        public Guid? AssignedUserId { get; set; }
        public string? CustomerSource { get; set; }
        public string? Tags { get; set; }
        public string PreferredLanguage { get; set; } = "en";
        public string? PreferredContactMethod { get; set; }
        public decimal CreditLimit { get; set; } = 0;
        public int? CreditScore { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing customer
    /// </summary>
    public class UpdateCustomerDto
    {
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
        public string? CustomerStatus { get; set; }
        public Guid? AssignedUserId { get; set; }
        public string? CustomerSource { get; set; }
        public string? Tags { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? PreferredContactMethod { get; set; }
        public decimal? CreditLimit { get; set; }
        public int? CreditScore { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// DTO for customer response
    /// </summary>
    public class CustomerDto
    {
        public Guid CustomerId { get; set; }
        public Guid TenantId { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string CustomerType { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CompanyName { get; set; }
        public string FullName { get; set; } = string.Empty;
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
        public string CustomerStatus { get; set; } = string.Empty;
        public Guid? AssignedUserId { get; set; }
        public string? CustomerSource { get; set; }
        public string? Tags { get; set; }
        public string PreferredLanguage { get; set; } = string.Empty;
        public string? PreferredContactMethod { get; set; }
        public decimal CreditLimit { get; set; }
        public int? CreditScore { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}