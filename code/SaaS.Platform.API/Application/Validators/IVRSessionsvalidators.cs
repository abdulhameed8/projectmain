using FluentValidation;
using SaaS.Platform.API.Application.DTOs.IVRSessions;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateIVRSessionsdto with business rules
    /// </summary>
    public class CreateIVRSessionsdtoValidator : AbstractValidator<CreateIVRSessionsdto>
    {
        public CreateIVRSessionsdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

           
            
            
            
        }
    }

    /// <summary>
    /// Validator for UpdateCustomerDto with business rules
    /// </summary>
    public class UpdateIVRSessionsdtoValidator : AbstractValidator<UpdateIVRSessionsdto>
    {
        public UpdateIVRSessionsdtoValidator()
        {
            

           
        }
    }
}