using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Notifications;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateNotificationsdto with business rules
    /// </summary>
    public class CreateNotificationsdtoValidator : AbstractValidator<CreateNotificationsdto>
    {
        public CreateNotificationsdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            
            RuleFor(x => x.NotificationType)
                .NotEmpty()
                .WithMessage("Notification type is required")
                .Must(x => x == "Individual" || x == "Corporate")
                .WithMessage("Notification type must be either 'Individual' or 'Corporate'");



            


        }
    }

    /// <summary>
    /// Validator for UpdateNotificationsdto with business rules
    /// </summary>
    public class UpdateNotificationsdtoValidator : AbstractValidator<UpdateNotificationsdto>
    {
        public UpdateNotificationsdtoValidator()
        {


            RuleFor(x => x.Status)
                .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                .WithMessage(" status must be Active, Inactive, or Blocked")
                .When(x => !string.IsNullOrWhiteSpace(x.Status));




        }
    }
}