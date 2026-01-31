using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Branches;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateBranchesdto with business rules
    /// </summary>
    public class CreateBranchesdtoValidator : AbstractValidator<CreateBranchesdto>
    {
        public CreateBranchesdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.BranchCode)
                .NotEmpty()
                .WithMessage("Branch code is required")
                .MaximumLength(50)
                .WithMessage("Branch code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("Branch code must contain only uppercase letters, numbers, and hyphens");
           

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


            RuleFor(x => x.PostalCode)
                .MaximumLength(20)
                .WithMessage("Postal code cannot exceed 20 characters");

           
        }
    }

    /// <summary>
    /// Validator for UpdateBranchesdto with business rules
    /// </summary>
    public class UpdateBranchesdtoValidator : AbstractValidator<UpdateBranchesdto>
    {
        public UpdateBranchesdtoValidator()
        {
            
            RuleFor(x => x.BranchName)
                .MaximumLength(200)
                .WithMessage("Branche name cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.BranchName));

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

           
            RuleFor(x => x.PostalCode)
                .MaximumLength(20)
                .WithMessage("Postal code cannot exceed 20 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.PostalCode));

            
        }
    }
}