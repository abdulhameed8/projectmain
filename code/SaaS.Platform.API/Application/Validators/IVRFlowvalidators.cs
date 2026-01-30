using FluentValidation;
using SaaS.Platform.API.Application.DTOs.IVRFlows;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateIVRFlowdto with business rules
    /// </summary>
    public class CreateIVRFlowsdtoValidator : AbstractValidator<CreateIVRFlowsdto>
    {
        public CreateIVRFlowsdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.FlowCode)
                .NotEmpty()
                .WithMessage("Flow code is required")
                .MaximumLength(50)
                .WithMessage("Flow code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("Flow code must contain only uppercase letters, numbers, and hyphens");

            RuleFor(x => x.FlowType)
                .NotEmpty()
                .WithMessage("Flow type is required")
                .Must(x => x == "Individual" || x == "Corporate")
                .WithMessage("Flow type must be either 'Individual' or 'Corporate'");



            // Corporate customer validation
            When(x => x.FlowType == "Corporate", () =>
            {
                RuleFor(x => x.FlowName)
                    .NotEmpty()
                    .WithMessage("Flow name is required for corporate IVRFlows")
                    .MaximumLength(200)
                    .WithMessage("Flow name cannot exceed 200 characters");
            });





        }

        /// <summary>
        /// Validator for UpdateIVRFlowdto with business rules
        /// </summary>
        public class UpdateIVRFlowdtoValidator : AbstractValidator<UpdateIVRFlowsdto>
        {
            public UpdateIVRFlowdtoValidator()
            {
                RuleFor(x => x.FlowName)
               .MaximumLength(200)
               .WithMessage("Flow name cannot exceed 200 characters")
               .When(x => !string.IsNullOrWhiteSpace(x.FlowName));
            }
        }
    }
}