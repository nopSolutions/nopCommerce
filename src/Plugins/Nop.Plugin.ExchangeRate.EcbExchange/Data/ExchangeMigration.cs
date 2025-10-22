using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.ExchangeRate.EcbExchange.Data;

[NopMigration("2021-09-16 00:00:00", "ExchangeRate.EcbExchange 1.30. Add setting for url for ECB", MigrationProcessType.Update)]
public class ExchangeEcbMigration : MigrationBase
{
    #region Fields

    protected readonly EcbExchangeRateSettings _ecbExchangeRateSettings;

    #endregion

    #region Ctor

    public ExchangeEcbMigration(EcbExchangeRateSettings ecbExchangeRateSettings)
    {
        _ecbExchangeRateSettings = ecbExchangeRateSettings;
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
        if (!this.SettingExists(_ecbExchangeRateSettings, settings => settings.EcbLink))
            _ecbExchangeRateSettings.EcbLink = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

        this.SaveSetting(_ecbExchangeRateSettings);
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