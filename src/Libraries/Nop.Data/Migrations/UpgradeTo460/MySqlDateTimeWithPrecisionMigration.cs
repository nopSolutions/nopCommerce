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
using Nop.Core.Domain.Orders;
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

        this.AddOrAlterColumnFor<ActivityLog>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Address>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<BackInStockSubscription>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<BlogComment>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<BlogPost>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<BlogPost>(t => t.EndDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<BlogPost>(t => t.StartDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<Campaign>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Campaign>(t => t.DontSendBeforeDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<Category>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Category>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Currency>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Currency>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Customer>(t => t.CannotLoginUntilDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Customer>(t => t.DateOfBirth)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.LastActivityDateUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Customer>(t => t.LastLoginDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<CustomerPassword>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Discount>(t => t.EndDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<Discount>(t => t.StartDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<DiscountUsageHistory>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Forum>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Forum>(t => t.LastPostTime)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<Forum>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<ForumGroup>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<ForumGroup>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<ForumPost>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<ForumPost>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<ForumPostVote>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<PrivateMessage>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<ForumSubscription>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<ForumTopic>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<ForumTopic>(t => t.LastPostTime)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<ForumTopic>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<GdprLog>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<GenericAttribute>(t => t.CreatedOrUpdatedDateUTC)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<GiftCard>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<GiftCardUsageHistory>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Log>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Manufacturer>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Manufacturer>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<MigrationVersionInfo>(t => t.AppliedOn)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<NewsLetterSubscription>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Order>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Order>(t => t.PaidDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<OrderItem>(t => t.RentalEndDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<OrderItem>(t => t.RentalStartDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<OrderNote>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Product>(t => t.AvailableEndDateTimeUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<Product>(t => t.AvailableStartDateTimeUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<Product>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Product>(t => t.MarkAsNewEndDateTimeUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<Product>(t => t.MarkAsNewStartDateTimeUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<Product>(t => t.PreOrderAvailabilityStartDateTimeUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<Product>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<ProductReview>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<QueuedEmail>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<QueuedEmail>(t => t.DontSendBeforeDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<QueuedEmail>(t => t.SentOnUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<RecurringPayment>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<RecurringPayment>(t => t.StartDateUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<RecurringPaymentHistory>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<ReturnRequest>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<ReturnRequest>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<RewardPointsHistory>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<RewardPointsHistory>(t => t.EndDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<ScheduleTask>(t => t.LastEnabledUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<ScheduleTask>(t => t.LastEndUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<ScheduleTask>(t => t.LastStartUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<ScheduleTask>(t => t.LastSuccessUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<Shipment>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<Shipment>(t => t.DeliveryDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<Shipment>(t => t.ReadyForPickupDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<Shipment>(t => t.ShippedDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<ShoppingCartItem>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<ShoppingCartItem>(t => t.RentalEndDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<ShoppingCartItem>(t => t.RentalStartDateUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<ShoppingCartItem>(t => t.UpdatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<StockQuantityHistory>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");

        this.AddOrAlterColumnFor<TierPrice>(t => t.EndDateTimeUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<TierPrice>(t => t.StartDateTimeUtc)
            .AsCustom("datetime(6)")
            .Nullable();

        this.AddOrAlterColumnFor<VendorNote>(t => t.CreatedOnUtc)
            .AsCustom("datetime(6)");
    }
}