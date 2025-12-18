using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Activities;
using SaaS.Platform.API.Application.DTOs.Customer;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateActivitydto with business rules
    /// </summary>
    public class CreateActivitydtoValidator : AbstractValidator<CreateActivitydto>
    {
        public CreateActivitydtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");


            RuleFor(x => x.ActivityType)
                .NotEmpty()
                .WithMessage("Activity type is required")
                .Must(x => x == "Individual" || x == "Corporate")
                .WithMessage("Activity type must be either 'Individual' or 'Corporate'");



            // Corporate activity validation
            When(x => x.ActivityType == "Corporate", () =>
            {

            });


            RuleFor(x => x.ActivityStatus)
                .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                .WithMessage("Activity status must be Active, Inactive, or Blocked");


        }
    }

    /// <summary>
    /// Validator for UpdateActivitydto with business rules
    /// </summary>
    public class UpdateActivitydtoValidator : AbstractValidator<UpdateActivitydto>
    {
        public UpdateActivitydtoValidator()
        {
            

           
            RuleFor(x => x.ActivityStatus)
                .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                .WithMessage("Activity status must be Active, Inactive, or Blocked")
                .When(x => !string.IsNullOrWhiteSpace(x.ActivityStatus));


        }
    }
}