using FluentValidation;
using SaaS.Platform.API.Application.DTOs;
using SaaS.Platform.API.Application.DTOs.Payment;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateCustomerDto with business rules
    /// </summary>
    public class CreatePaymentsdtoValidator : AbstractValidator<CreatePaymentsdto>
    {
        public CreatePaymentsdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");


            


        }
    }

    /// <summary>
    /// Validator for UpdatePaymentMethodto with business rules
    /// </summary>
    public class UpdatePaymentsdtoValidator : AbstractValidator<UpdatePaymentsdto>
    {
        public UpdatePaymentsdtoValidator()
        {

            RuleFor(x => x.Status)
               .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
               .WithMessage(" status must be Active, Inactive, or Blocked")
               .When(x => !string.IsNullOrWhiteSpace(x.Status));
        }
    }
}
