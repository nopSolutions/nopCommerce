using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Plugin.ExchangeRate.EcbExchange.Data;

[NopMigration("2021-09-16 00:00:00", "ExchangeRate.EcbExchange 1.30. Add setting for url for ECB", MigrationProcessType.Update)]
public class ExchangeEcbMigration : MigrationBase
{
    #region Fields

    protected readonly EcbExchangeRateSettings _ecbExchangeRateSettings;
    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public ExchangeEcbMigration(EcbExchangeRateSettings ecbExchangeRateSettings,
        ISettingService settingService)
    {
        _ecbExchangeRateSettings = ecbExchangeRateSettings;
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
        if (!_settingService.SettingExists(_ecbExchangeRateSettings, settings => settings.EcbLink))
            _ecbExchangeRateSettings.EcbLink = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

        _settingService.SaveSetting(_ecbExchangeRateSettings);
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