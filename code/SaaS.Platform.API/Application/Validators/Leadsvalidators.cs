using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Leads;
namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateLeadsDto with business rules
    /// </summary>
    public class CreateLeadsdtoValidator : AbstractValidator<CreateLeadsdto>
    {
        public CreateLeadsdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.LeadsCode)
                .NotEmpty()
                .WithMessage("Leads code is required")
                .MaximumLength(50)
                .WithMessage("Leads code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("Leads code must contain only uppercase letters, numbers, and hyphens");

            
            // Individual customer validation
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

                
            }

            // Corporate customer validation
            {
                RuleFor(x => x.CompanyName)
                    .NotEmpty()
                    .WithMessage("Company name is required for corporate leads")
                    .MaximumLength(200)
                    .WithMessage("Company name cannot exceed 200 characters");
            }

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


           
        }
    }

    /// <summary>
    /// Validator for UpdateLeadsdto with business rules
    /// </summary>
    public class UpdateLeadsdtoValidator : AbstractValidator<UpdateLeadsdto>
    {
        public UpdateLeadsdtoValidator()
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

            
           
        }
    }
}