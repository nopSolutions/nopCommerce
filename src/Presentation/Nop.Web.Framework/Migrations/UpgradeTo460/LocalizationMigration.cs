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

                //#29
                ["Admin.Configuration.Settings.Catalog.DisplayFromPrices"] = "Display \"From\" prices",
                ["Admin.Configuration.Settings.Catalog.DisplayFromPrices.Hint"] = "Check to display \"From\" prices on catalog pages.",

            }, languageId).Wait();
        }

        /// <summary>Collects the DOWN migration expressions</summary>
        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
