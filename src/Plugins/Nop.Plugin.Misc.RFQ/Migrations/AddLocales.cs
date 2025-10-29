using System.Globalization;
using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Common;
using Nop.Services.Localization;

namespace Nop.Plugin.Misc.RFQ.Migrations;

[NopMigration("2025/10/30 20:17:53:1677556", "Misc.RFQ add the locale")]
public class AddLocales : ForwardOnlyMigration
{
    private readonly ILanguageService _languageService;
    private readonly ILocalizationService _localizationService;

    public AddLocales(ILanguageService languageService, 
        ILocalizationService localizationService)
    {
        _languageService = languageService;
        _localizationService = localizationService;
    }

    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var languages = _languageService.GetAllLanguagesAsync(true).Result;
        var languageId = languages
            .Where(lang => lang.UniqueSeoCode == new CultureInfo(NopCommonDefaults.DefaultLanguageCulture).TwoLetterISOLanguageName)
            .Select(lang => lang.Id).FirstOrDefault();

        _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Misc.RFQ.CustomerRequest.RequestedQty.MustGreaterThanZero"] = "Requested qty of \"{0}\" product must be greater than zero.",
            ["Plugins.Misc.RFQ.CustomerRequest.RequestedUnitPrice.MustBeEqualOrGreaterThanZero"] = "Requested unit price of \"{0}\" product must be equal or greater than zero."
        }, languageId).Wait();
    }
}