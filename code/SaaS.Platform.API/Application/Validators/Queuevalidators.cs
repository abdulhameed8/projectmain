using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Queues;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateQueuedto with business rules
    /// </summary>
    public class CreateQueuedtoValidator : AbstractValidator<CreateQueuedto>
    {
        public CreateQueuedtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.QueueCode)
                .NotEmpty()
                .WithMessage("Queue code is required")
                .MaximumLength(50)
                .WithMessage("Queue code cannot exceed 50 characters")
                .Matches("^[A-Z0-9-]+$")
                .WithMessage("Queue code must contain only uppercase letters, numbers, and hyphens");

           
            
           RuleFor(x => x.MaxQueueSize)
                .GreaterThanOrEqualTo(0)
                .WithMessage("MaxQueueSize cannot be negative");

           
        }
    }

    /// <summary>
    /// Validator for UpdateQueuedto with business rules
    /// </summary>
    public class UpdateQueuedtoValidator : AbstractValidator<UpdateQueuedto>
    {
        public UpdateQueuedtoValidator()
        {
            

            RuleFor(x => x.MaxQueueSize)
                .GreaterThanOrEqualTo(0)
                .WithMessage("MaxQueueSize cannot be negative")
                .When(x => x.MaxQueueSize.HasValue);

            
            
        }
    }
}