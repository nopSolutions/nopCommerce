using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.ExchangeRate.EcbExchange.Data;

[NopMigration("2021-09-16 00:00:00", "ExchangeRate.EcbExchange 1.30. Add setting for url for ECB", MigrationProcessType.Update)]
public class ExchangeEcbMigration : MigrationBase
{
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //settings
        this.SetSettingIfNotExists<EcbExchangeRateSettings, string>(settings => settings.EcbLink,
            "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");
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