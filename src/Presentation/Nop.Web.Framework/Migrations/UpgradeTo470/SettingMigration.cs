using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Web.Framework.Migrations.UpgradeTo470
{
    [NopUpdateMigration("2023-02-01 14:00:03", "4.70.0", UpdateMigrationType.Settings)]
    public class SettingMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            var customerSettings = settingService.LoadSetting<CustomerSettings>();
            if (!settingService.SettingExists(customerSettings, settings => settings.PasswordMaxLength))
            {
                customerSettings.PasswordMaxLength = 64;
                settingService.SaveSetting(customerSettings, settings => settings.PasswordMaxLength);
            }

            if (!settingService.SettingExists(customerSettings, settings => settings.DefaultCountryId))
            {
                customerSettings.DefaultCountryId = null;
                settingService.SaveSetting(customerSettings, settings => settings.DefaultCountryId);
            }

            var securitySettings = settingService.LoadSetting<SecuritySettings>();
            if (!settingService.SettingExists(securitySettings, settings => settings.UseAesEncryptionAlgorithm))
            {
                securitySettings.UseAesEncryptionAlgorithm = false;
                settingService.SaveSetting(securitySettings, settings => settings.UseAesEncryptionAlgorithm);
            }

            if (!settingService.SettingExists(securitySettings, settings => settings.AllowStoreOwnerExportImportCustomersWithHashedPassword))
            {
                securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword = true;
                settingService.SaveSetting(securitySettings, settings => settings.AllowStoreOwnerExportImportCustomersWithHashedPassword);
            }

            var addressSettings = settingService.LoadSetting<AddressSettings>();
            if (!settingService.SettingExists(addressSettings, settings => settings.DefaultCountryId))
            {
                addressSettings.DefaultCountryId = null;
                settingService.SaveSetting(addressSettings, settings => settings.DefaultCountryId);
            }

            var taxSettings = settingService.LoadSetting<TaxSettings>();
            if (!settingService.SettingExists(taxSettings, settings => settings.AutomaticallyDetectCountry))
            {
                taxSettings.AutomaticallyDetectCountry = true;
                settingService.SaveSetting(taxSettings, settings => settings.AutomaticallyDetectCountry);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}