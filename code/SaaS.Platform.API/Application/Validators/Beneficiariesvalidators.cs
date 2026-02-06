using FluentValidation;
using SaaS.Platform.API.Application.DTOs;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateBeneficiariesdto with business rules
    /// </summary>
    public class CreateBeneficiariesdtoValidator : AbstractValidator<CreateBeneficiariesdto>
    {
        public CreateBeneficiariesdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.BankCode)
                .NotEmpty()
                .WithMessage("Bank code is required")
                .MaximumLength(50)
                .WithMessage("Bank code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("Bank code must contain only uppercase letters, numbers, and hyphens");

            RuleFor(x => x.BeneficiaryType)
                .NotEmpty()
                .WithMessage("Beneficiary type is required")
                .Must(x => x == "Individual" || x == "Corporate")
                .WithMessage("Beneficiary type must be either 'Individual' or 'Corporate'");

            

            // Corporate customer validation
            When(x => x.BeneficiaryType == "Corporate", () =>
            {
                RuleFor(x => x.BeneficiaryName)
                    .NotEmpty()
                    .WithMessage("Beneficiary name is required for corporate customers")
                    .MaximumLength(200)
                    .WithMessage("Beneficiary name cannot exceed 200 characters");
            });

           
        }
    }

    /// <summary>
    /// Validator for UpdateCustomerDto with business rules
    /// </summary>
    public class UpdateBeneficiariesdtoValidator : AbstractValidator<UpdateBeneficiariesdto>
    {
        public UpdateBeneficiariesdtoValidator()
        {
           

            RuleFor(x => x.BeneficiaryName)
                .MaximumLength(200)
                .WithMessage("Beneficiary name cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.BeneficiaryName));

            
        }
    }
}