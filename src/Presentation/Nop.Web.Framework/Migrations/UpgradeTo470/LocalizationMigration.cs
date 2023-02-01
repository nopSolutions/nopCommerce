using System.Collections.Generic;
using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo470
{
    [NopUpdateMigration("2023-01-01 00:00:00", "4.70.0", UpdateMigrationType.Localization)]
    public class LocalizationMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var (languageId, languages) = this.GetLanguageData();

            #region Delete locales

            localizationService.DeleteLocaleResources(new List<string>
            {
                //#4834
                "Admin.System.Warnings.PluginNotLoaded",
            });

            #endregion

            #region Rename locales

            #endregion

            #region Add or update locales

            localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
            {
                //#4834
                ["Admin.System.Warnings.PluginMainAssemblyNotFound"] = "{0}: The main assembly isn't found. Hence this plugin can't be loaded.",
                ["Admin.System.Warnings.PluginNotCompatibleWithCurrentVersion"] = "{0}: The plugin isn't compatible with the current version. Hence this plugin can't be loaded.",
                //#6309
                ["ShoppingCart.ReorderWarning"] = "Some products are not available anymore, so they weren't added to the cart.",
                //import product refactoring
                ["Admin.Catalog.Products.Import.DatabaseNotContainCategory"] = "Import product '{0}'. Database doesn't contain the '{1}' category",
                ["Admin.Catalog.Products.Import.DatabaseNotContainManufacturer"] = "Import product '{0}'. Database doesn't contain the '{1}' manufacturer",
                //6551
                ["Admin.Configuration.Settings.Catalog.CacheProductPrices.Hint"] = "Check to cache product prices. It can significantly improve performance. But you should not enable it if you use some complex discounts, discount requirement rules, or coupon codes.",
                //6541
                ["Customer.FullNameFormat"] = "{0} {1}",
                //6521
                ["Admin.Configuration.AppSettings.Common.CrawlerOnlyAdditionalUserAgentStringsPath"] = "Crawler user agent additional strings path",
                ["Admin.Configuration.AppSettings.Common.CrawlerOnlyAdditionalUserAgentStringsPath.Hint"] = "Specify a path to the file with additional crawler only user agent strings.",
                //6557
                ["Admin.Configuration.Settings.CustomerUser.PasswordMaxLength"] = "Password maximum length",
                ["Admin.Configuration.Settings.CustomerUser.PasswordMaxLength.Hint"] = "Specify password maximum length",
                ["Validation.Password.LengthValidation"] = "<li>must have at least {0} characters and not greater than {1} characters</li>",
            }, languageId);

            #endregion
        }

        /// <summary>Collects the DOWN migration expressions</summary>
        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
