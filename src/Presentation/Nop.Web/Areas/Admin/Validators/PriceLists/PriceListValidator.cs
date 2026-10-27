using FluentValidation;
using Nop.Core.Domain.PriceLists;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.PriceLists;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.PriceLists;

public partial class PriceListValidator : BaseNopValidator<PriceListModel>
{
    public PriceListValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.PriceLists.Fields.Name.Required"));

        SetDatabaseValidationRules<PriceList>();
    }
}
