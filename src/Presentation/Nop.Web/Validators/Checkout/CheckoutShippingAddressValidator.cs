using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Checkout;
using Nop.Web.Validators.Common;

namespace Nop.Web.Validators.Checkout;

public partial class CheckoutShippingAddressValidator : AbstractValidator<CheckoutShippingAddressModel>
{
    public CheckoutShippingAddressValidator(ILocalizationService localizationService,
        IStateProvinceService stateProvinceService,
        AddressSettings addressSettings,
        CustomerSettings customerSettings)
    {
        RuleFor(shippingAdress => shippingAdress.ShippingNewAddress).SetValidator(
            new AddressValidator(localizationService, stateProvinceService, addressSettings, customerSettings));
    }
}