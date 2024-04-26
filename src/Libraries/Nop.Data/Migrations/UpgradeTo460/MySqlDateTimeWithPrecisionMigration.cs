using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.UpgradeTo460;

[NopSchemaMigration("2023-07-28 08:00:00", "Update datetime type precision")]
public class MySqlDateTimeWithPrecisionMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        var dataSettings = DataSettingsManager.LoadSettings();

        //update the types only in MySql 
        if (dataSettings.DataProvider != DataProviderType.MySql)
            return;

        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ActivityLog)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ActivityLog), nameof(ActivityLog.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Address)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Address), nameof(Address.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(BackInStockSubscription)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(BackInStockSubscription), nameof(BackInStockSubscription.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(BlogComment)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(BlogComment), nameof(BlogComment.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(BlogPost)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(BlogPost), nameof(BlogPost.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(BlogPost)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(BlogPost), nameof(BlogPost.EndDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(BlogPost)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(BlogPost), nameof(BlogPost.StartDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Campaign)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Campaign), nameof(Campaign.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Campaign)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Campaign), nameof(Campaign.DontSendBeforeDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Category)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Category), nameof(Category.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Category)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Category), nameof(Category.UpdatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Currency)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Currency), nameof(Currency.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Currency)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Currency), nameof(Currency.UpdatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Customer)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Customer), nameof(Customer.CannotLoginUntilDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Customer)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Customer), nameof(Customer.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Customer)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Customer), nameof(Customer.DateOfBirth)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Customer)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Customer), nameof(Customer.LastActivityDateUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Customer)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Customer), nameof(Customer.LastLoginDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(CustomerPassword)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(CustomerPassword), nameof(CustomerPassword.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Discount)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Discount), nameof(Discount.EndDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Discount)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Discount), nameof(Discount.StartDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(DiscountUsageHistory)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(DiscountUsageHistory), nameof(DiscountUsageHistory.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Forum)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Forum), nameof(Forum.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Forum)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Forum), nameof(Forum.LastPostTime)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Forum)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Forum), nameof(Forum.UpdatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumGroup)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumGroup), nameof(ForumGroup.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumGroup)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumGroup), nameof(ForumGroup.UpdatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumPost)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumPost), nameof(ForumPost.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumPost)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumPost), nameof(ForumPost.UpdatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumPostVote)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumPostVote), nameof(ForumPostVote.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(PrivateMessage)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(PrivateMessage), nameof(PrivateMessage.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumSubscription)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumSubscription), nameof(ForumSubscription.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumTopic)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumTopic), nameof(ForumTopic.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumTopic)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumTopic), nameof(ForumTopic.LastPostTime)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumTopic)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ForumTopic), nameof(ForumTopic.UpdatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(GdprLog)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(GdprLog), nameof(GdprLog.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(GenericAttribute)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(GenericAttribute), nameof(GenericAttribute.CreatedOrUpdatedDateUTC)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(GiftCard)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(GiftCard), nameof(GiftCard.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(GiftCardUsageHistory)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(GiftCardUsageHistory), nameof(GiftCardUsageHistory.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Log)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Log), nameof(Log.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Manufacturer)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Manufacturer), nameof(Manufacturer.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Manufacturer)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Manufacturer), nameof(Manufacturer.UpdatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(MigrationVersionInfo)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(MigrationVersionInfo), nameof(MigrationVersionInfo.AppliedOn)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(NewsItem)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(NewsItem), nameof(NewsItem.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(NewsItem)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(NewsItem), nameof(NewsItem.EndDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(NewsItem)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(NewsItem), nameof(NewsItem.StartDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(NewsComment)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(NewsComment), nameof(NewsComment.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(NewsLetterSubscription)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(NewsLetterSubscription), nameof(NewsLetterSubscription.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Order)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Order), nameof(Order.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Order)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Order), nameof(Order.PaidDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(OrderItem)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(OrderItem), nameof(OrderItem.RentalEndDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(OrderItem)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(OrderItem), nameof(OrderItem.RentalStartDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(OrderNote)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(OrderNote), nameof(OrderNote.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Poll)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Poll), nameof(Poll.EndDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Poll)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Poll), nameof(Poll.StartDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(PollVotingRecord)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(PollVotingRecord), nameof(PollVotingRecord.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Product)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Product), nameof(Product.AvailableEndDateTimeUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Product)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Product), nameof(Product.AvailableStartDateTimeUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Product)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Product), nameof(Product.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Product)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Product), nameof(Product.MarkAsNewEndDateTimeUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Product)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Product), nameof(Product.MarkAsNewStartDateTimeUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Product)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Product), nameof(Product.PreOrderAvailabilityStartDateTimeUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Product)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Product), nameof(Product.UpdatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ProductReview)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ProductReview), nameof(ProductReview.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(QueuedEmail)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(QueuedEmail), nameof(QueuedEmail.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(QueuedEmail)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(QueuedEmail), nameof(QueuedEmail.DontSendBeforeDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(QueuedEmail)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(QueuedEmail), nameof(QueuedEmail.SentOnUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(RecurringPayment)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(RecurringPayment), nameof(RecurringPayment.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(RecurringPayment)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(RecurringPayment), nameof(RecurringPayment.StartDateUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(RecurringPaymentHistory)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(RecurringPaymentHistory), nameof(RecurringPaymentHistory.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ReturnRequest)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ReturnRequest), nameof(ReturnRequest.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ReturnRequest)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ReturnRequest), nameof(ReturnRequest.UpdatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(RewardPointsHistory)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(RewardPointsHistory), nameof(RewardPointsHistory.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(RewardPointsHistory)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(RewardPointsHistory), nameof(RewardPointsHistory.EndDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ScheduleTask)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ScheduleTask), nameof(ScheduleTask.LastEnabledUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ScheduleTask)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ScheduleTask), nameof(ScheduleTask.LastEndUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ScheduleTask)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ScheduleTask), nameof(ScheduleTask.LastStartUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ScheduleTask)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ScheduleTask), nameof(ScheduleTask.LastSuccessUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Shipment)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Shipment), nameof(Shipment.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Shipment)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Shipment), nameof(Shipment.DeliveryDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Shipment)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Shipment), nameof(Shipment.ReadyForPickupDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(Shipment)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(Shipment), nameof(Shipment.ShippedDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ShoppingCartItem)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ShoppingCartItem), nameof(ShoppingCartItem.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ShoppingCartItem)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ShoppingCartItem), nameof(ShoppingCartItem.RentalEndDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ShoppingCartItem)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ShoppingCartItem), nameof(ShoppingCartItem.RentalStartDateUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ShoppingCartItem)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(ShoppingCartItem), nameof(ShoppingCartItem.UpdatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(StockQuantityHistory)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(StockQuantityHistory), nameof(StockQuantityHistory.CreatedOnUtc)))
            .AsCustom("datetime(6)");
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(TierPrice)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(TierPrice), nameof(TierPrice.EndDateTimeUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(TierPrice)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(TierPrice), nameof(TierPrice.StartDateTimeUtc)))
            .AsCustom("datetime(6)")
            .Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(VendorNote)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(VendorNote), nameof(VendorNote.CreatedOnUtc)))
            .AsCustom("datetime(6)");
    }
}