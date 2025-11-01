using System.Globalization;
using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Common;
using Nop.Services.Localization;

namespace Nop.Plugin.Misc.RFQ.Data.Migrations;

[NopMigration("2025/11/01 19:41:53:1677556", "Misc.RFQ add the locale")]
public class AddLocales : Migration
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
            ["Plugins.Misc.RFQ.ShowCaptchaOnRequestPage"] = "Show CAPTCHA on request page",
            ["Plugins.Misc.RFQ.ShowCaptchaOnRequestPage.Hint"] = "Check to show CAPTCHA on request page, when send the new request a quote.",
            ["Plugins.Misc.RFQ.CaptchaDisabled.Notification"] = "In order to use this functionality, you have to enable the following setting: <a href='{0}' target='_blank'>General settings > CAPTCHA > CAPTCHA enabled</a>",
            ["Plugins.Misc.RFQ.CreatePdf"] = "Get a PDF quote",
            ["Plugins.Misc.RFQ.PdfFileName"] = "quote_{0}",
            ["Plugins.Misc.RFQ.AllowCustomerGenerateQuotePdf"] = "Quote PDF is allowed for the customer",
            ["Plugins.Misc.RFQ.AllowCustomerGenerateQuotePdf.Hint"] = "Check to allow the generation of the quote PDF for the customer",
            ["Plugins.Misc.RFQ.Fields.Quote.CustomerInfo"] = "Customer"
        }, languageId).Wait();
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        //nothing
    }
}