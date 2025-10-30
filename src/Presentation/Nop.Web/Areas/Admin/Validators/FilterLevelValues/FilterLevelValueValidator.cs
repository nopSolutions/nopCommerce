using FluentValidation;
using Nop.Core.Domain.FilterLevels;
using Nop.Services.FilterLevels;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.FilterLevelValues;

public partial class FilterLevelValueValidator : BaseNopValidator<FilterLevelValueModel>
{
    public FilterLevelValueValidator(IFilterLevelValueService filterLevelValueService,
        ILocalizationService localizationService)
    {
        var locale = localizationService.GetResourceAsync(NopValidationDefaults.NotNullValidationLocaleName);
        var (filterLevel1Disabled, filterLevel2Disabled, filterLevel3Disabled) = filterLevelValueService.IsFilterLevelDisabled();

        if (!filterLevel1Disabled)
        {
            RuleFor(x => x.FilterLevel1Value)
                .NotEmpty()
                .WithMessageAwait(locale, localizationService.GetLocalizedEnumAsync(FilterLevelEnum.FilterLevel1).Result);
        }

        if (!filterLevel2Disabled)
        {
            RuleFor(x => x.FilterLevel2Value)
                .NotEmpty()
                .WithMessageAwait(locale, localizationService.GetLocalizedEnumAsync(FilterLevelEnum.FilterLevel2).Result);
        }
            
        if (!filterLevel3Disabled)
        {
            RuleFor(x => x.FilterLevel3Value)
                .NotEmpty()
                .WithMessageAwait(locale, localizationService.GetLocalizedEnumAsync(FilterLevelEnum.FilterLevel3).Result);
        }

        SetDatabaseValidationRules<FilterLevelValue>();
    }
}
