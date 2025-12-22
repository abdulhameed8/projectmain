using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Notes;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateCustomerDto with business rules
    /// </summary>
    public class CreateNotedtoValidator : AbstractValidator<CreateNotedto>
    {
        public CreateNotedtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");



        }



        /// <summary>
        /// Validator for UpdateNotedto with business rules
        /// </summary>
        public class UpdateNotedtoValidator : AbstractValidator<UpdateNotedto>
        {
            public UpdateNotedtoValidator()
            {




            }
        }
    }
}