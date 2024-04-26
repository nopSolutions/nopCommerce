using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Payments.PayPalCommerce.Data;

[NopMigration("2021-12-01 00:00:00", "Payments.PayPalCommerce 1.07. Add Pay Later message", MigrationProcessType.Update)]
internal class PayLaterMessageMigration : MigrationBase
{
    #region Fields

    protected readonly PayPalCommerceSettings _payPalCommerceSettings;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public PayLaterMessageMigration(PayPalCommerceSettings payPalCommerceSettings,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ISettingService settingService)
    {
        _payPalCommerceSettings = payPalCommerceSettings;
        _languageService = languageService;
        _localizationService = localizationService;
        _settingService = settingService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //locales
        var (languageId, languages) = this.GetLanguageData();

        _localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Plugins.Payments.PayPalCommerce.Fields.DisplayPayLaterMessages"] = "Display Pay Later messages",
            ["Plugins.Payments.PayPalCommerce.Fields.DisplayPayLaterMessages.Hint"] = "Determine whether to display Pay Later messages. This message displays how much the customer pays in four payments. The message will be shown next to the PayPal buttons.",
        }, languageId);


        //settings
        if (!_settingService.SettingExists(_payPalCommerceSettings, settings => settings.DisplayPayLaterMessages))
            _payPalCommerceSettings.DisplayPayLaterMessages = false;

        _settingService.SaveSetting(_payPalCommerceSettings);
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        //nothing
    }

    #endregion
}