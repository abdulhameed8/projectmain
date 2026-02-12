using FluentValidation;
using SaaS.Platform.API.Application.DTOs.WorkflowDefinitions;
using SaaS.Platform.API.Application.DTOs.WorkflowInstances;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateWorkflowInstancesdto with business rules
    /// </summary>
    public class CreateWorkflowInstancesdtoValidator : AbstractValidator<CreateWorkflowInstancesdto>
    {
        public CreateWorkflowInstancesdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");



        }
    }

    /// <summary>
    /// Validator for UpdateWorkflowDefinitionsdto with business rules
    /// </summary>
    public class UpdateWorkflowInstancesdtoValidator : AbstractValidator<UpdateWorkflowInstancesdto>
    {
        public UpdateWorkflowInstancesdtoValidator()
        {


            


        }
    }
}