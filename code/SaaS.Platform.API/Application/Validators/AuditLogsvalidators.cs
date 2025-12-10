using FluentValidation;
using SaaS.Platform.API.Application.DTOs.AuditLogs;

namespace SaaS.Platform.API.Application.Validators
{
    public class CreateAuditLogsdtovalidators : AbstractValidator<CreateAuditLogsdto>
    {
        public CreateAuditLogsdtovalidators()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");
        }
    }

    /// <summary>
    /// Validator for UpdateAuditLogsdto with business rules
    /// </summary>
    public class UpdateAuditLogsdtoValidator : AbstractValidator<UpdateAuditLogsdto>
    {
    
        public UpdateAuditLogsdtoValidator()
        {


            RuleFor(x => x.EntityName)
                .MaximumLength(200)
                .WithMessage("Entity name cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.EntityName));




        }
    }
}
