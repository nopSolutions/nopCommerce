using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Areas.Admin.Validators.Common;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Settings;

public partial class ShippingSettingsValidator : BaseNopValidator<ShippingSettingsModel>
{
    public ShippingSettingsValidator(AddressSettings addressSettings, CustomerSettings customerSettings, ILocalizationService localizationService)
    {
        RuleFor(model => model.ShippingOriginAddress).SetValidator(new AddressValidator(addressSettings, customerSettings, localizationService));
    }
}