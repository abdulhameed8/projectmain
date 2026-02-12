using FluentValidation;
using SaaS.Platform.API.Application.DTOs.WorkflowDefinitions;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateWorkflowDefinitionsdto with business rules
    /// </summary>
    public class CreateWorkflowDefinitionsdtoValidator : AbstractValidator<CreateWorkflowDefinitionsdto>
    {
        public CreateWorkflowDefinitionsdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");
            
           
            
        }
    }

    /// <summary>
    /// Validator for UpdateWorkflowDefinitionsdto with business rules
    /// </summary>
    public class UpdateWorkflowDefinitiondtoValidator : AbstractValidator<UpdateWorkflowDefinitionsdto>
    {
        public UpdateWorkflowDefinitiondtoValidator()
        {
            

            RuleFor(x => x.WorkflowName)
                .MaximumLength(200)
                .WithMessage("WorkflowDefinition name cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.WorkflowName));

            
            
        }
    }
}