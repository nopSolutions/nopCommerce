using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Plugin.Misc.RFQ.Data.Migrations;

[NopMigration("2025/11/01 18:41:53:1677556", "Misc.RFQ add the settings")]
public class AddSettings : Migration
{
    private readonly ISettingService _settingService;

    public AddSettings(ISettingService settingService)
    {
        _settingService = settingService;
    }

    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var rfqSettings = _settingService.LoadSetting<RfqSettings>();
        if (!_settingService.SettingExists(rfqSettings, settings => settings.AllowCustomerGenerateQuotePdf))
        {
            rfqSettings.AllowCustomerGenerateQuotePdf = false;
            _settingService.SaveSetting(rfqSettings, settings => settings.AllowCustomerGenerateQuotePdf);
        }
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        //nothing
    }
}