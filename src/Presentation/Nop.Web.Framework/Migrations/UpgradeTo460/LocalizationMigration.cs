using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Common;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Migrations.UpgradeTo460
{
    [NopMigration("2022-02-07 00:00:00", "4.60.0", UpdateMigrationType.Localization, MigrationProcessType.Update)]
    public class LocalizationMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var languageService = EngineContext.Current.Resolve<ILanguageService>();
            var languages = languageService.GetAllLanguagesAsync(true).Result;
            var languageId = languages
                .Where(lang => lang.UniqueSeoCode == new CultureInfo(NopCommonDefaults.DefaultLanguageCulture).TwoLetterISOLanguageName)
                .Select(lang => lang.Id).FirstOrDefault();

            //use localizationService to add, update and delete localization resources
            localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                //#3075
                ["Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithCategoryName"] = "Allow customers to search with category name",
                ["Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithCategoryName.Hint"] = "Check to allow customer to search with category name.",
                ["Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithManufacturerName"] = "Allow customers to search with manufacturer name",
                ["Admin.Configuration.Settings.Catalog.AllowCustomersToSearchWithManufacturerName.Hint"] = "Check to allow customer to search with manufacturer name.",

                //#3997
                ["Admin.Configuration.Settings.GeneralCommon.InstagramLink"] = "Instagram URL",
                ["Admin.Configuration.Settings.GeneralCommon.InstagramLink.Hint"] = "Specify your Instagram page URL. Leave empty if you have no such page.",

                ["Footer.FollowUs.Instagram"] = "Instagram",

                //#3777
                ["ActivityLog.ExportCategories"] = "{0} categories were exported",
                ["ActivityLog.ExportCustomers"] = "{0} customers were exported",
                ["ActivityLog.ExportManufacturers"] = "{0} manufacturers were exported",
                ["ActivityLog.ExportOrders"] = "{0} orders were exported",
                ["ActivityLog.ExportProducts"] = "{0} products were exported",
                ["ActivityLog.ExportStates"] = "{0} states and provinces were exported",
                ["ActivityLog.ExportNewsLetterSubscriptions"] = "{0} newsletter subscriptions were exported",
                ["ActivityLog.ImportNewsLetterSubscriptions"] = "{0} newsletter subscriptions were imported",
            }, languageId).Wait();
        }

        /// <summary>Collects the DOWN migration expressions</summary>
        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
