using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Areas.Admin.Validators.Common;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Settings;

public partial class TaxSettingsValidator : BaseNopValidator<TaxSettingsModel>
{
    public TaxSettingsValidator(AddressSettings addressSettings, CustomerSettings customerSettings, ILocalizationService localizationService)
    {
        RuleFor(model => model.DefaultTaxAddress).SetValidator(new AddressValidator(addressSettings, customerSettings, localizationService));
    }
}