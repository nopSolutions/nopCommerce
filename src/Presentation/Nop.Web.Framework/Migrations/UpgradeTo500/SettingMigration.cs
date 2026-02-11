using FluentMigrator;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo500;

[NopUpdateMigration("2025-10-27 00:00:00", "5.00", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;
        
        //#7898
        this.SetSettingIfNotExists<ArtificialIntelligenceSettings, bool>(settings => settings.LogRequests, false);

        //#2430
        this.SetSettingIfNotExists<OtpSettings, bool>(settings => settings.LoginByPhoneEnabled, false);
        this.SetSettingIfNotExists<OtpSettings, int>(settings => settings.OtpTimeLife, 30);
        this.SetSettingIfNotExists<OtpSettings, int>(settings => settings.OtpCountAttemptsToSendCode, 3);
        this.SetSettingIfNotExists<OtpSettings, int>(settings => settings.OtpTimeToRepeat, 15);
        this.SetSettingIfNotExists<OtpSettings, int>(settings => settings.OtpLength, 6);
        this.SetSettingIfNotExists<OtpSettings, string>(settings => settings.ActiveSmsProviderSystemName, "Sms.Twilio");
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
