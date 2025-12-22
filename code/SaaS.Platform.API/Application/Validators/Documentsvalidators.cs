using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Customer;
using SaaS.Platform.API.Application.DTOs.Documents;

namespace SaaS.Platform.API.Application.Validators
{
    public class Documentsvalidators
    {


        /// <summary>
        /// Validator for CreateDocumentsDto with business rules
        /// </summary>
        public class CreateDocumentsdtoValidator : AbstractValidator<CreateDocumentsdto>
        {
            public CreateDocumentsdtoValidator()
            {
                RuleFor(x => x.TenantId)
                    .NotEmpty()
                    .WithMessage("Tenant ID is required");



                RuleFor(x => x.DocumentType)
                    .NotEmpty()
                    .WithMessage("Document type is required")
                    .Must(x => x == "Individual" || x == "Corporate")
                    .WithMessage("Document type must be either 'Individual' or 'Corporate'");


                // Corporate customer validation
                When(x => x.DocumentType == "Corporate", () =>
                {
                    RuleFor(x => x.DocumentName)
                        .NotEmpty()
                        .WithMessage("Document name is required for corporate documents")
                        .MaximumLength(200)
                        .WithMessage("Document name cannot exceed 200 characters");
                });// Corporate document validation
                When(x => x.DocumentType == "Corporate", () =>
                {
                    RuleFor(x => x.DocumentName)
                        .NotEmpty()
                        .WithMessage("Company name is required for corporate customers")
                        .MaximumLength(200)
                        .WithMessage("Company name cannot exceed 200 characters");
                });

            }
        }

        /// <summary>
        /// Validator for UpdateCustomerDto with business rules
        /// </summary>
        public class UpdatedocumentsdtoValidator : AbstractValidator<UpdateDocumentsdto>
        {
            public UpdatedocumentsdtoValidator()
            {


                RuleFor(x => x.DocumentName)
                    .MaximumLength(200)
                    .WithMessage("Company name cannot exceed 200 characters")
                    .When(x => !string.IsNullOrWhiteSpace(x.DocumentName));

            }
        }
    }
}