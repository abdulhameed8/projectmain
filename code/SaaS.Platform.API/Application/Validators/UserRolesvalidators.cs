using FluentValidation;
using SaaS.Platform.API.Application.DTOs.UserRoles;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateUserRolesDto with business rules
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

    /// <summary>
    /// Validator for UpdateUserRolesDto with business rules
    /// </summary>
    public class UpdateUserRolesDtoValidator : AbstractValidator<UpdateUserRolesdto>
    {
        public UpdateUserRolesDtoValidator()
        {
            


        }
    }
}