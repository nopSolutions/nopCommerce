using FluentMigrator;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
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
        this.SetSettingIfNotExists<CustomerSettings, bool>(settings => settings.AllowPrivateMessages, 
            this.GetSettingByKey<bool>($"{typeof(ForumSettings).Name}.{nameof(CustomerSettings.AllowPrivateMessages)}", false));

        this.SetSettingIfNotExists<CustomerSettings, bool>(settings => settings.ShowAlertForPM,
            this.GetSettingByKey($"{typeof(ForumSettings).Name}.{nameof(CustomerSettings.ShowAlertForPM)}", false));

        this.SetSettingIfNotExists<CustomerSettings, int>(settings => settings.PMSubjectMaxLength,
            this.GetSettingByKey($"{typeof(ForumSettings).Name}.{nameof(CustomerSettings.PMSubjectMaxLength)}", 450));

        this.SetSettingIfNotExists<CustomerSettings, int>(settings => settings.PMTextMaxLength,
            this.GetSettingByKey($"{typeof(ForumSettings).Name}.{nameof(CustomerSettings.PMTextMaxLength)}", 4000));

        this.SetSettingIfNotExists<CustomerSettings, int>(settings => settings.PrivateMessagesPageSize,
            this.GetSettingByKey($"{typeof(ForumSettings).Name}.{nameof(CustomerSettings.PrivateMessagesPageSize)}", 10));
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
