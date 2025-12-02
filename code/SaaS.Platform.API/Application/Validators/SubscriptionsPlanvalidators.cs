using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Customer;
using SaaS.Platform.API.Application.DTOs.SubscriptionsPlan;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateSubscriptionsPlandto with business rules
    /// </summary>
    public class CreateSubcriptionsPlandtoValidator : AbstractValidator<CreateSubscriptionsPlandto>
    {
        public CreateSubcriptionsPlandtoValidator()
        {


            RuleFor(x => x.PlanCode)
                .NotEmpty()
                .WithMessage("Plan code is required")
                .MaximumLength(50)
                .WithMessage("Plan code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("Plan code must contain only uppercase letters, numbers, and hyphens");


            // Corporate customer validation

            {
            }
            ;



        }
    }

    /// <summary>
    /// Validator for SubscriptionsPlanDto with business rules
    /// </summary>
    public class UpdateSubscriptionsPlandtoValidator : AbstractValidator<UpdateSubscriptionsPlandto>
    {
        public UpdateSubscriptionsPlandtoValidator()
        {


            RuleFor(x => x.PlanName)
                .MaximumLength(200)
                .WithMessage("Plan name cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.PlanName));
        }
    }
}