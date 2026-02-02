using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Bankaccounts;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateBankAccountdto with business rules
    /// </summary>
    public class CreateBankAccountsdtoValidator : AbstractValidator<CreateBankAccountsdto>
    {
        public CreateBankAccountsdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");



            RuleFor(x => x.AccountStatus)
                .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                .WithMessage("Account status must be Active, Inactive, or Blocked");


        }

        /// <summary>
        /// Validator for UpdateBankAccountsdto with business rules
        /// </summary>
        public class UpdateBankAcountsdtoValidator : AbstractValidator<UpdateBankaccountsdto>
        {
            public UpdateBankAcountsdtoValidator()
            {


                RuleFor(x => x.AccountName)
                    .MaximumLength(200)
                    .WithMessage("Account name cannot exceed 200 characters")
                    .When(x => !string.IsNullOrWhiteSpace(x.AccountName));



                RuleFor(x => x.AccountStatus)
                    .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                    .WithMessage("Account status must be Active, Inactive, or Blocked")
                    .When(x => !string.IsNullOrWhiteSpace(x.AccountStatus));


            }
        }
    }
}