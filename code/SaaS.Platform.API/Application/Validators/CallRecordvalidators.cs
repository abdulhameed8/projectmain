using FluentValidation;
using SaaS.Platform.API.Application.DTOs.CallRecord;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateCallRecordDto with business rules
    /// </summary>
    public class CreateCallRecordtoValidator : AbstractValidator<CreateCallRecorddto>
    {
        public CreateCallRecordtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

           
            RuleFor(x => x.CallType)
                .NotEmpty()
                .WithMessage("Call type is required")
                .Must(x => x == "Individual" || x == "Corporate")
                .WithMessage("Call type must be either 'Individual' or 'Corporate'");

            RuleFor(x => x.CallStatus)
                .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                .WithMessage("Call status must be Active, Inactive, or Blocked")
                .When(x => !string.IsNullOrWhiteSpace(x.CallStatus));




        }
    }

    /// <summary>
    /// Validator for UpdateCallRecordDto with business rules
    /// </summary>
    public class UpdateCallRecordtoValidator : AbstractValidator<UpdateCallRecorddto>
    {
        public UpdateCallRecordtoValidator()
        {


            RuleFor(x => x.CallStatus)
                .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
                .WithMessage("Call status must be Active, Inactive, or Blocked")
                .When(x => !string.IsNullOrWhiteSpace(x.CallStatus));

        }
    }
}