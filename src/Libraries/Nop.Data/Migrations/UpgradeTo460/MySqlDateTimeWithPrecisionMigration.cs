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
using Nop.Data.Extensions;

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

        Alter.AlterColumnFor<Address>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<BackInStockSubscription>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<BlogComment>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<BlogPost>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<BlogPost>(t => t.EndDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<BlogPost>(t => t.StartDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<Campaign>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Campaign>(t => t.DontSendBeforeDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<Category>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Category>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Currency>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Currency>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Customer>(t => t.CannotLoginUntilDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<Customer>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Customer>(t => t.DateOfBirth)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<Customer>(t => t.LastActivityDateUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Customer>(t => t.LastLoginDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<CustomerPassword>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Discount>(t => t.EndDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<Discount>(t => t.StartDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<DiscountUsageHistory>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Forum>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Forum>(t => t.LastPostTime)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<Forum>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<ForumGroup>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<ForumGroup>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<ForumPost>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<ForumPost>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<ForumPostVote>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<PrivateMessage>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<ForumSubscription>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<ForumTopic>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<ForumTopic>(t => t.LastPostTime)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<ForumTopic>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<GdprLog>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<GenericAttribute>(t => t.CreatedOrUpdatedDateUTC)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<GiftCard>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<GiftCardUsageHistory>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Log>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Manufacturer>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Manufacturer>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<MigrationVersionInfo>(t => t.AppliedOn)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<NewsItem>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<NewsItem>(t => t.EndDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<NewsItem>(t => t.StartDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<NewsComment>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<NewsLetterSubscription>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Order>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Order>(t => t.PaidDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<OrderItem>(t => t.RentalEndDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<OrderItem>(t => t.RentalStartDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<OrderNote>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Poll>(t => t.EndDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<Poll>(t => t.StartDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<PollVotingRecord>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Product>(t => t.AvailableEndDateTimeUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<Product>(t => t.AvailableStartDateTimeUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<Product>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Product>(t => t.MarkAsNewEndDateTimeUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<Product>(t => t.MarkAsNewStartDateTimeUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<Product>(t => t.PreOrderAvailabilityStartDateTimeUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<Product>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<ProductReview>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<QueuedEmail>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<QueuedEmail>(t => t.DontSendBeforeDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<QueuedEmail>(t => t.SentOnUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<RecurringPayment>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<RecurringPayment>(t => t.StartDateUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<RecurringPaymentHistory>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<ReturnRequest>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<ReturnRequest>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<RewardPointsHistory>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<RewardPointsHistory>(t => t.EndDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<ScheduleTask>(t => t.LastEnabledUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<ScheduleTask>(t => t.LastEndUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<ScheduleTask>(t => t.LastStartUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<ScheduleTask>(t => t.LastSuccessUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<Shipment>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<Shipment>(t => t.DeliveryDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<Shipment>(t => t.ReadyForPickupDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<Shipment>(t => t.ShippedDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<ShoppingCartItem>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<ShoppingCartItem>(t => t.RentalEndDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<ShoppingCartItem>(t => t.RentalStartDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<ShoppingCartItem>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<StockQuantityHistory>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        Alter.AlterColumnFor<TierPrice>(t => t.EndDateTimeUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<TierPrice>(t => t.StartDateTimeUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        Alter.AlterColumnFor<VendorNote>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

    }
}