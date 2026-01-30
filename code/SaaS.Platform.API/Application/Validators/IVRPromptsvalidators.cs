using FluentValidation;
using SaaS.Platform.API.Application.DTOs.IVRPrompts;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateIVRPromptsdto with business rules
    /// </summary>
    public class CreateIVRPRomptsdtoValidator : AbstractValidator<CreateIVRPromptsdto>
    {
        public CreateIVRPRomptsdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            
            RuleFor(x => x.PromptType)
                .NotEmpty()
                .WithMessage("Prompt type is required")
                .Must(x => x == "Individual" || x == "Corporate")
                .WithMessage("Prompt type must be either 'Individual' or 'Corporate'");

           
            // Corporate prompt validation
            When(x => x.PromptType == "Corporate", () =>
            {
                RuleFor(x => x.PromptName)
                    .NotEmpty()
                    .WithMessage("Prompt name is required for corporate prompts")
                    .MaximumLength(200)
                    .WithMessage("Prompt name cannot exceed 200 characters");
            });

            

           
        }
    }

    /// <summary>
    /// Validator for UpdatePromptsdto with business rules
    /// </summary>
    public class UpdatePromptsdtoValidator : AbstractValidator<UpdateIVRPromptsdto>
    {
        public UpdatePromptsdtoValidator()
        {
            RuleFor(x => x.PromptName)
               .MaximumLength(200)
               .WithMessage("Prompt name cannot exceed 200 characters")
               .When(x => !string.IsNullOrWhiteSpace(x.PromptName));

        }
    }
}