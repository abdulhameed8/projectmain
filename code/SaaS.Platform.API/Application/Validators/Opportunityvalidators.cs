using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Opportunities;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateOpportunitydto with business rules
    /// </summary>
    public class CreateOpportunitydtoValidator : AbstractValidator<CreateOpportunitydto>
    {
        public CreateOpportunitydtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.OpportunityCode)
                .NotEmpty()
                .WithMessage("Opportunity code is required")
                .MaximumLength(50)
                .WithMessage("Opportunity code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("Opportunty code must contain only uppercase letters, numbers, and hyphens");

            

            // Individual  validation
           


            {
                RuleFor(x => x.OpportunityName)
                    .NotEmpty()
                    .WithMessage("Opportunity name is required for corporate opprortunty")
                    .MaximumLength(200)
                    .WithMessage("Opportunity name cannot exceed 200 characters");
            }

        } 
    }

    /// <summary>
    /// Validator for UpdateOpportunitydto with business rules
    /// </summary>
    public class UpdateOpportunitydtoValidator : AbstractValidator<UpdateOpportunitydto>
    {
        public UpdateOpportunitydtoValidator()
        {
           
            RuleFor(x => x.OpportunityName)
                .MaximumLength(200)
                .WithMessage("Company name cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.OpportunityName));

           
            
        }
    }
}