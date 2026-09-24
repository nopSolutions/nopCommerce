using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Affiliates;
using Nop.Web.Areas.Admin.Validators.Common;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Affiliates;

public partial class AffiliateValidator : BaseNopValidator<AffiliateModel>
{
    public AffiliateValidator(AddressSettings addressSettings, CustomerSettings customerSettings, ILocalizationService localizationService)
    {
        RuleFor(model => model.Address).SetValidator(new AddressValidator(addressSettings, customerSettings, localizationService));

        SetDatabaseValidationRules<Affiliate>();
    }
}
