using FluentMigrator;
using Nop.Core;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Web.Framework.Migrations.UpgradeTo440
{
    [NopMigration(NopVersion.FULL_VERSION, MigrationType.Setting, "version 4.40. Update settings")]
    public class SettingMigration: MigrationBase
    {
        private readonly ISettingService _settingService;

        public SettingMigration(ISettingService settingService)
        {
            _settingService = settingService;
        }

        public override void Up()
        {
            //use _settingService to add, update and delete settings
            
            #if DEBUG
            throw new PreventFixMigrationException();
            #endif
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
