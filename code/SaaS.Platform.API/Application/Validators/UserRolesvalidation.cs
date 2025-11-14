using FluentValidation;
using SaaS.Platform.API.Application.DTOs.UserRoles;


namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateCustomerDto with business rules
    /// </summary>
    public class CreateUserRolesdtoValidator : AbstractValidator<CreateUserRolesdto>
    {
        public CreateUserRolesdtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required");
        }
        
    }
    }

    
      