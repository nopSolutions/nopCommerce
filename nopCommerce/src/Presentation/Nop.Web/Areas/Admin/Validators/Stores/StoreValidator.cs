using FluentValidation;
using Nop.Core.Domain.Stores;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Stores;

public partial class StoreValidator : BaseNopValidator<StoreModel>
{
    public StoreValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Stores.Fields.Name.Required"));
        RuleFor(x => x.Url).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Stores.Fields.Url.Required"));

        SetDatabaseValidationRules<Store>();
    }
}