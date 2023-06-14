using System.Collections.Generic;
using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.ExternalAuth.Facebook.Migrations
{
    [NopMigration("2022-06-23 00:00:00", "ExternalAuth.Facebook 1.77. Data deletion feature", MigrationProcessType.Update)]
    public class DataDeletionMigration : MigrationBase
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public DataDeletionMigration(ILanguageService languageService,
            ILocalizationService localizationService)
        {
            _languageService = languageService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            var (languageId, _) = this.GetLanguageData(_languageService);

            _localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.GoogleAuth.AuthenticationDataDeletedSuccessfully"] = "Data deletion request completed",
                ["Plugins.GoogleAuth.AuthenticationDataExist"] = "Data deletion request is pending, please contact the admin",
                ["Plugins.GoogleAuth.Instructions"] = "<p>To configure authentication with Google, please follow these steps:<br/><br/><ol><li>Navigate to the for Google API & Services and select create project.</li><li>Fill out the form and tap the Create button.</li><li>In the Oauth consent screen of the Dashboard.</li><li>Select User Type External and CREATE.</li><li>In the App information dialog Provide an app name for the app user support email and developer contact information.</li><li>Step through the Scopes and the test users step.</li><li>Review the OAuth consent screen and go back to the app Dashboard.</li><li>In the Credentials tab of the application Dashboard select CREATE CREDENTIALS OAuth client ID.</li><li>Select Application type Web application and choose a name.</li><li>In the Authorized redirect URIs section, select ADD URI to set the redirect URI. Example redirect URI: \"{0:s}signin-google\" into the <b>Valid OAuth Redirect URIs</b> field.</li><li>Copy your Client ID and Client secret below.</li></ol><br/><br/></p>"
            }, languageId);
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            //nothing
        }

        #endregion
    }
}