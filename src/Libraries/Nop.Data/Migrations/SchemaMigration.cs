using FluentMigrator;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tasks;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/01/31 11:24:16:2551771", "Nop.Data base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// <remarks>
        /// We use an explicit table creation order instead of an automatic one
        /// due to problems creating relationships between tables
        /// </remarks>
        /// </summary>
        public override void Up()
        {
            _migrationManager.BuildTable<AddressAttribute>(Create);
            _migrationManager.BuildTable<AddressAttributeValue>(Create);
            _migrationManager.BuildTable<GenericAttribute>(Create);
            _migrationManager.BuildTable<SearchTerm>(Create);
            _migrationManager.BuildTable<Country>(Create);
            _migrationManager.BuildTable<Currency>(Create);
            _migrationManager.BuildTable<MeasureDimension>(Create);
            _migrationManager.BuildTable<MeasureWeight>(Create);
            _migrationManager.BuildTable<StateProvince>(Create);
            _migrationManager.BuildTable<Address>(Create);
            _migrationManager.BuildTable<Affiliate>(Create);

            _migrationManager.BuildTable<CustomerAttribute>(Create);
            _migrationManager.BuildTable<CustomerAttributeValue>(Create);

            _migrationManager.BuildTable<Customer>(Create);
            _migrationManager.BuildTable<CustomerPassword>(Create);
            _migrationManager.BuildTable<CustomerAddressMapping>(Create);

            _migrationManager.BuildTable<CustomerRole>(Create);
            _migrationManager.BuildTable<CustomerCustomerRoleMapping>(Create);

            _migrationManager.BuildTable<ExternalAuthenticationRecord>(Create);

            _migrationManager.BuildTable<CheckoutAttribute>(Create);
            _migrationManager.BuildTable<CheckoutAttributeValue>(Create);

            _migrationManager.BuildTable<ReturnRequestAction>(Create);
            _migrationManager.BuildTable<ReturnRequest>(Create);
            _migrationManager.BuildTable<ReturnRequestReason>(Create);

            _migrationManager.BuildTable<ProductAttribute>(Create);
            _migrationManager.BuildTable<PredefinedProductAttributeValue>(Create);
            _migrationManager.BuildTable<ProductTag>(Create);

            _migrationManager.BuildTable<Product>(Create);
            _migrationManager.BuildTable<ProductTemplate>(Create);
            _migrationManager.BuildTable<BackInStockSubscription>(Create);
            _migrationManager.BuildTable<RelatedProduct>(Create);
            _migrationManager.BuildTable<ReviewType>(Create);
            _migrationManager.BuildTable<SpecificationAttribute>(Create);
            _migrationManager.BuildTable<ProductAttributeCombination>(Create);
            _migrationManager.BuildTable<ProductAttributeMapping>(Create);
            _migrationManager.BuildTable<ProductAttributeValue>(Create);

            _migrationManager.BuildTable<Order>(Create);
            _migrationManager.BuildTable<OrderItem>(Create);
            _migrationManager.BuildTable<RewardPointsHistory>(Create);

            _migrationManager.BuildTable<GiftCard>(Create);
            _migrationManager.BuildTable<GiftCardUsageHistory>(Create);

            _migrationManager.BuildTable<OrderNote>(Create);

            _migrationManager.BuildTable<RecurringPayment>(Create);
            _migrationManager.BuildTable<RecurringPaymentHistory>(Create);

            _migrationManager.BuildTable<ShoppingCartItem>(Create);

            _migrationManager.BuildTable<Store>(Create);
            _migrationManager.BuildTable<StoreMapping>(Create);

            _migrationManager.BuildTable<Language>(Create);
            _migrationManager.BuildTable<LocaleStringResource>(Create);
            _migrationManager.BuildTable<LocalizedProperty>(Create);

            _migrationManager.BuildTable<BlogPost>(Create);
            _migrationManager.BuildTable<BlogComment>(Create);

            _migrationManager.BuildTable<Category>(Create);
            _migrationManager.BuildTable<CategoryTemplate>(Create);

            _migrationManager.BuildTable<ProductCategory>(Create);

            _migrationManager.BuildTable<CrossSellProduct>(Create);
            _migrationManager.BuildTable<Manufacturer>(Create);
            _migrationManager.BuildTable<ManufacturerTemplate>(Create);

            _migrationManager.BuildTable<ProductManufacturer>(Create);
            _migrationManager.BuildTable<ProductProductTagMapping>(Create);
            _migrationManager.BuildTable<ProductReview>(Create);

            _migrationManager.BuildTable<ProductReviewHelpfulness>(Create);
            _migrationManager.BuildTable<ProductReviewReviewTypeMapping>(Create);

            _migrationManager.BuildTable<SpecificationAttributeOption>(Create);
            _migrationManager.BuildTable<ProductSpecificationAttribute>(Create);

            _migrationManager.BuildTable<TierPrice>(Create);

            _migrationManager.BuildTable<Warehouse>(Create);
            _migrationManager.BuildTable<DeliveryDate>(Create);
            _migrationManager.BuildTable<ProductAvailabilityRange>(Create);
            _migrationManager.BuildTable<Shipment>(Create);
            _migrationManager.BuildTable<ShipmentItem>(Create);
            _migrationManager.BuildTable<ShippingMethod>(Create);
            _migrationManager.BuildTable<ShippingMethodCountryMapping>(Create);

            _migrationManager.BuildTable<ProductWarehouseInventory>(Create);
            _migrationManager.BuildTable<StockQuantityHistory>(Create);

            _migrationManager.BuildTable<Download>(Create);
            _migrationManager.BuildTable<Picture>(Create);
            _migrationManager.BuildTable<PictureBinary>(Create);

            _migrationManager.BuildTable<ProductPicture>(Create);

            _migrationManager.BuildTable<Setting>(Create);

            _migrationManager.BuildTable<Discount>(Create);

            _migrationManager.BuildTable<DiscountCategoryMapping>(Create);
            _migrationManager.BuildTable<DiscountProductMapping>(Create);
            _migrationManager.BuildTable<DiscountRequirement>(Create);
            _migrationManager.BuildTable<DiscountUsageHistory>(Create);
            _migrationManager.BuildTable<DiscountManufacturerMapping>(Create);

            _migrationManager.BuildTable<PrivateMessage>(Create);
            _migrationManager.BuildTable<ForumGroup>(Create);
            _migrationManager.BuildTable<Forum>(Create);
            _migrationManager.BuildTable<ForumTopic>(Create);
            _migrationManager.BuildTable<ForumPost>(Create);
            _migrationManager.BuildTable<ForumPostVote>(Create);
            _migrationManager.BuildTable<ForumSubscription>(Create);

            _migrationManager.BuildTable<GdprConsent>(Create);
            _migrationManager.BuildTable<GdprLog>(Create);

            _migrationManager.BuildTable<ActivityLogType>(Create);
            _migrationManager.BuildTable<ActivityLog>(Create);
            _migrationManager.BuildTable<Log>(Create);

            _migrationManager.BuildTable<Campaign>(Create);
            _migrationManager.BuildTable<EmailAccount>(Create);
            _migrationManager.BuildTable<MessageTemplate>(Create);
            _migrationManager.BuildTable<NewsLetterSubscription>(Create);
            _migrationManager.BuildTable<QueuedEmail>(Create);

            _migrationManager.BuildTable<NewsItem>(Create);
            _migrationManager.BuildTable<NewsComment>(Create);

            _migrationManager.BuildTable<Poll>(Create);
            _migrationManager.BuildTable<PollAnswer>(Create);
            _migrationManager.BuildTable<PollVotingRecord>(Create);

            _migrationManager.BuildTable<AclRecord>(Create);
            _migrationManager.BuildTable<PermissionRecord>(Create);
            _migrationManager.BuildTable<PermissionRecordCustomerRoleMapping>(Create);

            _migrationManager.BuildTable<UrlRecord>(Create);

            _migrationManager.BuildTable<ScheduleTask>(Create);

            _migrationManager.BuildTable<TaxCategory>(Create);

            _migrationManager.BuildTable<TopicTemplate>(Create);
            _migrationManager.BuildTable<Topic>(Create);

            _migrationManager.BuildTable<Vendor>(Create);
            _migrationManager.BuildTable<VendorAttribute>(Create);
            _migrationManager.BuildTable<VendorAttributeValue>(Create);
            _migrationManager.BuildTable<VendorNote>(Create);
        }
    }
}
