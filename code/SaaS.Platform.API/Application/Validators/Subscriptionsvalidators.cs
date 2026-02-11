using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Subscriptions;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateSubscriptionsdto with business rules
    /// </summary>
    public class CreateSubscriptionsdtoValidator : AbstractValidator<CreateSubscriptionsdto>
    {
        public CreateSubscriptionsdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");



           RuleFor(x => x.Status)
                .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                .WithMessage(" status must be Active, Inactive, or Blocked");


        }
    }

    /// <summary>
    /// Validator for UpdateSubscriptiondto with business rules
    /// </summary>
    public class UpdateSubscriptionsdtoValidator : AbstractValidator<UpdateSubscriptionsdto>
    {
        public UpdateSubscriptionsdtoValidator()
        {


            

            RuleFor(x => x.Status)
                .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                .WithMessage(" status must be Active, Inactive, or Blocked")
                .When(x => !string.IsNullOrWhiteSpace(x.Status));


        }
    }
}