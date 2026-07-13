using FluentMigrator;
using Nop.Core.Domain;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.PriceLists;
using Nop.Core.Domain.Reminders;
using Nop.Core.Domain.Security;
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

        this.DeleteSettingsByNames([$"{nameof(CommonSettings)}.BbcodeEditorOpenLinksInNewWindow"]);

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

        //#8069
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.ShowSearchTermHistory, true);
        this.SetSettingIfNotExists<CatalogSettings, int>(settings => settings.NumberOfSearchTermHistoryItems, 10);

        //#7743
        this.SetSettingIfNotExists<ReminderSettings, bool>(settings => settings.AbandonedCartEnabled, false);
        this.SetSettingIfNotExists<ReminderSettings, bool>(settings => settings.PendingOrdersEnabled, false);
        this.SetSettingIfNotExists<ReminderSettings, bool>(settings => settings.IncompleteRegistrationEnabled, false);
        this.SetSettingIfNotExists<ReminderSettings, DateTime?>(settings => settings.ProcessingStartDateUtc, DateTime.UtcNow);

        //#8120
        this.SetSettingIfNotExists<OrderSettings, bool>(settings => settings.AutoCancelEnabled, false);
        this.SetSettingIfNotExists<OrderSettings, int>(settings => settings.AutoCancelDelay, 48 * 60);
        this.SetSettingIfNotExists<OrderSettings, List<string>>(settings => settings.AutoCancelIgnoredPaymentMethods, []);
        this.SetSettingIfNotExists<OrderSettings, bool>(settings => settings.AutoCancelRestoreShoppingCart, false);
        this.SetSettingIfNotExists<OrderSettings, DateTime?>(settings => settings.AutoCancelIgnoreBeforeUtc, DateTime.UtcNow);

        //#2430
        this.SetSettingIfNotExists<OtpSettings, bool>(settings => settings.LoginByPhoneEnabled, false);
        this.SetSettingIfNotExists<OtpSettings, int>(settings => settings.OtpTimeLife, 30);
        this.SetSettingIfNotExists<OtpSettings, int>(settings => settings.OtpCountAttemptsToSendCode, 3);
        this.SetSettingIfNotExists<OtpSettings, int>(settings => settings.OtpTimeToRepeat, 15);
        this.SetSettingIfNotExists<OtpSettings, int>(settings => settings.OtpLength, 6);
        this.SetSettingIfNotExists<MessagesSettings, string>(settings => settings.ActiveSmsProviderSystemName, "");

        //#4279
        this.SetSettingIfNotExists<MediaSettings, bool>(settings => settings.Object3dCameraControlEnabled, true);
        this.SetSettingIfNotExists<MediaSettings, bool>(settings => settings.Object3dZoomEnabled, true);
        this.SetSettingIfNotExists<MediaSettings, bool>(settings => settings.Object3dAutoRotateEnabled, false);
        this.SetSettingIfNotExists<MediaSettings, bool>(settings => settings.Object3dLazyLoadingEnabled, true);
        this.SetSettingIfNotExists<MediaSettings, int>(settings => settings.Object3dUploadSizeLimit, 20);

        //#8098
        this.SetSettingIfNotExists<CatalogSettings, PriceListStrategy>(settings => settings.PriceListStrategy, PriceListStrategy.MinimalPrice);

        //#8161
        this.SetSettingIfNotExists<ReturnRequestSettings, string>(settings => settings.ReturnRequestNumberMask,
            this.GetSettingByKey($"OrderSettings.{nameof(ReturnRequestSettings.ReturnRequestNumberMask)}", "{ID}"));

        this.SetSettingIfNotExists<ReturnRequestSettings, bool>(settings => settings.ReturnRequestsEnabled,
            this.GetSettingByKey($"OrderSettings.{nameof(ReturnRequestSettings.ReturnRequestsEnabled)}", true));

        this.SetSettingIfNotExists<ReturnRequestSettings, bool>(settings => settings.ReturnRequestsAllowFiles,
            this.GetSettingByKey($"OrderSettings.{nameof(ReturnRequestSettings.ReturnRequestsAllowFiles)}", false));

        this.SetSettingIfNotExists<ReturnRequestSettings, int>(settings => settings.NumberOfDaysReturnRequestAvailable,
            this.GetSettingByKey($"OrderSettings.{nameof(ReturnRequestSettings.NumberOfDaysReturnRequestAvailable)}", 365));

        this.SetSettingIfNotExists<ReturnRequestSettings, int>(settings => settings.ReturnRequestsFileMaximumSize,
            this.GetSettingByKey($"OrderSettings.{nameof(ReturnRequestSettings.ReturnRequestsFileMaximumSize)}", 2048));

        this.SetSettingIfNotExists<ReturnRequestSettings, bool>(settings => settings.UseEuWithdrawalLocales, false);
        this.SetSettingIfNotExists<ReturnRequestSettings, bool>(settings => settings.GuestReturnRequestsAllowed, false);
        this.SetSettingIfNotExists<ReturnRequestSettings, bool>(settings => settings.ReturnReasonsEnabled, true);
        this.SetSettingIfNotExists<ReturnRequestSettings, bool>(settings => settings.ReturnActionsEnabled, true);
        this.SetSettingIfNotExists<ReturnRequestSettings, int>(settings => settings.WithdrawalLinkDaysValid, 7);
        this.SetSettingIfNotExists<CaptchaSettings, bool>(settings => settings.ShowOnWithdrawalForm, false);

        //#309
        this.SetSettingIfNotExists<OrderSettings, int>(settings => settings.NextRecurringPaymentNotificationDays, 1);

        //#8093
        this.DeleteSettingsByNames([
            $"{nameof(CustomerSettings)}.PhoneNumberValidationUseRegex",
            $"{nameof(CustomerSettings)}.PhoneNumberValidationRule"
        ]);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
