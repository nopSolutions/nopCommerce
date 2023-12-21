using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Nop.Services.Localization;


namespace Nop.Plugin.Shipping.UPS.Migrations;

[NopMigration("2023-12-13 20:00:00", "Shipping.UPS Update to v2.0", MigrationProcessType.Update)]
public class UpgradeTo470 : Migration
{
    private ILocalizationService _localizationService;
    private ISettingService _settingService;

    public UpgradeTo470(ILocalizationService localizationService,
        ISettingService settingService)
    {
        _localizationService = localizationService;
        _settingService = settingService;
    }

    public override void Up()
    {
        _localizationService.DeleteLocaleResources(new[]
        {
            "Plugins.Shipping.UPS.Fields.Password",
            "Plugins.Shipping.UPS.Fields.Password.Hint",
            "Plugins.Shipping.UPS.Fields.Username",
            "Plugins.Shipping.UPS.Fields.Username.Hint"
        });

        _localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Plugins.Shipping.UPS.Fields.ClientId"] = "Client ID",
            ["Plugins.Shipping.UPS.Fields.ClientId.Hint"] = "Specify UPS client ID.",
            ["Plugins.Shipping.UPS.Fields.ClientSecret"] = "Client secret",
            ["Plugins.Shipping.UPS.Fields.ClientSecret.Hint"] = "Specify UPS client secret.",
            ["Plugins.Shipping.UPS.Fields.Tracing.Hint"] = "Check if you want to record plugin tracing in System Log. Warning: The entire request and response will be logged (including Client Id/secret, AccountNumber). Do not leave this enabled in a production environment."
        });

        var setting = _settingService.LoadSetting<UPSSettings>();
        if (!_settingService.SettingExists(setting, settings => settings.RequestTimeout))
        {
            setting.RequestTimeout = UPSDefaults.RequestTimeout;
            _settingService.SaveSetting(setting, settings => settings.RequestTimeout);
        }
            
        var accessKey = _settingService.GetSetting("upssettings.accesskey");
        if (accessKey is not null) 
            _settingService.DeleteSetting(accessKey);

        var username = _settingService.GetSetting("upssettings.username");
        if (username is not null)
            _settingService.DeleteSetting(username);

        var password = _settingService.GetSetting("upssettings.password");
        if (password is not null)
            _settingService.DeleteSetting(password);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}