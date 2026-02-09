using FluentValidation;
using SaaS.Platform.API.Application.DTOs.LoanTypes;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateLoanTypedto with business rules
    /// </summary>
    public class CreateLoanTypesdtoValidator : AbstractValidator<CreateLoanTypesdto>
    {
        public CreateLoanTypesdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.LoanTypeCode)
                .NotEmpty()
                .WithMessage("LoanType code is required")
                .MaximumLength(50)
                .WithMessage("LoanType code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("loanType code must contain only uppercase letters, numbers, and hyphens");

           

            
        }
    }

    /// <summary>
    /// Validator for UpdateLoanTypedto with business rules
    /// </summary>
    public class UpdateLoanTypesdtoValidator : AbstractValidator<UpdateLoanTypesdto>
    {
        public UpdateLoanTypesdtoValidator()
        {


            RuleFor(x => x.LoanTypeName)
                .MaximumLength(200)
                .WithMessage("LoanType name cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.LoanTypeName));

        }
    }
}