using FluentMigrator;
using Nop.Data;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Tax.Avalara.Domain;

namespace Nop.Plugin.Tax.Avalara.Data
{
    [NopMigration("2022-07-13 00:00:02", "Tax.Avalara 2.65. Update datetime type precision", MigrationProcessType.Update)]
    public class MySqlDateTimeWithPrecisionMigration : Migration
    {

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            var dataSettings = DataSettingsManager.LoadSettings();

            //update the types only in MySql 
            if (dataSettings.DataProvider != DataProviderType.MySql)
                return;

            Alter.Table(NameCompatibilityManager.GetTableName(typeof(TaxTransactionLog)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(TaxTransactionLog), nameof(TaxTransactionLog.CreatedDateUtc)))
                 .AsCustom("datetime(6)");
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            //nothing
        }
    }
}