using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Common;
using Nop.Services.Localization;

namespace Nop.Plugin.Widgets.FacebookPixel.Data.UpgradeTo460
{
    [NopMigration("2022-03-18 12:00:00", "Widgets.FacebookPixel 2.00. Update localizations", MigrationProcessType.Update)]
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
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.AccessToken"] = "Access token",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.AccessToken.Hint"] = "Enter the Facebook Conversions API access token.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.AccessToken.Required"] = "Access token is required",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelScriptEnabled"] = "Pixel enabled",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelScriptEnabled.Hint"] = "Toggle to enable/disable Facebook Pixel for this configuration.",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.ConversionsApiEnabled"] = "Conversions API enabled",
                ["Plugins.Widgets.FacebookPixel.Configuration.Fields.ConversionsApiEnabled.Hint"] = "Toggle to enable/disable Facebook Conversions API for this configuration."

            }, languageId).Wait();

            localizationService.DeleteLocaleResourcesAsync(new List<string>
            {
                "Plugins.Widgets.FacebookPixel.Configuration.Fields.Enabled",
                "Plugins.Widgets.FacebookPixel.Configuration.Fields.Enabled.Hint"

            }, languageId).Wait();
        }

        /// <summary>Collects the DOWN migration expressions</summary>
        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
