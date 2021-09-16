using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Plugin.ExchangeRate.EcbExchange;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.ExchangeRate.EcbExchange.Data
{ 
    [NopMigration("2021-09-16 00:00:00", "ExchangeRate.EcbExchange 1.30. Add setting for url for ECB")]
    [SkipMigrationOnInstall]
    public class ExchangeEcbMigration : MigrationBase
    {
        #region Fields

        private readonly EcbExchangeRateSettings _ecbExchangeRateSettings;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public ExchangeEcbMigration(EcbExchangeRateSettings ecbExchangeRateSettings,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ISettingService settingService)
        {
            _ecbExchangeRateSettings = ecbExchangeRateSettings;
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
            
            //settings
            if (!_settingService.SettingExistsAsync(_ecbExchangeRateSettings, settings => settings.EcbLink).Result)
                _ecbExchangeRateSettings.EcbLink = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

            _settingService.SaveSettingAsync(_ecbExchangeRateSettings).Wait();
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