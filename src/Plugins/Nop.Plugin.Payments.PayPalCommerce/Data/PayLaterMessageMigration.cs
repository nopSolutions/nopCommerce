using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.Payments.PayPalCommerce.Data
{
    [NopMigration("2021-12-01 00:00:00", "Payments.PayPalCommerce 1.07. Add Pay Later message", MigrationProcessType.Update)]
    internal class PayLaterMessageMigration : MigrationBase
    {
        #region Fields

        private readonly PayPalCommerceSettings _payPalCommerceSettings;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;

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
            var languages = _languageService.GetAllLanguagesAsync(true).Result;
            var languageId = languages
                .FirstOrDefault(lang => lang.UniqueSeoCode == new CultureInfo(NopCommonDefaults.DefaultLanguageCulture).TwoLetterISOLanguageName)
                ?.Id;

            _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Payments.PayPalCommerce.Fields.DisplayPayLaterMessages"] = "Display Pay Later messages",
                ["Plugins.Payments.PayPalCommerce.Fields.DisplayPayLaterMessages.Hint"] = "Determine whether to display Pay Later messages. This message displays how much the customer pays in four payments. The message will be shown next to the PayPal buttons.",
            }, languageId).Wait();


            //settings
            if (!_settingService.SettingExistsAsync(_payPalCommerceSettings, settings => settings.DisplayPayLaterMessages).Result)
                _payPalCommerceSettings.DisplayPayLaterMessages = false;
            
            _settingService.SaveSettingAsync(_payPalCommerceSettings).Wait();
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
}
