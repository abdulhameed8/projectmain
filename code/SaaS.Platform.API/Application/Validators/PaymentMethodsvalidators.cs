using FluentValidation;
using SaaS.Platform.API.Application.DTOs;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateCustomerDto with business rules
    /// </summary>
    public class CreatePaymentMethodsdtoValidator : AbstractValidator<CreatePaymentMethodsdto>
    {
        public CreatePaymentMethodsdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            
            RuleFor(x => x.MethodType)
                .NotEmpty()
                .WithMessage("Method type is required")
                .Must(x => x == "Individual" || x == "Corporate")
                .WithMessage("Method type must be either 'Individual' or 'Corporate'");

            
        }
    }

    /// <summary>
    /// Validator for UpdatePaymentMethodto with business rules
    /// </summary>
    public class UpdatePaymentMethodsdtoValidator : AbstractValidator<UpdatePaymentMethodsdto>
    {
        public UpdatePaymentMethodsdtoValidator()
        {
            
          
        }
    }
}
