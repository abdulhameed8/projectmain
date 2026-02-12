using FluentValidation;
using SaaS.Platform.API.Application.DTOs.WorkflowTask;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateWorkflowInstancesdto with business rules
    /// </summary>
    public class CreateWorkflowTaskdtoValidator : AbstractValidator<CreateWorkflowTaskdto>
    {
        public CreateWorkflowTaskdtoValidator()
        {
           


        }
    }

    /// <summary>
    /// Validator for UpdateWorkflowTaskdto with business rules
    /// </summary>
    public class UpdateWorkflowTaskdtoValidator : AbstractValidator<UpdateWorkflowTaskdto>
    {
        public UpdateWorkflowTaskdtoValidator()
        {

            RuleFor(x => x.TaskName)
               .MaximumLength(200)
               .WithMessage("Task name cannot exceed 200 characters")
               .When(x => !string.IsNullOrWhiteSpace(x.TaskName));




        }
    }
}