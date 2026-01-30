using FluentValidation;
using SaaS.Platform.API.Application.DTOs.IVRMenus;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateIVRMenusto with business rules
    /// </summary>
    public class CreateIVRMenusdtoValidator : AbstractValidator<CreateIVRMenusdto>
    {
        public CreateIVRMenusdtoValidator()
        {
           
            
           

            RuleFor(x => x.MaxRetries)
                .GreaterThanOrEqualTo(0)
                .WithMessage("MaxRetries cannot be negative");

           
        }
    }

    /// <summary>
    /// Validator for UpdateCustomerdto with business rules
    /// </summary>
    public class UpdateIVRMenusdtoValidator : AbstractValidator<UpdateIVRMenusdto>
    {
        public UpdateIVRMenusdtoValidator()
        {
            

            RuleFor(x => x.MenuName)
                .MaximumLength(200)
                .WithMessage("Menu name cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.MenuName));


            
           
        }
    }
}