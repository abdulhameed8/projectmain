using FluentValidation;
using SaaS.Platform.API.Application.DTOs.CallDispositions;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateCustomerDto with business rules
    /// </summary>
    public class CreateCallDispositiondtoValidator : AbstractValidator<CreateCallDispositiondto>
    {
        public CreateCallDispositiondtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.DispositionCode)
                .NotEmpty()
                .WithMessage("Disposition code is required")
                .MaximumLength(50)
                .WithMessage("Disposition code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("Disposition code must contain only uppercase letters, numbers, and hyphens");

            

            
            

            
        }
    }

    /// <summary>
    /// Validator for UpdateCallDispositiondto with business rules
    /// </summary>
    public class UpdateCallDispositiondtoValidator : AbstractValidator<UpdateCallDispositiondto>
    {
        public UpdateCallDispositiondtoValidator()
        {
            

            RuleFor(x => x.DispositionName)
                .MaximumLength(200)
                .WithMessage("disposition name cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.DispositionName));

            
        }
    }
}