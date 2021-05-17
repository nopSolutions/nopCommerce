using FluentMigrator;
using Nop.Core.Domain.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Web.Framework.Migrations.UpgradeTo450
{
    [NopMigration("2021-04-23 00:00:00", "4.50.0", UpdateMigrationType.Settings)]
    [SkipMigrationOnInstall]
    public class SettingMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var settingRepository = EngineContext.Current.Resolve<IRepository<Setting>>();
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            //miniprofiler settings are moved to appSettings
            settingRepository
                .DeleteAsync(setting => setting.Name == "storeinformationsettings.displayminiprofilerforadminonly" ||
                    setting.Name == "storeinformationsettings.displayminiprofilerinpublicstore").Wait();
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}