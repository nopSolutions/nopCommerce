using System.Collections.Generic;
using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Migrations.UpgradeTo440
{
    [NopMigration(MigrationType.Localization)]
    [SkipMigrationOnInstall]
    public class LocalizationMigration : MigrationBase
    { 
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if(!DataSettingsManager.DatabaseIsInstalled)
                return;

            //do not use DI, because it produces exception on the installation process
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            //use localizationService to add, update and delete localization resources
            
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
