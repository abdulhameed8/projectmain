using FluentValidation;
using SaaS.Platform.API.Application.DTOs.NotificationTemplete;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateNotificationTempletesdto with business rules
    /// </summary>
    public class CreateNotificationTempletesdtoValidator : AbstractValidator<CreateNotificationTempletesdto>
    {
        public CreateNotificationTempletesdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.TempleteCode)
                .NotEmpty()
                .WithMessage("Templete code is required")
                .MaximumLength(50)
                .WithMessage("Templete code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("Templete code must contain only uppercase letters, numbers, and hyphens");

            RuleFor(x => x.NotificationType)
                .NotEmpty()
                .WithMessage("Notification type is required")
                .Must(x => x == "Individual" || x == "Corporate")
                .WithMessage("Notification type must be either 'Individual' or 'Corporate'");

            

            // Corporate templetes validation
            When(x => x.NotificationType == "Corporate", () =>
            {
                RuleFor(x => x.TempleteName)
                    .NotEmpty()
                    .WithMessage("Templete name is required for corporate templetes")
                    .MaximumLength(200)
                    .WithMessage("Templete name cannot exceed 200 characters");
            });

           
        }
    }

    /// <summary>
    /// Validator for UpdateNotificationTempletesdto with business rules
    /// </summary>
    public class UpdateNotificationTempletesdtoValidator : AbstractValidator<UpdateNotificationTempletesdto>
    {
        public UpdateNotificationTempletesdtoValidator()
        {
            

            RuleFor(x => x.TempleteName)
                .MaximumLength(200)
                .WithMessage("Templete name cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.TempleteName));

            

            
        }
    }
}