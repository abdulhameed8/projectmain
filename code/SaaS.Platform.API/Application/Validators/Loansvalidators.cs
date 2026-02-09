using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Loans;

namespace SaaS.Platform.API.Application.Validators
{
   
    
        /// <summary>
        /// Validator for CreateLoansdto with business rules
        /// </summary>
        public class CreateLoansdtoValidator : AbstractValidator<CreateLoansdto>
        {
            public CreateLoansdtoValidator()
            {
                RuleFor(x => x.TenantId)
                    .NotEmpty()
                    .WithMessage("Tenant ID is required");

               
                
                RuleFor(x => x.LoanStatus)
                    .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                    .WithMessage("Loan status must be Active, Inactive, or Blocked");

                
            }
        }

        /// <summary>
        /// Validator for UpdateCustomerDto with business rules
        /// </summary>
        public class UpdateLoansdtoValidator : AbstractValidator<UpdateLoansdto>
        {
            public UpdateLoansdtoValidator()


            { 
                RuleFor(x => x.LoanStatus)
                    .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                    .WithMessage("Loan status must be Active, Inactive, or Blocked")
                    .When(x => !string.IsNullOrWhiteSpace(x.LoanStatus));

                
            }
        }
    }


