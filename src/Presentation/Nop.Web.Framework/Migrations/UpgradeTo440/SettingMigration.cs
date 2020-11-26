using FluentMigrator;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Seo;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Web.Framework.Migrations.UpgradeTo440
{
    [NopMigration("2020-06-10 00:00:00", "4.40.0", UpdateMigrationType.Settings)]
    [SkipMigrationOnInstall]
    public class SettingMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.DatabaseIsInstalled)
                return;

            //do not use DI, because it produces exception on the installation process
            var settingRepository = EngineContext.Current.Resolve<IRepository<Setting>>();
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            //#4904 External authentication errors logging
            var externalAuthenticationSettings = settingService.LoadSetting<ExternalAuthenticationSettings>();
            if (!settingService.SettingExists(externalAuthenticationSettings, settings => settings.LogErrors))
            {
                externalAuthenticationSettings.LogErrors = false;
                settingService.SaveSetting(externalAuthenticationSettings);
            }

            var multiFactorAuthenticationSettings = settingService.LoadSetting<MultiFactorAuthenticationSettings>();
            if (!settingService.SettingExists(multiFactorAuthenticationSettings, settings => settings.ForceMultifactorAuthentication))
            {
                multiFactorAuthenticationSettings.ForceMultifactorAuthentication = false;

                settingService.SaveSetting(multiFactorAuthenticationSettings);
            }

            //#5102 Delete Full-text settings
            settingRepository.Delete(setting =>
                setting.Name == "commonsettings.usefulltextsearch" ||
                setting.Name == "commonsettings.fulltextmode");

            var seoSettings = settingService.LoadSetting<SeoSettings>();
            var newUrlRecord = "products";
            if(!seoSettings.ReservedUrlRecordSlugs.Contains(newUrlRecord))
            {
                seoSettings.ReservedUrlRecordSlugs.Add(newUrlRecord);
                settingService.SaveSetting(seoSettings);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}