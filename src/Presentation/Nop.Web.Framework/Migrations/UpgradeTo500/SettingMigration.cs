using FluentMigrator;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Reminders;
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

        //#7743
        this.SetSettingIfNotExists<ReminderSettings, bool>(settings => settings.AbandonedCartEnabled, false);
        this.SetSettingIfNotExists<ReminderSettings, bool>(settings => settings.PendingOrdersEnabled, false);
        this.SetSettingIfNotExists<ReminderSettings, bool>(settings => settings.IncompleteRegistrationEnabled, false);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
