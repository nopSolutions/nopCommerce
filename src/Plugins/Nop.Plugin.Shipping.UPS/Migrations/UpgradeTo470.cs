using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Shipping.UPS.Migrations;

[NopMigration("2023-12-13 20:00:00", "Shipping.UPS Update to v2.0", MigrationProcessType.Update)]
public class UpgradeTo470 : Migration
{
    public override void Up()
    {
        this.DeleteLocaleResources(new[]
        {
            "Plugins.Shipping.UPS.Fields.Password",
            "Plugins.Shipping.UPS.Fields.Password.Hint",
            "Plugins.Shipping.UPS.Fields.Username",
            "Plugins.Shipping.UPS.Fields.Username.Hint"
        });

        this.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Plugins.Shipping.UPS.Fields.ClientId"] = "Client ID",
            ["Plugins.Shipping.UPS.Fields.ClientId.Hint"] = "Specify UPS client ID.",
            ["Plugins.Shipping.UPS.Fields.ClientSecret"] = "Client secret",
            ["Plugins.Shipping.UPS.Fields.ClientSecret.Hint"] = "Specify UPS client secret.",
            ["Plugins.Shipping.UPS.Fields.Tracing.Hint"] = "Check if you want to record plugin tracing in System Log. Warning: The entire request and response will be logged (including Client Id/secret, AccountNumber). Do not leave this enabled in a production environment."
        });

        this.SetSettingIfNotExists<UPSSettings, int?>(settings => settings.RequestTimeout, UPSDefaults.RequestTimeout);
        this.DeleteSettingsByNames(["upssettings.accesskey", "upssettings.username", "upssettings.password"]);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}