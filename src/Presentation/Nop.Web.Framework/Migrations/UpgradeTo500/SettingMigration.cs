using FluentMigrator;
using Nop.Core.Domain;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
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

        //#7386
        this.SetSettingIfNotExists<ShippingSettings, bool>(settings => settings.AllowCustomerToChooseDeliveryDate, true);
        this.SetSettingIfNotExists<ShippingSettings, int>(settings => settings.DeliveryDateRangeDays, 7);

        //#8097
        this.SetSettingIfNotExists<StoreInformationSettings, string>(settings => settings.XLink, setting =>
        {
            var twitterLink = this.GetSettingByKey<string>("storeinformationsettings.twitterlink");
            if (!string.IsNullOrEmpty(twitterLink))
            {
                setting.XLink = twitterLink;
            }
            else
            {
                setting.XLink = "https://x.com/nopCommerce";
            }
            this.DeleteSettingsByNames(["storeinformationsettings.twitterlink"]);
        });

        //#8136
        this.SetSettingIfNotExists<StoreInformationSettings, string>(settings => settings.TikTokLink, string.Empty);
        this.SetSettingIfNotExists<StoreInformationSettings, string>(settings => settings.SnapchatLink, string.Empty);
        this.SetSettingIfNotExists<StoreInformationSettings, string>(settings => settings.PinterestLink, string.Empty);
        this.SetSettingIfNotExists<StoreInformationSettings, string>(settings => settings.TumblrLink, string.Empty);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
