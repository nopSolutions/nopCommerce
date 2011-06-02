using FluentValidation;
using Nop.Admin.Models.Discounts;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Discounts
{
    public class DiscountValidator : AbstractValidator<DiscountModel>
    {
        public DiscountValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Promotions.Discounts.Fields.Name.Required"));
        }
    }
}