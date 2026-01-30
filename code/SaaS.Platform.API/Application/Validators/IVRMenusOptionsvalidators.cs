using FluentValidation;
using SaaS.Platform.API.Application.DTOs.IVRMenusOptions;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateIVRMenusOptionsdto with business rules
    /// </summary>
    public class CreateIVRMenusOptionsdtoValidator : AbstractValidator<CreateIVRMenusOptionsdto>
    {
        public CreateIVRMenusOptionsdtoValidator()
        {
            
            RuleFor(x => x.ActionType)
                .NotEmpty()
                .WithMessage("Action type is required")
                .Must(x => x == "Individual" || x == "Corporate")
                .WithMessage("Action type must be either 'Individual' or 'Corporate'");

            

        }
    }

    /// <summary>
    /// Validator for UpdateCustomerDto with business rules
    /// </summary>
    public class UpdateIVRMenusOptionsdtoValidator : AbstractValidator<UpdateIVRMenusOptionsdto>
    {
        public UpdateIVRMenusOptionsdtoValidator()
        {
           

          
        }
    }
}
