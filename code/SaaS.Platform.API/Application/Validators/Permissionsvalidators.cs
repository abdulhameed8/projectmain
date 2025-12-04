using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Permissions;

namespace SaaS.Platform.API.Application.Validators
{
    
    
        /// <summary>
        /// Validator for CreatePermissionsdto with business rules
        /// </summary>
        public class CreatePermissionsdtoValidator : AbstractValidator<CreatePermissionsdto>
        {
            public CreatePermissionsdtoValidator()
            {
               

                RuleFor(x => x.PermissionCode)
                    .NotEmpty()
                    .WithMessage("Permission code is required")
                    .MaximumLength(50)
                    .WithMessage("Permission code cannot exceed 50 characters")
                    .Matches("^[A-Z0-9-]+$")
                    .WithMessage("Permission code must contain only uppercase letters, numbers, and hyphens");

                
            }
        }

        /// <summary>
        /// Validator for UpdateCustomerDto with business rules
        /// </summary>
        public class UpdatePermissionsdtoValidator : AbstractValidator<UpdatePermissionsdto>
        {
            public UpdatePermissionsdtoValidator()
            {
            }
        }
    }


