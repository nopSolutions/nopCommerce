using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Checkout;
using Nop.Web.Validators.Common;

namespace Nop.Web.Validators.Checkout;

public partial class CheckoutBillingAddressValidator : AbstractValidator<CheckoutBillingAddressModel>
{
    public CheckoutBillingAddressValidator(ILocalizationService localizationService,
        IStateProvinceService stateProvinceService,
        AddressSettings addressSettings,
        CustomerSettings customerSettings)
    {
        RuleFor(billingAddress => billingAddress.BillingNewAddress).SetValidator(new AddressValidator(localizationService, stateProvinceService, addressSettings, customerSettings));
    }
}