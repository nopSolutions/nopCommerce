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
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var settingRepository = EngineContext.Current.Resolve<IRepository<Setting>>();
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            //#4904 External authentication errors logging
            var externalAuthenticationSettings = settingService.LoadSettingAsync<ExternalAuthenticationSettings>().Result;
            if (!settingService.SettingExistsAsync(externalAuthenticationSettings, settings => settings.LogErrors).Result)
            {
                externalAuthenticationSettings.LogErrors = false;
                settingService.SaveSettingAsync(externalAuthenticationSettings).Wait();
            }

            var multiFactorAuthenticationSettings = settingService.LoadSettingAsync<MultiFactorAuthenticationSettings>().Result;
            if (!settingService.SettingExistsAsync(multiFactorAuthenticationSettings, settings => settings.ForceMultifactorAuthentication).Result)
            {
                multiFactorAuthenticationSettings.ForceMultifactorAuthentication = false;

                settingService.SaveSettingAsync(multiFactorAuthenticationSettings).Wait();
            }

            //#5102 Delete Full-text settings
            settingRepository
                .DeleteAsync(setting => setting.Name == "commonsettings.usefulltextsearch" || setting.Name == "commonsettings.fulltextmode")
                .Wait();

            //#4196
            settingRepository
                .DeleteAsync(setting => setting.Name == "commonsettings.scheduletaskruntimeout" ||
                    setting.Name == "commonsettings.staticfilescachecontrol" ||
                    setting.Name == "commonsettings.supportpreviousnopcommerceversions" ||
                    setting.Name == "securitysettings.pluginstaticfileextensionsBlacklist")
                .Wait();

            var seoSettings = settingService.LoadSettingAsync<SeoSettings>().Result;
            var newUrlRecord = "products";
            if (!seoSettings.ReservedUrlRecordSlugs.Contains(newUrlRecord))
            {
                seoSettings.ReservedUrlRecordSlugs.Add(newUrlRecord);
                settingService.SaveSettingAsync(seoSettings).Wait();
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}