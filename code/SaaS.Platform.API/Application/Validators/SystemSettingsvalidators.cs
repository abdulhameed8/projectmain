using FluentValidation;
using SaaS.Platform.API.Application.DTOs.SystemSettings;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateSysyemSettingsdto with business rules
    /// </summary>
    public class CreateSystemSettingsdtoValidator : AbstractValidator<CreateSystemSettingsdto>
    {
        public CreateSystemSettingsdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");
        }
    }
}