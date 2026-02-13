using FluentValidation;
using SaaS.Platform.API.Application.DTOs.EmailConfig;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateEmailconfigto with business rules
    /// </summary>
    public class CreateEmailConfigurationsdtoValidator : AbstractValidator<CreateEmailConfigurationsdto>
    {
        public CreateEmailConfigurationsdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");
        }
    }
}