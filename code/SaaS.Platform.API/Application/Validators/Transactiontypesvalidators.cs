using FluentValidation;
using SaaS.Platform.API.Application.DTOs.TransactionTypes;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateTransactionTypesdto with business rules
    /// </summary>
    public class CreateTransactionTypesdtoValidator : AbstractValidator<CreateTransactionTypesdto>
    {
        public CreateTransactionTypesdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.TypeCode)
                .NotEmpty()
                .WithMessage("Type code is required")
                .MaximumLength(50)
                .WithMessage("Type code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("Type code must contain only uppercase letters, numbers, and hyphens");

                  
        }
    }

    /// <summary>
    /// Validator for UpdateTransactionTypesdto with business rules
    /// </summary>
    public class UpdateTransactionTypesdtoValidator : AbstractValidator<UpdateTransactionTypesdto>
    {
        public UpdateTransactionTypesdtoValidator()
        {
            

            RuleFor(x => x.TypeName)
                .MaximumLength(200)
                .WithMessage("Type name cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.TypeName));


           
        }
    }
}