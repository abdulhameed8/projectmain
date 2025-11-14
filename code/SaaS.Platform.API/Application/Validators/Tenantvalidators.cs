using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Tenant;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateTenantDto with business rules
    /// </summary>
    public class CreateTenantDtoValidator : AbstractValidator<CreateTenantdto>
    {
        public CreateTenantDtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.TenantCode)
                .NotEmpty()
                .WithMessage("Tenant code is required")
                .MaximumLength(50)
                .WithMessage("Tenant code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("Tenant code must contain only uppercase letters, numbers, and hyphens");


           

            

            RuleFor(x => x.ContactEmail)
                .NotEmpty()
                .WithMessage("ContactEmail is required")
                .EmailAddress()
                .WithMessage("Invalid Contactemail format")
                .MaximumLength(255)
                .WithMessage("ContactEmail cannot exceed 255 characters");

            RuleFor(x => x.ContactPhone)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Invalid Contactphone number format")
                .When(x => !string.IsNullOrWhiteSpace(x.ContactPhone));

            RuleFor(x => x.Mobile)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Invalid mobile number format")
                .When(x => !string.IsNullOrWhiteSpace(x.Mobile));

            RuleFor(x => x.PostalCode)
                .MaximumLength(20)
                .WithMessage("Postal code cannot exceed 20 characters");

            RuleFor(x => x.TenantStatus)
                .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                .WithMessage("Tenant status must be Active, Inactive, or Blocked");

           
        }
    }

    /// <summary>
    /// Validator for UpdateTenantDto with business rules
    /// </summary>
    public class UpdateTenantDtoValidator : AbstractValidator<UpdateTenantDto>
    {
        public UpdateTenantDtoValidator()
        {
            RuleFor(x => x.TenantName)
                .MaximumLength(100)
                .WithMessage("Tenantname cannot exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.TenantName));

           

            RuleFor(x => x.ContactEmail)
                .EmailAddress()
                .WithMessage("Invalid Contactemail format")
                .MaximumLength(255)
                .WithMessage("ContactEmail cannot exceed 255 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));

            RuleFor(x => x.ContactPhone)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Invalid Contactphone number format")
                .When(x => !string.IsNullOrWhiteSpace(x.ContactPhone));

            RuleFor(x => x.Mobile)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Invalid mobile number format")
                .When(x => !string.IsNullOrWhiteSpace(x.Mobile));

            
            RuleFor(x => x.PostalCode)
                .MaximumLength(20)
                .WithMessage("Postal code cannot exceed 20 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.PostalCode));

            RuleFor(x => x.TenantStatus)
                .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                .WithMessage("Tenant status must be Active, Inactive, or Blocked")
                .When(x => !string.IsNullOrWhiteSpace(x.TenantStatus));

            
        }
    }
}