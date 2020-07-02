using FluentMigrator;
using Nop.Core;
using Nop.Data.Migrations;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Migrations.UpgradeTo440
{
    [NopMigration(NopVersion.FULL_VERSION, MigrationType.Localization, "version 4.40. Update localization")]
    public class LocalizationMigration : MigrationBase
    {
        private readonly ILocalizationService _localizationService;

        public LocalizationMigration(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            //use _localizationService to add, update and delete localization resources
            
            #if DEBUG
            throw new PreventFixMigrationException();
            #endif
        }

        /// <summary>Collects the DOWN migration expressions</summary>
        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
