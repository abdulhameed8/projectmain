using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Transaction;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateTransactionsdto with business rules
    /// </summary>
    public class CreateTransactionsdtoValidator : AbstractValidator<CreateTransactionsdto>
    {
        public CreateTransactionsdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

           
        }
    }

    /// <summary>
    /// Validator for UpdateTransactionsdto with business rules
    /// </summary>
    public class UpdateTransactionsdtoValidator : AbstractValidator<UpdateTransactionsdto>
    {
        public UpdateTransactionsdtoValidator()
        {


            


        }
    }
}