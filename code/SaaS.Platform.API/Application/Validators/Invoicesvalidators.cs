using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Invoices;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateInvoicesdto with business rules
    /// </summary>
    public class CreateInvoicesdtoValidator : AbstractValidator<CreateInvoicesdto>
    {
        public CreateInvoicesdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

                
        }
    }

    /// <summary>
    /// Validator for UpdateInvoicesdto with business rules
    /// </summary>
    public class UpdateInvoicesdtoValidator : AbstractValidator<UpdateInvoicesdto>
    {
        public UpdateInvoicesdtoValidator()
        {
             

            RuleFor(x => x.Status)
                .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                .WithMessage(" status must be Active, Inactive, or Blocked")
                .When(x => !string.IsNullOrWhiteSpace(x.Status));

           
        }
    }
}