using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo510;

[NopUpdateMigration("2026-10-28 00:00:00", "5.10", UpdateMigrationType.Localization)]
public class LocalizationMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        #region Delete locales

        this.DeleteLocaleResources(new List<string>
        {
            
        });

        #endregion

        #region Rename locales

        this.RenameLocales(new Dictionary<string, string>
        {
            
        });

        #endregion

        #region Add or update locales

        this.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            //#8273
            ["Admin.Configuration.Settings.Affiliate"] = "Affiliate settings",
            ["Enums.Nop.Core.Domain.Affiliates.AffiliateStorageStrategyType.NoOverwrites"] = "No overwrites",
            ["Enums.Nop.Core.Domain.Affiliates.AffiliateStorageStrategyType.AlwaysOverwrite"] = "Always overwrite",
            ["Enums.Nop.Core.Domain.Affiliates.AffiliateStorageStrategyType.OverwriteAfterOrder"] = "Overwrite after order",
            ["Admin.Configuration.Settings.Affiliate.BlockTitle.Common"] = "Common",
            ["Admin.Configuration.Settings.Affiliate.AffiliateStorageStrategy"] = "Affiliate storage strategy",
            ["Admin.Configuration.Settings.Affiliate.AffiliateStorageStrategy.Hint"] = "Select the affiliate storage strategy",
            ["Admin.Configuration.Settings.Affiliate.AllowCustomersToApplyForAffiliateAccount"] = "Allow customers to apply affiliate account",
            ["Admin.Configuration.Settings.Affiliate.AllowCustomersToApplyForAffiliateAccount.Hint"] = "Check to allow customers users to fill a form to become an affiliate partner. Then a store owner will have to manually approve it.",
            ["Admin.Configuration.Settings.Affiliate.CommissionAmount"] = "Commission amount",
            ["Admin.Configuration.Settings.Affiliate.CommissionAmount.Hint"] = "Set the default commission amount",
            ["Admin.Configuration.Settings.Affiliate.CommissionPercentage"] = "Commission percentage",
            ["Admin.Configuration.Settings.Affiliate.CommissionPercentage.Hint"] = "Set the default commission percentage",
            ["Admin.Configuration.Settings.Affiliate.HoldingPeriodInDays"] = "Holding period in days",
            ["Admin.Configuration.Settings.Affiliate.HoldingPeriodInDays.Hint"] = "Set the holding period in days; use 0 to disable the holding period",
            ["Admin.Configuration.Settings.Affiliate.UseDefaultCommissionIfNotSetOnCatalog"] = "Use default commission if not set on catalog",
            ["Admin.Configuration.Settings.Affiliate.UseDefaultCommissionIfNotSetOnCatalog.Hint"] = "Check to use the default commission if not set on catalog",
            ["Admin.Configuration.Settings.Affiliate.UsePercentage"] = "Use percentage",
            ["Admin.Configuration.Settings.Affiliate.UsePercentage.Hint"] = "Check to use percentage instead of amount for default commission",
            ["Affiliate.ApplyAccount.AlreadyApplied"] = "You already applied for a affiliate account. Please register as a new customer in order to apply for one more affiliate account.",
            ["Affiliate.ApplyAccount.FriendlyUrlName"] = "Friendly URL name",
            ["Affiliate.ApplyAccount.FriendlyUrlName.Hint"] = "Enter a friendly name for your affiliate URL",
            ["Affiliate.ApplyAccount.ExistingAddresses"] = "Address",
            ["Affiliate.ApplyAccount.Submitted"] = "Your application has been submitted successfully. You will be notified via email once your application is reviewed.",
            ["Affiliate.ApplyAccount.Button"] = "Submit",
            ["Affiliate.ApplyAccount.Fields.FriendlyUrlName.Required"] = "Friendly URL name is required.",
            ["Affiliate.ApplyAccount.Fields.FriendlyUrlName.Unique"] = "Friendly URL name must be unique.",
            ["Affiliate.ApplyAccount.Fields.FriendlyUrlName.Length"] = "Friendly URL name must be between 3 and 100 characters long.",
            ["PageTitle.Affiliate.Apply"] = "Apply for Affiliate Account",
            ["Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnApplyAffiliatePage"] = "Show on apply for affiliate account page",
            ["Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnApplyAffiliatePage.Hint"] = "Check to show CAPTCHA on apply for affiliate account page.",
            ["Admin.Configuration.Settings.ProductEditor.Affiliate"] = "Affiliate commission",
            ["Admin.Configuration.Settings.Affiliate.CustomerAffiliatePageSize"] = "Page size",
            ["Admin.Configuration.Settings.Affiliate.CustomerAffiliatePageSize.Hint"] = "Set the page size for customer affiliates e.g. '4' affiliates per page.",
            ["Enums.Nop.Core.Domain.Affiliates.CommissionStatus.Pending"] = "Pending",
            ["Enums.Nop.Core.Domain.Affiliates.CommissionStatus.Paid"] = "Paid",
            ["Admin.Affiliates.AssociatedCustomer.Choose"] = "Choose the customer to associate with this affiliate",
            ["PageTitle.AffiliateInfo"] = "Affiliate Info",
            ["Admin.Catalog.Products.SpecifyAffiliateCommission"] = "Specify affiliate commission",
            ["Admin.Catalog.Products.SpecifyAffiliateCommission.Hint"] = "Check to specify affiliate commission for this product",
            ["Admin.Catalog.Products.AffiliateCommissionAmount"] = "Commission amount",
            ["Admin.Catalog.Products.AffiliateCommissionAmount.Hint"] = "Enter the affiliate commission amount",
            ["Admin.Catalog.Products.AffiliateCommissionPercentage"] = "Commission percentage",
            ["Admin.Catalog.Products.AffiliateCommissionPercentage.Hint"] = "Enter the affiliate commission percentage",
            ["Admin.Catalog.Products.AffiliateUsePercentage"] = "Use percentage",
            ["Admin.Catalog.Products.AffiliateUsePercentage.Hint"] = "Check to use percentage instead of amount for affiliate commission",
            ["Admin.Catalog.Products.Affiliate"] = "Affiliate commission",
            ["Account.Affiliates"] = "Affiliates",
            ["Account.Affiliates.Period"] = "Reports from",
            ["Account.Affiliates.Status"] = "Affiliate status",
            ["Account.Affiliates.Status.Active"] = "Active",
            ["Account.Affiliates.Status.Disabled"] = "Disabled",
            ["Account.Affiliates.TotalAffiliatedCustomers"] = "Total number of affiliated customers",
            ["Account.Affiliates.TotalOrders"] = "Total number of orders",
            ["Account.Affiliates.OrdersCount"] = "Orders count",
            ["Account.Affiliates.Orders.CreatedOn"] = "Created on",
            ["Account.Affiliates.Orders.AffiliateCommission"] = "Commission",
            ["Account.Affiliates.Orders.CommissionStatus"] = "Commission status",
            ["Account.Affiliates.Orders.CommissionPaidOn"] = "Paid on",
            ["Account.Affiliates.TotalCommission"] = "Total commission",
            ["Account.Affiliates.PaidCommission"] = "Paid commission",
            ["Admin.Affiliates.Orders.AffiliateCommission"] = "Commission",
            ["Admin.Affiliates.Orders.CommissionStatus"] = "Commission status",
            ["Admin.Affiliates.Orders.CommissionPaidOn"] = "Paid on",
            ["Admin.Affiliates.Orders.MarkPaid"] = "Mark as paid",
            ["Admin.Affiliates.Commissions"] = "Affiliate commissions",
            ["Admin.Affiliates.Commissions.StartDate"] = "Start date",
            ["Admin.Affiliates.Commissions.StartDate.Hint"] = "The start date for the search.",
            ["Admin.Affiliates.Commissions.EndDate"] = "End date",
            ["Admin.Affiliates.Commissions.EndDate.Hint"] = "The end date for the search.",
            ["Admin.Affiliates.Commissions.CommissionStatus"] = "Commission status",
            ["Admin.Affiliates.Commissions.CommissionStatus.Hint"] = "Commission status for the search.",
            ["Admin.Affiliates.Commission.CommissionId"] = "#",
            ["Admin.Affiliates.Commission.CommissionStatus"] = "Commission status",
            ["Admin.Affiliates.Commission.PaidOn"] = "Paid on",
            ["Admin.Affiliates.Commission.CreateOn"] = "Create on",
            ["Admin.Affiliates.Commission.TotalCommissionAmount"] = "Total commission amount",
            ["Admin.Affiliates.Commission.AdminComment"] = "Admin comment",
            ["Admin.Affiliates.Commission.AdminComment.Hint"] = "Admin comment. For internal use.",
            ["Admin.Affiliates.Commission.AddNew"] = "Add new affiliate commission",
            ["Admin.Affiliates.Commission.Edit"] = "Edit affiliate commission",
            ["Admin.Affiliates.Commission.CreateOrUpdate.BackToAffiliate"] = "Back to affiliate page",
            ["Admin.Affiliates.Commission.CreateOrUpdate.AvailableOrders"] = "List of available orders to add into the commission",
            ["Admin.Affiliates.Commission.CreateOrUpdate.IncludedOrders"] = "List of orders which included into the commission",
            ["Admin.Affiliates.Commission.AddSelectedOrders"] = "Add selected",
            ["Admin.Affiliates.Commission.SaveForEdit"] = "Save commission before edit",
            ["Admin.Affiliates.Commission.SelectOrders"] = "Select orders",
            ["Admin.Affiliates.Commission.RemoveOrderFromCommission"] = "Remove order",
            ["Admin.Affiliates.Fields.AssociatedCustomerId"] = "Associated Customer",
            ["Admin.Affiliates.Fields.AssociatedCustomerId.Hint"] = "Select the customer associated with this affiliate",
            ["Admin.Affiliates.Fields.AssociatedCustomerId.Choose"] = "Choose customer",
            ["Admin.Affiliates.Fields.AssociatedCustomerId.Remove"] = "Remove customer",
            ["Literals.Nop.Core.Http.NopRouteNames.General.ApplyAffiliateAccount"] = "Apply for affiliate account",
            ["ActivityLog.DeleteAffiliateCommission"] = "Deleted an affiliate commission (ID = {0})",
            ["ActivityLog.EditAffiliateCommission"] = "Edited an affiliate commission (ID = {0})",
            ["ActivityLog.AddNewAffiliateCommission"] = "Added a new affiliate commission (ID = {0})",
            ["Admin.ContentManagement.MessageTemplates.Description.AffiliateAccountActive.CustomerNotification"] = "This message template is used when a new affiliate account is activated. The message is received by a customer.",
            ["Admin.ContentManagement.MessageTemplates.Description.AffiliateAccountApply.StoreOwnerNotification"] = "This message template is used when a new affiliate account is applied for. The message is received by a store owner.",
            ["Affiliate.ApplyAccount.FriendlyUrlName.Required"] = "Friendly URL name is required.",
            ["Admin.Affiliates.Fields.CreatedOn"] = "Creates on",
            ["Admin.Affiliates.Fields.CreatedOn.Hint"] = "The date and time when the affiliate was created.",
        });

        #endregion
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}