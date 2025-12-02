using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Roles;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateRolesdto with business rules
    /// </summary>
    public class CreateRolesdtoValidator : AbstractValidator<CreateRolesdto>
    {
        public CreateRolesdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");
        }
    }
}