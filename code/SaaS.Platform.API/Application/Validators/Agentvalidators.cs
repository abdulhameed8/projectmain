using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Agents;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateAgentdto with business rules
    /// </summary>
    public class CreateAgentdtoValidator : AbstractValidator<CreateAgentQueuesdto>
    {
        public CreateAgentdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.AgentCode)
                .NotEmpty()
                .WithMessage("Agent code is required")
                .MaximumLength(50)
                .WithMessage("Agent code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("Agent code must contain only uppercase letters, numbers, and hyphens");

           

           RuleFor(x => x.AgentStatus)
                .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                .WithMessage("Agent status must be Active, Inactive, or Blocked");

            
        }
    }

    /// <summary>
    /// Validator for UpdateAgentdto with business rules
    /// </summary>
    public class UpdateAgentdtoValidator : AbstractValidator<UpdateAgentQueuesdto>
    {
        public UpdateAgentdtoValidator()
        {
           
            

        }
    }
}