using FluentMigrator;
using Nop.Data;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Shipping.EasyPost.Domain.Batch;

namespace Nop.Plugin.Shipping.EasyPost.Data
{
    [NopMigration("2022-07-13 00:00:01", "EasyPost 1.31. Update datetime type precision", MigrationProcessType.Update)]
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

            Alter.Table(NameCompatibilityManager.GetTableName(typeof(EasyPostBatch)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(EasyPostBatch), nameof(EasyPostBatch.CreatedOnUtc)))
                 .AsCustom("datetime(6)");

            Alter.Table(NameCompatibilityManager.GetTableName(typeof(EasyPostBatch)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(EasyPostBatch), nameof(EasyPostBatch.UpdatedOnUtc)))
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