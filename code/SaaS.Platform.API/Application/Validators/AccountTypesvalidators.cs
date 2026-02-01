using FluentValidation;
using SaaS.Platform.API.Application.DTOs.AccountTypes;
using SaaS.Platform.API.Application.DTOs.Customer;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateAccountTypesdto with business rules
    /// </summary>
    public class CreateAccountTypesdtoValidator : AbstractValidator<CreateAccountTypesdto>
    {
        public CreateAccountTypesdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.AccountTypeCode)
                .NotEmpty()
                .WithMessage("AccountType code is required")
                .MaximumLength(50)
                .WithMessage("AccountType code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("AccountType code must contain only uppercase letters, numbers, and hyphens");

           
            
        }
    }

    /// <summary>
    /// Validator for UpdateAccountTypesdto with business rules
    /// </summary>
    public class UpdateAccountTypesdtoValidator : AbstractValidator<UpdateAccountTypesdto>
    {
        public UpdateAccountTypesdtoValidator()
        {


            RuleFor(x => x.AccountTypeName)
                  .MaximumLength(200)
                  .WithMessage("accountType name cannot exceed 200 characters")
                  .When(x => !string.IsNullOrWhiteSpace(x.AccountTypeName));




        }
    }
}