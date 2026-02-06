using FluentValidation;
using SaaS.Platform.API.Application.DTOs.Branches;
using SaaS.Platform.API.Application.DTOs.Cards;

namespace SaaS.Platform.API.Application.Validators
{
    /// <summary>
    /// Validator for CreateBranchesdto with business rules
    /// </summary>
    public class CreateCardsdtoValidator : AbstractValidator<CreateCardsdto>
    {
        public CreateCardsdtoValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required");

            RuleFor(x => x.CardType)
                .NotEmpty()
                .WithMessage("Card type is required")
                .Must(x => x == "Individual" || x == "Corporate")
                .WithMessage("Card type must be either 'Individual' or 'Corporate'");


            RuleFor(x => x.CardStatus)
              .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
              .WithMessage("Card status must be Active, Inactive, or Blocked");

        }
    }

    /// <summary>
    /// Validator for UpdateCardsdto with business rules
    /// </summary>
    public class UpdateCardsdtoValidator : AbstractValidator<UpdateCardsdto>
    {
        public UpdateCardsdtoValidator()
        {

            RuleFor(x => x.CardStatus)
              .Must(x => x == "Active" || x == "Inactive" || x == "Blocked")
              .WithMessage("Card status must be Active, Inactive, or Blocked");


        }
    }
}