using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Campaigns;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateCustomerDto with business rules
    /// </summary>
    public class CreateCampaigndtoValidator : AbstractValidator<CreateCampaigndto>
    {
        public CreateCampaigndtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            

            RuleFor(x => x.CampaignType)
                .NotEmpty()
                .WithMessage("Campaign type is required")
                .Must(x => x == "Individual" || x == "Corporate")
                .WithMessage("Campaign type must be either 'Individual' or 'Corporate'");

           

            // Corporate campaign validation
            When(x => x.CampaignType == "Corporate", () =>
            {
                RuleFor(x => x.CampaignName)
                    .NotEmpty()
                    .WithMessage("Campaign name is required for corporate campaign")
                    .MaximumLength(200)
                    .WithMessage("Campaign name cannot exceed 200 characters");
            });

           
            

            
        }
    }

    /// <summary>
    /// Validator for UpdateCampaigndto with business rules
    /// </summary>
    public class UpdateCampaigndtoValidator : AbstractValidator<UpdateCampaigndto>
    {
        public UpdateCampaigndtoValidator()
        {
            

            RuleFor(x => x.CampaignName)
                .MaximumLength(200)
                .WithMessage("Campaign name cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.CampaignName));

            
            
        }
    }
}