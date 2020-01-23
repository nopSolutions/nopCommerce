using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public class SchemeBuilder
    {
        private readonly IMigrationContext _context;
        private readonly ConcurrentDictionary<Type, IMigrationExpression> _cache = new ConcurrentDictionary<Type, IMigrationExpression>();

        public SchemeBuilder(IMigrationContext context)
        {
            _context = context;
            Init();
        }

        public void Init()
        {
            BuildEntity(new AddressAttributeBuilder());
            BuildEntity(new GenericAttributeBuilder());
            BuildEntity<SearchTerm>();
            BuildEntity(new CountryBuilder());
            BuildEntity(new CurrencyBuilder());
            BuildEntity(new MeasureDimensionBuilder());
            BuildEntity(new MeasureWeightBuilder());
            BuildEntity(new StateProvinceBuilder());
            BuildEntity(new AddressBuilder());
            BuildEntity(new AffiliateBuilder());
            BuildEntity(new CustomerAttributeBuilder());
            BuildEntity(new CustomerAttributeValueBuilder());
            BuildEntity(new CustomerBuilder());
            BuildEntity(new CustomerPasswordBuilder());
            BuildEntity(new CustomerAddressMappingBuilder());
            BuildEntity(new CustomerRoleBuilder());
            BuildEntity(new CustomerCustomerRoleMappingBuilder());
            BuildEntity(new ExternalAuthenticationRecordBuilder());
            BuildEntity(new CheckoutAttributeBuilder());
            BuildEntity(new CheckoutAttributeValueBuilder());
            BuildEntity(new ReturnRequestActionBuilder());
            BuildEntity(new ReturnRequestBuilder());
            BuildEntity(new ReturnRequestReasonBuilder());
            BuildEntity(new ProductAttributeBuilder());
            BuildEntity(new PredefinedProductAttributeValueBuilder());
            BuildEntity(new ProductTagBuilder());
            BuildEntity(new ProductBuilder());
            BuildEntity(new ProductTemplateBuilder());
            BuildEntity(new BackInStockSubscriptionBuilder());
            BuildEntity<RelatedProduct>();
            BuildEntity(new ReviewTypeBuilder());
            BuildEntity(new SpecificationAttributeBuilder());
            BuildEntity(new ProductAttributeCombinationBuilder());
            BuildEntity(new ProductAttributeMappingBuilder());
            BuildEntity(new ProductAttributeValueBuilder());
            BuildEntity(new OrderBuilder());
            BuildEntity(new OrderItemBuilder());
            BuildEntity(new RewardPointsHistoryBuilder());
            BuildEntity(new GiftCardBuilder());
            BuildEntity(new GiftCardUsageHistoryBuilder());
            BuildEntity(new OrderNoteBuilder());
            BuildEntity(new RecurringPaymentBuilder());
            BuildEntity(new RecurringPaymentHistoryBuilder());
            BuildEntity(new ShoppingCartItemBuilder());
            BuildEntity(new StoreBuilder());
            BuildEntity(new StoreMappingBuilder());
            BuildEntity(new LanguageBuilder());
            BuildEntity(new LocaleStringResourceBuilder());
            BuildEntity(new LocalizedPropertyBuilder());
            BuildEntity(new BlogPostBuilder());
            BuildEntity(new BlogCommentBuilder());
            BuildEntity(new CategoryBuilder());
            BuildEntity(new CategoryTemplateBuilder());
            BuildEntity(new ProductCategoryBuilder());
            BuildEntity<CrossSellProduct>();
            BuildEntity(new ManufacturerBuilder());
            BuildEntity(new ManufacturerTemplateBuilder());
            BuildEntity(new ProductManufacturerBuilder());
            BuildEntity(new ProductProductTagMappingBuilder());
            BuildEntity(new ProductReviewBuilder());
            BuildEntity(new ProductReviewHelpfulnessBuilder());
            BuildEntity(new ProductReviewReviewTypeMappingBuilder());
            BuildEntity(new SpecificationAttributeOptionBuilder());
            BuildEntity(new ProductSpecificationAttributeBuilder());
            BuildEntity(new TierPriceBuilder());
            BuildEntity(new WarehouseBuilder());
            BuildEntity(new DeliveryDateBuilder());
            BuildEntity(new ProductAvailabilityRangeBuilder());
            BuildEntity(new ShipmentBuilder());
            BuildEntity(new ShipmentItemBuilder());
            BuildEntity(new ShippingMethodBuilder());
            BuildEntity(new ShippingMethodCountryMappingBuilder());
            BuildEntity(new ProductWarehouseInventoryBuilder());
            BuildEntity(new StockQuantityHistoryBuilder());
            BuildEntity<Download>();
            BuildEntity(new PictureBuilder());
            BuildEntity(new PictureBinaryBuilder());
            BuildEntity(new ProductPictureBuilder());
            BuildEntity(new SettingBuilder());
            BuildEntity(new DiscountBuilder());
            BuildEntity(new DiscountCategoryMappingBuilder());
            BuildEntity(new DiscountProductMappingBuilder());
            BuildEntity(new DiscountRequirementBuilder());
            BuildEntity(new DiscountUsageHistoryBuilder());
            BuildEntity(new PrivateMessageBuilder());
            BuildEntity(new ForumGroupBuilder());
            BuildEntity(new ForumBuilder());
            BuildEntity(new ForumTopicBuilder());
            BuildEntity(new ForumPostBuilder());
            BuildEntity(new ForumPostVoteBuilder());
            BuildEntity(new ForumSubscriptionBuilder());
            BuildEntity(new GdprConsentBuilder());
            BuildEntity(new GdprLogBuilder());
            BuildEntity(new ActivityLogTypeBuilder());
            BuildEntity(new ActivityLogBuilder());
            BuildEntity(new LogBuilder());
            BuildEntity(new CampaignBuilder());
            BuildEntity(new EmailAccountBuilder());
            BuildEntity(new MessageTemplateBuilder());
            BuildEntity(new NewsLetterSubscriptionBuilder());
            BuildEntity(new QueuedEmailBuilder());
            BuildEntity(new NewsItemBuilder());
            BuildEntity(new NewsCommentBuilder());
            BuildEntity(new PollBuilder());
            BuildEntity(new PollAnswerBuilder());
            BuildEntity(new PollVotingRecordBuilder());
            BuildEntity(new AclRecordBuilder());
            BuildEntity(new PermissionRecordBuilder());
            BuildEntity(new PermissionRecordCustomerRoleMappingBuilder());
            BuildEntity(new UrlRecordBuilder());
            BuildEntity(new ScheduleTaskBuilder());
            BuildEntity(new TaxCategoryBuilder());
            BuildEntity(new TopicTemplateBuilder());
            BuildEntity(new TopicBuilder());
            BuildEntity(new VendorBuilder());
            BuildEntity(new VendorAttributeBuilder());
            BuildEntity(new VendorAttributeValueBuilder());
            BuildEntity(new VendorNoteBuilder());
        }

        private void BuildEntity<TEntity>(IEntityBuilder<TEntity> builder = null) where TEntity : BaseEntity
        {
            var tableName = builder?.TableName ?? typeof(TEntity).Name;

            var tableExpr = new CreateTableExpression { TableName = tableName };

            var create = new CreateTableExpressionBuilder(tableExpr, _context);
            
            if(builder != null)
                builder.MapEntity(create);

            if (!tableExpr.Columns.Any(c => c.IsPrimaryKey))
            {
                var pk = new ColumnDefinition
                {
                    Name = nameof(BaseEntity.Id),
                    Type = DbType.Int32,
                    IsIdentity = true,
                    TableName = tableName,
                    ModificationType = ColumnModificationType.Create,
                    IsPrimaryKey = true
                };

                tableExpr.Columns.Insert(0, pk);
            }

            var entityColumnNames = tableExpr.Columns.Select(c => c.Name);
            var propertiesToAutoMap = typeof(TEntity).GetProperties().Where(p => !entityColumnNames.Contains(p.Name));
            if (propertiesToAutoMap is null || propertiesToAutoMap.Count() == 0)
                return;

            foreach (var prop in propertiesToAutoMap)
            {
                create.WithSelfType(prop.Name, prop.PropertyType);
            }

            _cache.TryAdd(typeof(TEntity), tableExpr);
        }
    }
}