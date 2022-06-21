using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Common;
using Nop.Services.Localization;

namespace Nop.Plugin.ExternalAuth.Facebook.Migrations
{
    [NopMigration("2022-04-05 17:00:00", "ExternalAuth.Facebook 1.77. Update localizations", MigrationProcessType.Update)]
    public class LocalizationMigration : MigrationBase
    {
        public override void Down()
        {
            //add the downgrade logic if necessary 
        }

        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var languageService = EngineContext.Current.Resolve<ILanguageService>();
            var languages = languageService.GetAllLanguagesAsync(true).Result;
            var languageId = languages
                .Where(lang => lang.UniqueSeoCode == new CultureInfo(NopCommonDefaults.DefaultLanguageCulture).TwoLetterISOLanguageName)
                .Select(lang => lang.Id).FirstOrDefault();

            //use localizationService to add, update and delete localization resources
            localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.ExternalAuth.Facebook.AuthenticationDataDeletedSuccessfully"] = "Facebook authentication data already deleted.",
                ["Plugins.ExternalAuth.Facebook.AuthenticationDataExist"] = "Facebook authentication data exist on the database. Please contact with an admin.",
                ["Plugins.ExternalAuth.Facebook.Instructions"] = "<p>To configure authentication with Facebook, please follow these steps:<br/><br/><ol><li>Navigate to the <a href=\"https://developers.facebook.com/apps\" target =\"_blank\"> Facebook for Developers</a> page and sign in. If you don't already have a Facebook account, use the <b>Sign up for Facebook</b> link on the login page to create one.</li><li>Tap the <b>+ Add a New App button</b> in the upper right corner to create a new App ID. (If this is your first app with Facebook, the text of the button will be <b>Create a New App</b>.)</li><li>Fill out the form and tap the <b>Create App ID button</b>.</li><li>The <b>Product Setup</b> page is displayed, letting you select the features for your new app. Click <b>Get Started</b> on <b>Facebook Login</b>.</li><li>Click the <b>Settings</b> link in the menu at the left, you are presented with the <b>Client OAuth Settings</b> page with some defaults already set.</li><li>Enter \"{0:s}signin-facebook\" into the <b>Valid OAuth Redirect URIs</b> field.</li><li>From User data deletion dropdown menu select \"Data deletion instructions URL\" </li><li> Enter \"{0:s}facebook/data-deletion-callback/\" into the <b> User data deletion </b> input field.</li><li>Click <b>Save Changes</b>.</li><li>Click the <b>Dashboard</b> link in the left navigation.</li><li>Copy your App ID and App secret below.</li></ol><br/><br/></p>"
            }, languageId).Wait();
        }
    }
}
