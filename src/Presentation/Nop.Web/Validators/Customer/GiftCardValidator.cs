using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;

namespace Nop.Web.Validators.Customer;

public partial class GiftCardValidator : BaseNopValidator<CheckGiftCardBalanceModel>
{
    public GiftCardValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.GiftCardCode).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("CheckGiftCardBalance.GiftCardCouponCode.Empty"));
    }
}