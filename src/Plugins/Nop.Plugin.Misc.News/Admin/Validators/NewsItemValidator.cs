using FluentValidation;
using Nop.Plugin.Misc.News.Admin.Models;
using Nop.Plugin.Misc.News.Domain;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.News.Admin.Validators;

/// <summary>
/// Represents news item model validator
/// </summary>
public class NewsItemValidator : BaseNopValidator<NewsItemModel>
{
    public NewsItemValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.News.NewsItems.Fields.Title.Required"));

        RuleFor(x => x.Short)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.News.NewsItems.Fields.Short.Required"));

        RuleFor(x => x.Full)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.News.NewsItems.Fields.Full.Required"));

        RuleFor(x => x.SeName)
            .Length(0, NopSeoDefaults.SearchEngineNameLength)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), NopSeoDefaults.SearchEngineNameLength);

        SetDatabaseValidationRules<NewsItem>();
    }
}