using System.Collections.Generic;
using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Migrations.UpgradeTo440
{
    [NopMigration("2020-06-10 00:00:00", "4.40.0", UpdateMigrationType.Localization)]
    [SkipMigrationOnInstall]
    public class LocalizationMigration : MigrationBase
    { 
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if(!DataSettingsManager.DatabaseIsInstalled)
                return;

            //do not use DI, because it produces exception on the installation process
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            //use localizationService to add, update and delete localization resources
            localizationService.DeleteLocaleResources(new List<string>
            {
                "Account.Fields.VatNumber.Status",
                "Account.Fields.VatNumberStatus",
                "Account.PasswordRecovery.OldPassword",
                "Account.PasswordRecovery.OldPassword.Required",
                "Account.Register.Unsuccessful",
                "Account.ShoppingCart",
                "ActivityLog.AddNewWidget",
                "ActivityLog.DeleteWidget",
                "ActivityLog.EditWidget",
                "Admin.Address.Fields.StateProvince.Required",
                "Admin.Catalog.AdditionalProductReviews.Fields.Description",
                "Admin.Catalog.Categories.Breadcrumb",
                "Admin.Catalog.Categories.Fields.CreatedOn",
                "Admin.Catalog.Categories.SwitchToListView",
                "Admin.Catalog.Manufacturers.Fields.CreatedOn",
                "Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.ViewLink",
                "Admin.Catalog.Products.ProductAttributes.Attributes.Values.EditAttributeDetails",
                "Admin.Catalog.Products.SpecificationAttributes.NoAttributeOptions",
                "Admin.Catalog.Products.SpecificationAttributes.SelectOption",
                "Admin.Common.CancelChanges",
                "Admin.Common.Check",
                "Admin.Common.DeleteConfirmationParam",
                "Admin.Common.List",
                "Admin.Common.LoseUnsavedChanges",
                "Admin.Common.SaveChanges",
                "Admin.Configuration.Currencies.Localization",
                "Admin.Configuration.Currencies.Select",
                "Admin.Configuration.EmailAccounts.Fields.SendTestEmailTo.Button",
                "Admin.Configuration.PaymentMethods",
                "Admin.Configuration.PaymentMethodsAndRestrictions",
                "Admin.Configuration.Settings.CustomerUser.BlockTitle.DefaultFields",
                "Admin.Configuration.Settings.CustomerUser.BlockTitle.ExternalAuthentication",
                "Admin.Configuration.Settings.CustomerUser.BlockTitle.TimeZone",
                "Admin.Configuration.Settings.CustomerUser.CustomerSettings",
                "Admin.Configuration.Settings.Order.OrderSettings",
                "Admin.Configuration.Settings.ProductEditor.BlockTitle.LinkedProducts",
                "Admin.Configuration.Settings.ProductEditor.Id",
                "Admin.Configuration.Shipping.Measures.Dimensions.Description",
                "Admin.Configuration.Shipping.Measures.Weights.Description",
                "Admin.Configuration.SMSProviders",
                "Admin.Configuration.SMSProviders.BackToList",
                "Admin.Configuration.SMSProviders.Configure",
                "Admin.Configuration.SMSProviders.Fields.FriendlyName",
                "Admin.Configuration.SMSProviders.Fields.IsActive",
                "Admin.Configuration.SMSProviders.Fields.SystemName",
                "Admin.ContentManagement.Topics.Fields.Store.AllStores",
                "Admin.ContentManagement.Widgets.ChooseZone",
                "Admin.ContentManagement.Widgets.ChooseZone.Hint",
                "Admin.Customers.Customers.Fields.Email.Required",
                "Admin.Customers.Customers.Fields.FirstName.Required",
                "Admin.Customers.Customers.Fields.LastName.Required",
                "Admin.Customers.Customers.Fields.SystemName",
                "Admin.Customers.Customers.Fields.SystemName.Hint",
                "Admin.Customers.Customers.Fields.Username.Required",
                "Admin.Customers.Customers.RewardPoints.Alert.HistoryAdd",
                "Admin.DT.Processing",
                "Admin.NopCommerceNews.HideAdv",
                "Admin.NopCommerceNews.ShowAdv",
                "Admin.Orders.OrderNotes.Alert.Add",
                "Admin.Promotions.Discounts.Fields.AppliedToCategories",
                "Admin.Promotions.Discounts.Fields.AppliedToCategories.Hint",
                "Admin.Promotions.Discounts.Fields.AppliedToCategories.NoRecords",
                "Admin.System.QueuedEmails.Fields.Priority.Required",
                "Common.DeleteConfirmationParam",
                "Common.Extensions.RelativeFormat",
                "Common.Home",
                "EUCookieLaw.CannotBrowse",
                "EUCookieLaw.Title",
                "Filtering.FilterResults",
                "Forum.Replies.Count",
                "Forum.Topics.Count",
                "News.Archive",
                "Newsletter.ResultAlreadyDeactivated",
                "PageTitle.EmailRevalidation",
                "PDFInvoice.CreatedOn",
                "PDFInvoice.Note",
                "PrivateMessages.Send.Subject.Required",
                "PrivateMessages.Sent.DateColumn",
                "PrivateMessages.Sent.DeleteSelected",
                "PrivateMessages.Sent.SubjectColumn",
                "PrivateMessages.Sent.ToColumn",
                "Profile.FullName",
                "RewardPoints.Message.Expired",
                "ShoppingCart.AddToWishlist.Update",
                "ShoppingCart.UpdateCartItem",
                "Tax.SelectType",
            });

            localizationService.AddLocaleResource(new Dictionary<string, string>
            {
                ["Admin.System.Warnings.PluginNotEnabled.AutoFixAndRestart"] = "Uninstall and delete all not used plugins automatically (site will be restarted)",
                ["ActivityLog.AddNewSpecAttributeGroup"] = "Added a new specification attribute group ('{0}')",
                ["ActivityLog.EditSpecAttributeGroup"] = "Edited a specification attribute group ('{0}')",
                ["ActivityLog.DeleteSpecAttributeGroup"] = "Deleted a specification attribute group ('{0}')",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Buttons.AddNew"] = "Add attribute",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Buttons.DeleteSelected"] = "Delete attributes (selected)",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.SpecificationAttributeGroup"] = "Group",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.SpecificationAttributeGroup.Hint"] = "The group of the specification attribute.",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.SpecificationAttributeGroup.None"] = "None",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Added"] = "The new attribute group has been added successfully.",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.AddNew"] = "Add a new specification attribute group",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.BackToList"] = "back to specification attribute list",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Buttons.AddNew"] = "Add group",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.DefaultGroupName"] = "Default group (non-grouped specification attributes)",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Deleted"] = "The attribute group has been deleted successfully.",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.EditAttributeGroupDetails"] = "Edit specification attribute group details",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.DisplayOrder"] = "Display order",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.DisplayOrder.Hint"] = "The display order of the specification attribute group.",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.Name"] = "Name",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.Name.Hint"] = "The name of the specification attribute group.",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.Name.Required"] = "Please provide a name.",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Info"] = "Attribute group info",
                ["Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Updated"] = "The attribute group has been updated successfully.",
                ["Admin.Catalog.Products.SpecificationAttributes.NameFormat"] = "{0} >> {1}",
            });

            // rename locales
            var localesToRename = new[] {
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Added", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Added" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.AddNew", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.AddNew" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.BackToList", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.BackToList" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Deleted", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Deleted" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.EditAttributeDetails", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.EditAttributeDetails" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Fields.DisplayOrder", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.DisplayOrder" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Fields.DisplayOrder.Hint", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.DisplayOrder.Hint" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Fields.Name", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.Name" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Fields.Name.Hint", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.Name.Hint" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Fields.Name.Required", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.Name.Required" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Info", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Info" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Options", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Options.AddNew", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.AddNew" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Options.EditOptionDetails", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.EditOptionDetails" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.ColorSquaresRgb", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.ColorSquaresRgb" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.ColorSquaresRgb.Hint", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.ColorSquaresRgb.Hint" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.DisplayOrder", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.DisplayOrder" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.DisplayOrder.Hint", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.DisplayOrder.Hint" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.EnableColorSquaresRgb", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.EnableColorSquaresRgb" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.EnableColorSquaresRgb.Hint", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.EnableColorSquaresRgb.Hint" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.Name", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.Name" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.Name.Hint", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.Name.Hint" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.Name.Required", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.Name.Required" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.NumberOfAssociatedProducts", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.NumberOfAssociatedProducts" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Options.SaveBeforeEdit", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.SaveBeforeEdit" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.Updated", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Updated" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.UsedByProducts", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.UsedByProducts" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.UsedByProducts.Product", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.UsedByProducts.Product" },
                new { Name = "Admin.Catalog.Attributes.SpecificationAttributes.UsedByProducts.Published", NewName = "Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.UsedByProducts.Published" },
            };

            var languageService = EngineContext.Current.Resolve<ILanguageService>();

            foreach (var lang in languageService.GetAllLanguages(true))
            {
                foreach (var locale in localesToRename)
                {
                    var lsr = localizationService.GetLocaleStringResourceByName(locale.Name, lang.Id, false);
                    if (lsr != null)
                    {
                        lsr.ResourceName = locale.NewName;
                        localizationService.UpdateLocaleStringResource(lsr);
                    }
                }
            }
        }

        /// <summary>Collects the DOWN migration expressions</summary>
        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
