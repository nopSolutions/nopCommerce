using FluentMigrator;
using Nop.Data;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.FacebookPixel.Domain;

namespace Nop.Plugin.Widgets.FacebookPixel.Data
{
    [NopMigration("2022-03-18 00:00:00", "Widgets.FacebookPixel 2.00. Add conversions api configuration columns", MigrationProcessType.Update)]
    public class SchemaMigration : MigrationBase
    {
        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            var facebookPixelConfigurationTableName = NameCompatibilityManager.GetTableName(typeof(FacebookPixelConfiguration));

            //add ConversionsApiEnabled column if not exists
            if (!Schema.Table(facebookPixelConfigurationTableName).Column(nameof(FacebookPixelConfiguration.ConversionsApiEnabled)).Exists())
                Alter.Table(facebookPixelConfigurationTableName)
                    .AddColumn(nameof(FacebookPixelConfiguration.ConversionsApiEnabled)).AsBoolean();

            //add AccessToken column if not exists
            if (!Schema.Table(facebookPixelConfigurationTableName).Column(nameof(FacebookPixelConfiguration.AccessToken)).Exists())
                Alter.Table(facebookPixelConfigurationTableName)
                    .AddColumn(nameof(FacebookPixelConfiguration.AccessToken)).AsString();

            //rename Enabled column to PixelScriptEnabled
            if (Schema.Table(facebookPixelConfigurationTableName).Column("Enabled").Exists())
                Rename.Column("Enabled")
                    .OnTable(facebookPixelConfigurationTableName)
                    .To(nameof(FacebookPixelConfiguration.PixelScriptEnabled));
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
