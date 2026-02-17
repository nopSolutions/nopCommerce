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

        //#7336
        this.SetSettingIfNotExists<PrivateMessageSettings, bool>(settings => settings.AllowPrivateMessages, 
            this.GetSettingByKey($"ForumSettings.{nameof(PrivateMessageSettings.AllowPrivateMessages)}", false));

        this.SetSettingIfNotExists<PrivateMessageSettings, bool>(settings => settings.NotifyAboutPrivateMessages,
            this.GetSettingByKey($"ForumSettings.{nameof(PrivateMessageSettings.NotifyAboutPrivateMessages)}", false));

        this.SetSettingIfNotExists<PrivateMessageSettings, bool>(settings => settings.ShowAlertForPM,
            this.GetSettingByKey($"ForumSettings.{nameof(PrivateMessageSettings.ShowAlertForPM)}", false));

        this.SetSettingIfNotExists<PrivateMessageSettings, int>(settings => settings.PMSubjectMaxLength,
            this.GetSettingByKey($"ForumSettings.{nameof(PrivateMessageSettings.PMSubjectMaxLength)}", 450));

        this.SetSettingIfNotExists<PrivateMessageSettings, int>(settings => settings.PMTextMaxLength,
            this.GetSettingByKey($"ForumSettings.{nameof(PrivateMessageSettings.PMTextMaxLength)}", 4000));

        this.SetSettingIfNotExists<PrivateMessageSettings, int>(settings => settings.PrivateMessagesPageSize,
            this.GetSettingByKey($"ForumSettings.{nameof(PrivateMessageSettings.PrivateMessagesPageSize)}", 10));
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
