using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo510;

[NopUpdateMigration("2026-04-11 00:00:01", "5.10", UpdateMigrationType.Localization)]
public class AutoCancelUnpaidOrdersLocalization : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //add localization resources for auto-cancel unpaid orders feature
        this.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Admin.Configuration.Settings.Order.AutoCancelUnpaidOrdersEnabled"] = "Enable auto-cancel unpaid orders",
            ["Admin.Configuration.Settings.Order.AutoCancelUnpaidOrdersEnabled.Hint"] = "Check to enable automatic cancellation of unpaid orders after a specified delay.",
            
            ["Admin.Configuration.Settings.Order.AutoCancelUnpaidOrdersDelay"] = "Auto-cancel delay (minutes)",
            ["Admin.Configuration.Settings.Order.AutoCancelUnpaidOrdersDelay.Hint"] = "Specify the delay in minutes after which unpaid orders should be automatically cancelled. Default is 600 minutes (10 hours).",
            
            ["Admin.Configuration.Settings.Order.IgnorePaymentMethods"] = "Ignore payment methods",
            ["Admin.Configuration.Settings.Order.IgnorePaymentMethods.Hint"] = "Enter comma-separated payment method system names to exclude from auto-cancellation (e.g., \"Payments.CheckMoneyOrder,Payments.Manual\"). Orders using these payment methods will not be automatically cancelled.",
            
            ["Admin.Configuration.Settings.Order.RestoreCartAfterCancellation"] = "Restore cart after cancellation",
            ["Admin.Configuration.Settings.Order.RestoreCartAfterCancellation.Hint"] = "Check to automatically restore shopping cart items when an order is cancelled by the auto-cancel task."
        });
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //nothing
    }
}
