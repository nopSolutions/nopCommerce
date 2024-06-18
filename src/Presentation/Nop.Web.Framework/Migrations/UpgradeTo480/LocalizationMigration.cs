using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo480;

[NopUpdateMigration("2024-05-15 00:00:00", "4.80", UpdateMigrationType.Localization)]
public class LocalizationMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //do not use DI, because it produces exception on the installation process
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        var (languageId, _) = this.GetLanguageData();

        #region Delete locales

        localizationService.DeleteLocaleResources(new List<string>
        {
            //#6977
            "Common.FileUploader.Upload",
            "Common.FileUploader.Upload.Files",
            "Common.FileUploader.Cancel",
            "Common.FileUploader.Retry",
            "Common.FileUploader.Delete",

            //#7215
            "Admin.Configuration.Settings.ProductEditor.DisplayAttributeCombinationImagesOnly",
        });

        #endregion

        #region Rename locales

        #endregion

        #region Add or update locales

        localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            //#6977
            ["Common.FileUploader.Browse"] = "Browse",
            ["Common.FileUploader.Processing"] = "Uploading...", 

            //#7089
            ["Admin.ContentManagement.MessageTemplates.List.SearchEmailAccount"] = "Email account",
            ["Admin.ContentManagement.MessageTemplates.List.SearchEmailAccount.All"] = "All",

            //#7108
            ["Admin.ContentManagement.MessageTemplates.Description.OrderCancelled.VendorNotification"] = "This message template is used to notify a vendor that the certain order was cancelled.The order can be cancelled by a customer on the account page or by store owner in Customers - Customers in Orders tab or in Sales - Orders.",

            //#7215
            ["Admin.Catalog.Products.Fields.DisplayAttributeCombinationImagesOnly.Hint"] = "You may choose pictures associated to each product attribute value or attribute combination (these pictures will replace the main product image when this product attribute value or attribute combination is selected). Enable this option if you want to display only images of a chosen product attribute value or a attribute combination (other pictures will be hidden). Otherwise, all uploaded pictures will be displayed on the product details page",

            //#7208
            ["Admin.Customers.Customers.List.SearchIsActive"] = "Is active",
            ["Admin.Customers.Customers.List.SearchIsActive.Hint"] = "Search customers by an account status.",
        }, languageId);

        #endregion
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
