using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo480
{
    [NopMigration("2024-05-09 00:00:00", "4.80.0", UpdateMigrationType.Localization, MigrationProcessType.Update)]
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

            #endregion

            #region Rename locales

            #endregion

            #region Add or update locales

            localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
            {
                //#7108
                ["Admin.ContentManagement.MessageTemplates.Description.OrderCancelled.VendorNotification"] = "This message template is used to notify a vendor that the certain order was cancelled.The order can be cancelled by a customer on the account page or by store owner in Customers - Customers in Orders tab or in Sales - Orders.",
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