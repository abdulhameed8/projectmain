using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Users;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateCustomerDto with business rules
    /// </summary>
    public class CreateUserDtoValidator : AbstractValidator<CreateUserdto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

           

            // Individual User validation
            
            {
                RuleFor(x => x.FirstName)
                    .NotEmpty()
                    .WithMessage("First name is required for individual customers")
                    .MaximumLength(100)
                    .WithMessage("First name cannot exceed 100 characters");

                RuleFor(x => x.LastName)
                    .NotEmpty()
                    .WithMessage("Last name is required for individual customers")
                    .MaximumLength(100)
                    .WithMessage("Last name cannot exceed 100 characters");

                
            };

            // Corporate customer validation
            

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format")
                .MaximumLength(255)
                .WithMessage("Email cannot exceed 255 characters");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            
           
        }
    }

    /// <summary>
    /// Validator for UpdateUserDto with business rules
    /// </summary>
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserdto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(100)
                .WithMessage("First name cannot exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.FirstName));

            RuleFor(x => x.LastName)
                .MaximumLength(100)
                .WithMessage("Last name cannot exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.LastName));

           
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Invalid email format")
                .MaximumLength(255)
                .WithMessage("Email cannot exceed 255 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            
        }
    }
}