using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Customer;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateCustomerDto with business rules
    /// </summary>
    public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
    {
        public CreateCustomerDtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.CustomerCode)
                .NotEmpty()
                .WithMessage("Customer code is required")
                .MaximumLength(50)
                .WithMessage("Customer code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("Customer code must contain only uppercase letters, numbers, and hyphens");

            RuleFor(x => x.CustomerType)
                .NotEmpty()
                .WithMessage("Customer type is required")
                .Must(x => x == "Individual" || x == "Corporate")
                .WithMessage("Customer type must be either 'Individual' or 'Corporate'");

            // Individual customer validation
            When(x => x.CustomerType == "Individual", () =>
            {
                RuleFor(x => x.FirstName)
                    .NotEmpty()
                    .WithMessage("First name is required for individual customers")
                    .MaximumLength(100)
                    .WithMessage("First name cannot exceed 100 characters");

                RuleFor(x => x.LastName)
                    .NotEmpty()
                    .WithMessage("Last name is required for individual customers")
                    .MaximumLength(100)
                    .WithMessage("Last name cannot exceed 100 characters");

                RuleFor(x => x.DateOfBirth)
                    .LessThan(DateTime.Today)
                    .WithMessage("Date of birth must be in the past")
                    .When(x => x.DateOfBirth.HasValue);
            });

            // Corporate customer validation
            When(x => x.CustomerType == "Corporate", () =>
            {
                RuleFor(x => x.CompanyName)
                    .NotEmpty()
                    .WithMessage("Company name is required for corporate customers")
                    .MaximumLength(200)
                    .WithMessage("Company name cannot exceed 200 characters");
            });

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format")
                .MaximumLength(255)
                .WithMessage("Email cannot exceed 255 characters");

            RuleFor(x => x.Phone)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            RuleFor(x => x.Mobile)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Invalid mobile number format")
                .When(x => !string.IsNullOrWhiteSpace(x.Mobile));

            RuleFor(x => x.PostalCode)
                .MaximumLength(20)
                .WithMessage("Postal code cannot exceed 20 characters");

            RuleFor(x => x.CustomerStatus)
                .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                .WithMessage("Customer status must be Active, Inactive, or Blocked");

            RuleFor(x => x.CreditLimit)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Credit limit cannot be negative");

            RuleFor(x => x.CreditScore)
                .InclusiveBetween(300, 850)
                .WithMessage("Credit score must be between 300 and 850")
                .When(x => x.CreditScore.HasValue);

            RuleFor(x => x.PreferredLanguage)
                .MaximumLength(10)
                .WithMessage("Preferred language code cannot exceed 10 characters");
        }
    }

    /// <summary>
    /// Validator for UpdateCustomerDto with business rules
    /// </summary>
    public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
    {
        public UpdateCustomerDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(100)
                .WithMessage("First name cannot exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.FirstName));

            RuleFor(x => x.LastName)
                .MaximumLength(100)
                .WithMessage("Last name cannot exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.LastName));

            RuleFor(x => x.CompanyName)
                .MaximumLength(200)
                .WithMessage("Company name cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.CompanyName));

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Invalid email format")
                .MaximumLength(255)
                .WithMessage("Email cannot exceed 255 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Phone)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            RuleFor(x => x.Mobile)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Invalid mobile number format")
                .When(x => !string.IsNullOrWhiteSpace(x.Mobile));

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today)
                .WithMessage("Date of birth must be in the past")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.PostalCode)
                .MaximumLength(20)
                .WithMessage("Postal code cannot exceed 20 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.PostalCode));

            RuleFor(x => x.CustomerStatus)
                .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                .WithMessage("Customer status must be Active, Inactive, or Blocked")
                .When(x => !string.IsNullOrWhiteSpace(x.CustomerStatus));

            RuleFor(x => x.CreditLimit)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Credit limit cannot be negative")
                .When(x => x.CreditLimit.HasValue);

            RuleFor(x => x.CreditScore)
                .InclusiveBetween(300, 850)
                .WithMessage("Credit score must be between 300 and 850")
                .When(x => x.CreditScore.HasValue);

            RuleFor(x => x.PreferredLanguage)
                .MaximumLength(10)
                .WithMessage("Preferred language code cannot exceed 10 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.PreferredLanguage));
        }
    }
}