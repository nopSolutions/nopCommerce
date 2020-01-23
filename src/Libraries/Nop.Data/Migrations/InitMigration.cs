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
using Nop.Data.Migrations.Builders;

namespace Nop.Data.Migrations
{
    [Migration(1)]
    public class InitMigration : BaseInitMigration
    {
        public override void Up()
        {
            BuildEntity<AddressAttribute>(builder: new AddressAttributeBuilder());
            BuildEntity<GenericAttribute>(builder: new GenericAttributeBuilder());
            BuildEntity<SearchTerm>();
            BuildEntity<Country>(builder: new CountryBuilder());
            BuildEntity<Currency>(builder: new CurrencyBuilder());
            BuildEntity<MeasureDimension>(builder: new MeasureDimensionBuilder());
            BuildEntity<MeasureWeight>(builder: new MeasureWeightBuilder());
            BuildEntity<StateProvince>(builder: new StateProvinceBuilder());
            BuildEntity<Address>(builder: new AddressBuilder());
            BuildEntity<Affiliate>(builder: new AffiliateBuilder());

            BuildEntity<CustomerAttribute>(builder: new CustomerAttributeBuilder());
            BuildEntity<CustomerAttributeValue>(builder: new CustomerAttributeValueBuilder());

            BuildEntity<Customer>(builder: new CustomerBuilder());
            BuildEntity<CustomerPassword>(builder: new CustomerPasswordBuilder());
            BuildEntity<CustomerAddressMapping>(builder: new CustomerAddressMappingBuilder());

            BuildEntity<CustomerRole>(builder: new CustomerRoleBuilder());
            BuildEntity<CustomerCustomerRoleMapping>(builder: new CustomerCustomerRoleMappingBuilder());

            BuildEntity<ExternalAuthenticationRecord>(builder: new ExternalAuthenticationRecordBuilder());

            BuildEntity<CheckoutAttribute>(builder: new CheckoutAttributeBuilder());
            BuildEntity<CheckoutAttributeValue>(builder: new CheckoutAttributeValueBuilder());

            BuildEntity<ReturnRequestAction>(builder: new ReturnRequestActionBuilder());
            BuildEntity<ReturnRequest>(builder: new ReturnRequestBuilder());
            BuildEntity<ReturnRequestReason>(builder: new ReturnRequestReasonBuilder());

            BuildEntity<ProductAttribute>(builder: new ProductAttributeBuilder());
            BuildEntity<PredefinedProductAttributeValue>(builder: new PredefinedProductAttributeValueBuilder());
            BuildEntity<ProductTag>(builder: new ProductTagBuilder());

            BuildEntity<Product>(builder: new ProductBuilder());
            BuildEntity<ProductTemplate>(builder: new ProductTemplateBuilder());
            BuildEntity<BackInStockSubscription>(builder: new BackInStockSubscriptionBuilder());
            BuildEntity<RelatedProduct>();
            BuildEntity<ReviewType>(builder: new ReviewTypeBuilder());
            BuildEntity<SpecificationAttribute>(builder: new SpecificationAttributeBuilder());
            BuildEntity<ProductAttributeCombination>(builder: new ProductAttributeCombinationBuilder());
            BuildEntity<ProductAttributeMapping>(builder: new ProductAttributeMappingBuilder());
            BuildEntity<ProductAttributeValue>(builder: new ProductAttributeValueBuilder());

            BuildEntity<Order>(builder: new OrderBuilder());
            BuildEntity<OrderItem>(builder: new OrderItemBuilder());
            BuildEntity<RewardPointsHistory>(builder: new RewardPointsHistoryBuilder());

            BuildEntity<GiftCard>(builder: new GiftCardBuilder());
            BuildEntity<GiftCardUsageHistory>(builder: new GiftCardUsageHistoryBuilder());

            BuildEntity<OrderNote>(builder: new OrderNoteBuilder());

            BuildEntity<RecurringPayment>(builder: new RecurringPaymentBuilder());
            BuildEntity<RecurringPaymentHistory>(builder: new RecurringPaymentHistoryBuilder());

            BuildEntity<ShoppingCartItem>(builder: new ShoppingCartItemBuilder());

            BuildEntity<Store>(builder: new StoreBuilder());
            BuildEntity<StoreMapping>(builder: new StoreMappingBuilder());

            BuildEntity<Language>(builder: new LanguageBuilder());
            BuildEntity<LocaleStringResource>(builder: new LocaleStringResourceBuilder());
            BuildEntity<LocalizedProperty>(builder: new LocalizedPropertyBuilder());

            BuildEntity<BlogPost>(builder: new BlogPostBuilder());
            BuildEntity<BlogComment>(builder: new BlogCommentBuilder());

            BuildEntity<Category>(builder: new CategoryBuilder());
            BuildEntity<CategoryTemplate>(builder: new CategoryTemplateBuilder());

            BuildEntity<ProductCategory>(builder: new ProductCategoryBuilder());

            BuildEntity<CrossSellProduct>();
            BuildEntity<Manufacturer>(builder: new ManufacturerBuilder());
            BuildEntity<ManufacturerTemplate>(builder: new ManufacturerTemplateBuilder());

            BuildEntity<ProductManufacturer>(builder: new ProductManufacturerBuilder());

            BuildEntity<ProductProductTagMapping>(builder: new ProductProductTagMappingBuilder());
            BuildEntity<ProductReview>(builder: new ProductReviewBuilder());

            BuildEntity<ProductReviewHelpfulness>(builder: new ProductReviewHelpfulnessBuilder());
            BuildEntity<ProductReviewReviewTypeMapping>(builder: new ProductReviewReviewTypeMappingBuilder());

            BuildEntity<SpecificationAttributeOption>(builder: new SpecificationAttributeOptionBuilder());
            BuildEntity<ProductSpecificationAttribute>(builder: new ProductSpecificationAttributeBuilder());

            BuildEntity<TierPrice>(builder: new TierPriceBuilder());

            BuildEntity<Warehouse>(builder: new WarehouseBuilder());
            BuildEntity<DeliveryDate>(builder: new DeliveryDateBuilder());
            BuildEntity<ProductAvailabilityRange>(builder: new ProductAvailabilityRangeBuilder());
            BuildEntity<Shipment>(builder: new ShipmentBuilder());
            BuildEntity<ShipmentItem>(builder: new ShipmentItemBuilder());
            BuildEntity<ShippingMethod>(builder: new ShippingMethodBuilder());
            BuildEntity<ShippingMethodCountryMapping>(builder: new ShippingMethodCountryMappingBuilder());

            BuildEntity<ProductWarehouseInventory>(builder: new ProductWarehouseInventoryBuilder());
            BuildEntity<StockQuantityHistory>(builder: new StockQuantityHistoryBuilder());

            BuildEntity<Download>();
            BuildEntity<Picture>(builder: new PictureBuilder());
            BuildEntity<PictureBinary>(builder: new PictureBinaryBuilder());

            BuildEntity<ProductPicture>(builder: new ProductPictureBuilder());

            BuildEntity<Setting>(builder: new SettingBuilder());

            BuildEntity<Discount>(builder: new DiscountBuilder());

            BuildEntity<DiscountCategoryMapping>(builder: new DiscountCategoryMappingBuilder());
            BuildEntity<DiscountProductMapping>(builder: new DiscountProductMappingBuilder());
            BuildEntity<DiscountRequirement>(builder: new DiscountRequirementBuilder());
            BuildEntity<DiscountUsageHistory>(builder: new DiscountUsageHistoryBuilder());

            BuildEntity<PrivateMessage>(builder: new PrivateMessageBuilder());
            BuildEntity<ForumGroup>(builder: new ForumGroupBuilder());
            BuildEntity<Forum>(builder: new ForumBuilder());
            BuildEntity<ForumTopic>(builder: new ForumTopicBuilder());
            BuildEntity<ForumPost>(builder: new ForumPostBuilder());
            BuildEntity<ForumPostVote>(builder: new ForumPostVoteBuilder());
            BuildEntity<ForumSubscription>(builder: new ForumSubscriptionBuilder());

            BuildEntity<GdprConsent>(builder: new GdprConsentBuilder());
            BuildEntity<GdprLog>(builder: new GdprLogBuilder());

            BuildEntity<ActivityLogType>(builder: new ActivityLogTypeBuilder());
            BuildEntity<ActivityLog>(builder: new ActivityLogBuilder());
            BuildEntity<Log>(builder: new LogBuilder());

            BuildEntity<Campaign>(builder: new CampaignBuilder());
            BuildEntity<EmailAccount>(builder: new EmailAccountBuilder());
            BuildEntity<MessageTemplate>(builder: new MessageTemplateBuilder());
            BuildEntity<NewsLetterSubscription>(builder: new NewsLetterSubscriptionBuilder());
            BuildEntity<QueuedEmail>(builder: new QueuedEmailBuilder());

            BuildEntity<NewsItem>(builder: new NewsItemBuilder());
            BuildEntity<NewsComment>(builder: new NewsCommentBuilder());

            BuildEntity<Poll>(builder: new PollBuilder());
            BuildEntity<PollAnswer>(builder: new PollAnswerBuilder());
            BuildEntity<PollVotingRecord>(builder: new PollVotingRecordBuilder());

            BuildEntity<AclRecord>(builder: new AclRecordBuilder());
            BuildEntity<PermissionRecord>(builder: new PermissionRecordBuilder());
            BuildEntity<PermissionRecordCustomerRoleMapping>(builder: new PermissionRecordCustomerRoleMappingBuilder());

            BuildEntity<UrlRecord>(builder: new UrlRecordBuilder());

            BuildEntity<ScheduleTask>(builder: new ScheduleTaskBuilder());

            BuildEntity<TaxCategory>(builder: new TaxCategoryBuilder());

            BuildEntity<TopicTemplate>(builder: new TopicTemplateBuilder());
            BuildEntity<Topic>(builder: new TopicBuilder());            

            BuildEntity<Vendor>(builder: new VendorBuilder());
            BuildEntity<VendorAttribute>(builder: new VendorAttributeBuilder());
            BuildEntity<VendorAttributeValue>(builder: new VendorAttributeValueBuilder());
            BuildEntity<VendorNote>(builder: new VendorNoteBuilder());
        }
    }
}
