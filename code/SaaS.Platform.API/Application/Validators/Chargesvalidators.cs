using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Charges;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateChargedto with business rules
    /// </summary>
    public class CreateChargesdtoValidator : AbstractValidator<CreateChargesdto>
    {
        public CreateChargesdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.ChargeCode)
                .NotEmpty()
                .WithMessage("Charge code is required")
                .MaximumLength(50)
                .WithMessage("Charge code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("Charge code must contain only uppercase letters, numbers, and hyphens");

            RuleFor(x => x.ChargeType)
                .NotEmpty()
                .WithMessage("Charge type is required")
                .Must(x => x == "Individual" || x == "Corporate")
                .WithMessage("Charge type must be either 'Individual' or 'Corporate'");

            
            // Corporate charges validation
            When(x => x.ChargeType == "Corporate", () =>
            {
                RuleFor(x => x.ChargeName)
                    .NotEmpty()
                    .WithMessage("Charge name is required for corporate charges")
                    .MaximumLength(200)
                    .WithMessage("Charge name cannot exceed 200 characters");
            });

        }
    }

    /// <summary>
    /// Validator for UpdateCustomerDto with business rules
    /// </summary>
    public class UpdateChargesdtoValidator : AbstractValidator<UpdateChargesdto>
    {
        public UpdateChargesdtoValidator()
        {


            RuleFor(x => x.ChargeName)
                .MaximumLength(200)
                .WithMessage("Charge name cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.ChargeName));

        }
    }
}