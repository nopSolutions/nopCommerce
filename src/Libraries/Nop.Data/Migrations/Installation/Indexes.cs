using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Topics;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Installation;

[NopSchemaMigration("2020/03/13 09:36:08:9037677", "Nop.Data base indexes", MigrationProcessType.Installation)]
public class Indexes : ForwardOnlyMigration
{
    #region Fields

    private readonly INopDataProvider _dataProvider;

    #endregion

    #region Ctor

    public Indexes(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    #endregion

    #region Methods

    public override void Up()
    {
        var urlRecordTableName = nameof(UrlRecord);

        var urlRecordSlugIndexName = _dataProvider.GetIndexName(urlRecordTableName, nameof(UrlRecord.Slug));
        Create.Index(urlRecordSlugIndexName)
            .OnTable(urlRecordTableName)
            .OnColumn(nameof(UrlRecord.Slug))
            .Ascending()
            .WithOptions()
            .NonClustered();


        var urlRecordCustomIndexName = _dataProvider.GetIndexName(urlRecordTableName, $"{nameof(UrlRecord.EntityId)}_{nameof(UrlRecord.EntityName)}_{nameof(UrlRecord.LanguageId)}_{nameof(UrlRecord.IsActive)}");
        Create.Index(urlRecordCustomIndexName).OnTable(nameof(UrlRecord))
            .OnColumn(nameof(UrlRecord.EntityId)).Ascending()
            .OnColumn(nameof(UrlRecord.EntityName)).Ascending()
            .OnColumn(nameof(UrlRecord.LanguageId)).Ascending()
            .OnColumn(nameof(UrlRecord.IsActive)).Ascending()
            .WithOptions().NonClustered();

        var storeMappingCustomIndexName = _dataProvider.GetIndexName(nameof(StoreMapping), $"{nameof(StoreMapping.EntityId)}_{nameof(StoreMapping.EntityName)}");
        Create.Index(storeMappingCustomIndexName).OnTable(nameof(StoreMapping))
            .OnColumn(nameof(StoreMapping.EntityId)).Ascending()
            .OnColumn(nameof(StoreMapping.EntityName)).Ascending()
            .WithOptions().NonClustered();

        var shoppingCartItemCustomIndexName = _dataProvider.GetIndexName(nameof(ShoppingCartItem), $"{nameof(ShoppingCartItem.ShoppingCartTypeId)}_{nameof(ShoppingCartItem.CustomerId)}");
        Create.Index(shoppingCartItemCustomIndexName).OnTable(nameof(ShoppingCartItem))
            .OnColumn(nameof(ShoppingCartItem.ShoppingCartTypeId)).Ascending()
            .OnColumn(nameof(ShoppingCartItem.CustomerId)).Ascending()
            .WithOptions().NonClustered();

        var relatedProductProductId1IndexName = _dataProvider.GetIndexName(nameof(RelatedProduct), nameof(RelatedProduct.ProductId1));
        Create.Index(relatedProductProductId1IndexName).OnTable(nameof(RelatedProduct))
            .OnColumn(nameof(RelatedProduct.ProductId1)).Ascending()
            .WithOptions().NonClustered();

        var queuedEmailCustomIndexName = _dataProvider.GetIndexName(nameof(QueuedEmail), $"{nameof(QueuedEmail.SentOnUtc)}_{nameof(QueuedEmail.DontSendBeforeDateUtc)}");
        Create.Index(queuedEmailCustomIndexName).OnTable(nameof(QueuedEmail))
            .OnColumn(nameof(QueuedEmail.SentOnUtc)).Ascending()
            .OnColumn(nameof(QueuedEmail.DontSendBeforeDateUtc)).Ascending()
            .WithOptions().NonClustered()
            .Include(nameof(QueuedEmail.SentTries));

        var queuedEmailCreatedOnUtcIndexName = _dataProvider.GetIndexName(nameof(QueuedEmail), nameof(QueuedEmail.CreatedOnUtc));
        Create.Index(queuedEmailCreatedOnUtcIndexName).OnTable(nameof(QueuedEmail))
            .OnColumn(nameof(QueuedEmail.CreatedOnUtc)).Descending()
            .WithOptions().NonClustered();

        var productSpecificationAttributeTableName = NameCompatibilityManager.GetTableName(typeof(ProductSpecificationAttribute));
        var productSpecificationAttributeCustomIndexName = _dataProvider.GetIndexName(productSpecificationAttributeTableName, $"{nameof(ProductSpecificationAttribute.SpecificationAttributeOptionId)}_{nameof(ProductSpecificationAttribute.AllowFiltering)}");
        Create.Index(productSpecificationAttributeCustomIndexName).OnTable(productSpecificationAttributeTableName)
            .OnColumn(nameof(ProductSpecificationAttribute.SpecificationAttributeOptionId)).Ascending()
            .OnColumn(nameof(ProductSpecificationAttribute.AllowFiltering)).Ascending()
            .WithOptions().NonClustered()
            .Include(nameof(ProductSpecificationAttribute.ProductId));

        var productSpecificationAttributeAllowFilteringIndexName = _dataProvider.GetIndexName(productSpecificationAttributeTableName, nameof(ProductSpecificationAttribute.AllowFiltering));
        Create.Index(productSpecificationAttributeAllowFilteringIndexName).OnTable(productSpecificationAttributeTableName)
            .OnColumn(nameof(ProductSpecificationAttribute.AllowFiltering)).Ascending()
            .WithOptions().NonClustered()
            .Include(nameof(ProductSpecificationAttribute.ProductId))
            .Include(nameof(ProductSpecificationAttribute.SpecificationAttributeOptionId));

        var productTableName = nameof(Product);
        var productCustomIndexName = _dataProvider.GetIndexName(productTableName, $"{nameof(Product.VisibleIndividually)}_{nameof(Product.Published)}_{nameof(Product.Deleted)}");
        Create.Index(productCustomIndexName).OnTable(productTableName)
            .OnColumn(nameof(Product.VisibleIndividually)).Ascending()
            .OnColumn(nameof(Product.Published)).Ascending()
            .OnColumn(nameof(Product.Deleted)).Ascending()
            .WithOptions().NonClustered()
            .Include(nameof(Product.Id))
            .Include(nameof(Product.AvailableStartDateTimeUtc))
            .Include(nameof(Product.AvailableEndDateTimeUtc));

        var productVisibleIndividuallyIndexName = _dataProvider.GetIndexName(productTableName, nameof(Product.VisibleIndividually));
        Create.Index("IX_Product_VisibleIndividually").OnTable(productTableName)
            .OnColumn(nameof(Product.VisibleIndividually)).Ascending()
            .WithOptions().NonClustered();

        var productTagTableName = nameof(ProductTag);
        var productTagNameIndexName = _dataProvider.GetIndexName(productTagTableName, nameof(ProductTag.Name));
        Create.Index(productTagNameIndexName).OnTable(productTagTableName)
            .OnColumn(nameof(ProductTag.Name)).Ascending()
            .WithOptions().NonClustered();

        var productNameIndexName = _dataProvider.GetIndexName(productTableName, nameof(Product.Name));
        Create.Index(productNameIndexName).OnTable(productTableName)
            .OnColumn(nameof(Product.Name)).Ascending()
            .WithOptions().NonClustered();

        var productSubjectToAclIndexName = _dataProvider.GetIndexName(productTableName, nameof(Product.SubjectToAcl));
        Create.Index(productSubjectToAclIndexName).OnTable(productTableName)
            .OnColumn(nameof(Product.SubjectToAcl)).Ascending()
            .WithOptions().NonClustered();

        var productShowOnHomepageIndexName = _dataProvider.GetIndexName(productTableName, nameof(Product.ShowOnHomepage));
        Create.Index(productShowOnHomepageIndexName).OnTable(productTableName)
            .OnColumn(nameof(Product.ShowOnHomepage)).Ascending()
            .WithOptions().NonClustered();

        var productPublishedIndexName = _dataProvider.GetIndexName(productTableName, nameof(Product.Published));
        Create.Index(productPublishedIndexName).OnTable(productTableName)
            .OnColumn(nameof(Product.Published)).Ascending()
            .WithOptions().NonClustered();

        var productAttributeMappingTableName = NameCompatibilityManager.GetTableName(typeof(ProductAttributeMapping));
        var productAttributeMappingCustomIndexName = _dataProvider.GetIndexName(productAttributeMappingTableName, $"{nameof(ProductAttributeMapping.ProductId)}_{nameof(ProductAttributeMapping.DisplayOrder)}");
        Create.Index(productAttributeMappingCustomIndexName).OnTable(productAttributeMappingTableName)
            .OnColumn(nameof(ProductAttributeMapping.ProductId)).Ascending()
            .OnColumn(nameof(ProductAttributeMapping.DisplayOrder)).Ascending()
            .WithOptions().NonClustered();

        var productPriceDatesEtcIndexName = _dataProvider.GetIndexName(productTableName, $"{nameof(Product.Price)}_{nameof(Product.AvailableStartDateTimeUtc)}_{nameof(Product.AvailableEndDateTimeUtc)}_{nameof(Product.Published)}_{nameof(Product.Deleted)}");
        Create.Index(productPriceDatesEtcIndexName).OnTable(productTableName)
            .OnColumn(nameof(Product.Price)).Ascending()
            .OnColumn(nameof(Product.AvailableStartDateTimeUtc)).Ascending()
            .OnColumn(nameof(Product.AvailableEndDateTimeUtc)).Ascending()
            .OnColumn(nameof(Product.Published)).Ascending()
            .OnColumn(nameof(Product.Deleted)).Ascending()
            .WithOptions().NonClustered();

        var productParentGroupedProductIdIndexName = _dataProvider.GetIndexName(productTableName, nameof(Product.ParentGroupedProductId));
        Create.Index(productParentGroupedProductIdIndexName).OnTable(productTableName)
            .OnColumn(nameof(Product.ParentGroupedProductId)).Ascending()
            .WithOptions().NonClustered();

        var productManufacturerTableName = NameCompatibilityManager.GetTableName(typeof(ProductManufacturer));
        var productManufacturerIsFeaturedProductIndexName = _dataProvider.GetIndexName(productManufacturerTableName, nameof(ProductManufacturer.IsFeaturedProduct));
        Create.Index(productManufacturerIsFeaturedProductIndexName)
            .OnTable(productManufacturerTableName)
            .OnColumn(nameof(ProductManufacturer.IsFeaturedProduct)).Ascending()
            .WithOptions().NonClustered();

        var productLimitedToStoresIndexName = _dataProvider.GetIndexName(productTableName, nameof(Product.LimitedToStores));
        Create.Index(productLimitedToStoresIndexName).OnTable(productTableName)
            .OnColumn(nameof(Product.LimitedToStores)).Ascending()
            .WithOptions().NonClustered();

        var productDeletedIdIndexName = _dataProvider.GetIndexName(productTableName, $"{nameof(Product.Deleted)}_{nameof(Product.Id)}");
        Create.Index(productDeletedIdIndexName).OnTable(productTableName)
            .OnColumn(nameof(Product.Deleted)).Ascending()
            .OnColumn(nameof(Product.Id)).Ascending()
            .WithOptions().NonClustered();

        var productDeletedPublishedIndexName = _dataProvider.GetIndexName(productTableName, $"{nameof(Product.Published)}_{nameof(Product.Deleted)}");
        Create.Index(productDeletedPublishedIndexName).OnTable(productTableName)
            .OnColumn(nameof(Product.Published)).Ascending()
            .OnColumn(nameof(Product.Deleted)).Ascending()
            .WithOptions().NonClustered();

        var productCategoryTableName = NameCompatibilityManager.GetTableName(typeof(ProductCategory));
        var productCategoryIsFeaturedProductIndexName = _dataProvider.GetIndexName(productCategoryTableName, nameof(ProductCategory.IsFeaturedProduct));
        Create.Index(productCategoryIsFeaturedProductIndexName).OnTable(productCategoryTableName)
            .OnColumn(nameof(ProductCategory.IsFeaturedProduct)).Ascending()
            .WithOptions().NonClustered();

        var productAttributeValueCustomIndexName = _dataProvider.GetIndexName(nameof(ProductAttributeValue), $"{nameof(ProductAttributeValue.ProductAttributeMappingId)}_{nameof(ProductAttributeValue.DisplayOrder)}");
        Create.Index(productAttributeValueCustomIndexName).OnTable(nameof(ProductAttributeValue))
            .OnColumn(nameof(ProductAttributeValue.ProductAttributeMappingId)).Ascending()
            .OnColumn(nameof(ProductAttributeValue.DisplayOrder)).Ascending()
            .WithOptions().NonClustered();

        var productManufacturerManufacturerIdProductIdIndexName = _dataProvider.GetIndexName(productManufacturerTableName, $"{nameof(ProductManufacturer.ManufacturerId)}_{nameof(ProductManufacturer.ProductId)}");
        Create.Index(productManufacturerManufacturerIdProductIdIndexName).OnTable(productManufacturerTableName)
            .OnColumn(nameof(ProductManufacturer.ManufacturerId)).Ascending()
            .OnColumn(nameof(ProductManufacturer.ProductId)).Ascending()
            .WithOptions().NonClustered();

        var productManufacturerIsFeaturedProductIdIndexName = _dataProvider.GetIndexName(productManufacturerTableName, $"{nameof(ProductManufacturer.ProductId)}_{nameof(ProductManufacturer.IsFeaturedProduct)}");
        Create.Index(productManufacturerIsFeaturedProductIdIndexName).OnTable(productManufacturerTableName)
            .OnColumn(nameof(ProductManufacturer.ProductId)).Ascending()
            .OnColumn(nameof(ProductManufacturer.IsFeaturedProduct)).Ascending()
            .WithOptions().NonClustered()
            .Include(nameof(ProductManufacturer.ManufacturerId));

        var productCategoryIsFeaturedProductIdIndexName = _dataProvider.GetIndexName(productCategoryTableName, $"{nameof(ProductCategory.ProductId)}_{nameof(ProductCategory.IsFeaturedProduct)}");
        Create.Index(productCategoryIsFeaturedProductIdIndexName).OnTable(productCategoryTableName)
            .OnColumn(nameof(ProductCategory.ProductId)).Ascending()
            .OnColumn(nameof(ProductCategory.IsFeaturedProduct)).Ascending()
            .WithOptions().NonClustered()
            .Include(nameof(ProductCategory.CategoryId));

        var productCategoryCategoryIdProductIdIndexName = _dataProvider.GetIndexName(productCategoryTableName, $"{nameof(ProductCategory.CategoryId)}_{nameof(ProductCategory.ProductId)}");
        Create.Index(productCategoryCategoryIdProductIdIndexName).OnTable(productCategoryTableName)
            .OnColumn(nameof(ProductCategory.CategoryId)).Ascending()
            .OnColumn(nameof(ProductCategory.ProductId)).Ascending()
            .WithOptions().NonClustered();

        var orderCreatedOnUtcIndexName = _dataProvider.GetIndexName(nameof(Order), nameof(Order.CreatedOnUtc));
        Create.Index(orderCreatedOnUtcIndexName).OnTable(nameof(Order))
            .OnColumn(nameof(Order.CreatedOnUtc)).Descending()
            .WithOptions().NonClustered();

        var newsLetterSubscriptionEmailStoreIdIndexName = _dataProvider.GetIndexName(nameof(NewsLetterSubscription), $"{nameof(NewsLetterSubscription.Email)}_{nameof(NewsLetterSubscription.StoreId)}");
        Create.Index(newsLetterSubscriptionEmailStoreIdIndexName).OnTable(nameof(NewsLetterSubscription))
            .OnColumn(nameof(NewsLetterSubscription.Email)).Ascending()
            .OnColumn(nameof(NewsLetterSubscription.StoreId)).Ascending()
            .WithOptions().NonClustered();

        var manufacturerSubjectToAclIndexName = _dataProvider.GetIndexName(nameof(Manufacturer), nameof(Manufacturer.SubjectToAcl));
        Create.Index(manufacturerSubjectToAclIndexName).OnTable(nameof(Manufacturer))
            .OnColumn(nameof(Manufacturer.SubjectToAcl)).Ascending()
            .WithOptions().NonClustered();

        var manufacturerLimitedToStoresIndexName = _dataProvider.GetIndexName(nameof(Manufacturer), nameof(Manufacturer.LimitedToStores));
        Create.Index("IX_Manufacturer_LimitedToStores").OnTable(nameof(Manufacturer))
            .OnColumn(nameof(Manufacturer.LimitedToStores)).Ascending()
            .WithOptions().NonClustered();

        var manufacturerDisplayOrderIndexName = _dataProvider.GetIndexName(nameof(Manufacturer), nameof(Manufacturer.DisplayOrder));
        Create.Index(manufacturerDisplayOrderIndexName).OnTable(nameof(Manufacturer))
            .OnColumn(nameof(Manufacturer.DisplayOrder)).Ascending()
            .WithOptions().NonClustered();

        var logCreatedOnUtcIndexName = _dataProvider.GetIndexName(nameof(Log), nameof(Log.CreatedOnUtc));
        Create.Index(logCreatedOnUtcIndexName).OnTable(nameof(Log))
            .OnColumn(nameof(Log.CreatedOnUtc)).Descending()
            .WithOptions().NonClustered();

        var localeStringResourceIndexName = _dataProvider.GetIndexName(nameof(LocaleStringResource), $"{nameof(LocaleStringResource.ResourceName)}_{nameof(LocaleStringResource.LanguageId)}");
        Create.Index(localeStringResourceIndexName).OnTable(nameof(LocaleStringResource))
            .OnColumn(nameof(LocaleStringResource.ResourceName)).Ascending()
            .OnColumn(nameof(LocaleStringResource.LanguageId)).Ascending()
            .WithOptions().NonClustered();

        var IndexName = _dataProvider.GetIndexName(nameof(Language), nameof(Language.DisplayOrder));
        Create.Index("IX_Language_DisplayOrder").OnTable(nameof(Language))
            .OnColumn(nameof(Language.DisplayOrder)).Ascending()
            .WithOptions().NonClustered();

        var productLowStockIndexName = _dataProvider.GetIndexName(productTableName, $"{nameof(Product.Deleted)}_{nameof(Product.VendorId)}_{nameof(Product.ProductTypeId)}_{nameof(Product.ManageInventoryMethodId)}_{nameof(Product.MinStockQuantity)}_{nameof(Product.UseMultipleWarehouses)}");
        Create.Index(productLowStockIndexName).OnTable(productTableName)
            .OnColumn(nameof(Product.Deleted)).Ascending()
            .OnColumn(nameof(Product.VendorId)).Ascending()
            .OnColumn(nameof(Product.ProductTypeId)).Ascending()
            .OnColumn(nameof(Product.ManageInventoryMethodId)).Ascending()
            .OnColumn(nameof(Product.MinStockQuantity)).Ascending()
            .OnColumn(nameof(Product.UseMultipleWarehouses)).Ascending()
            .WithOptions().NonClustered();

        var genericAttributeEntityIdKeyGroupIndexName = _dataProvider.GetIndexName(nameof(GenericAttribute), $"{nameof(GenericAttribute.EntityId)}_{nameof(GenericAttribute.KeyGroup)}");
        Create.Index(genericAttributeEntityIdKeyGroupIndexName).OnTable(nameof(GenericAttribute))
            .OnColumn(nameof(GenericAttribute.EntityId)).Ascending()
            .OnColumn(nameof(GenericAttribute.KeyGroup)).Ascending()
            .WithOptions().NonClustered();

        var forumSubscriptionTableName = NameCompatibilityManager.GetTableName(typeof(ForumSubscription));
        var forumSubscriptionTopicIdIndexName = _dataProvider.GetIndexName(forumSubscriptionTableName, nameof(ForumSubscription.TopicId));
        Create.Index(forumSubscriptionTopicIdIndexName).OnTable(forumSubscriptionTableName)
            .OnColumn(nameof(ForumSubscription.TopicId)).Ascending()
            .WithOptions().NonClustered();

        var forumSubscriptionForumIdIndexName = _dataProvider.GetIndexName(forumSubscriptionTableName, nameof(ForumSubscription.ForumId));
        Create.Index(forumSubscriptionForumIdIndexName).OnTable(forumSubscriptionTableName)
            .OnColumn(nameof(ForumSubscription.ForumId)).Ascending()
            .WithOptions().NonClustered();

        var forumGroupTableName = NameCompatibilityManager.GetTableName(typeof(ForumGroup));
        var forumGroupDisplayOrderIndexName = _dataProvider.GetIndexName(forumGroupTableName, nameof(ForumGroup.DisplayOrder));
        Create.Index(forumGroupDisplayOrderIndexName).OnTable(forumGroupTableName)
            .OnColumn(nameof(ForumGroup.DisplayOrder)).Ascending()
            .WithOptions().NonClustered();

        var forumTableName = NameCompatibilityManager.GetTableName(typeof(Forum));
        var forumDisplayOrderIndexName = _dataProvider.GetIndexName(forumTableName, nameof(Forum.DisplayOrder));
        Create.Index(forumDisplayOrderIndexName).OnTable(forumTableName)
            .OnColumn(nameof(Forum.DisplayOrder)).Ascending()
            .WithOptions().NonClustered();

        var forumTopicTableName = NameCompatibilityManager.GetTableName(typeof(ForumTopic));
        var forumTopicSubjectIndexName = _dataProvider.GetIndexName(forumTopicTableName, nameof(ForumTopic.Subject));
        Create.Index(forumTopicSubjectIndexName).OnTable(forumTopicTableName)
            .OnColumn(nameof(ForumTopic.Subject)).Ascending()
            .WithOptions().NonClustered();

        var customerTableName = nameof(Customer);
        var customerUsernameIndexName = _dataProvider.GetIndexName(customerTableName, nameof(Customer.Username));
        Create.Index(customerUsernameIndexName).OnTable(customerTableName)
            .OnColumn(nameof(Customer.Username)).Ascending()
            .WithOptions().NonClustered();

        var customerSystemNameIndexName = _dataProvider.GetIndexName(customerTableName, nameof(Customer.SystemName));
        Create.Index(customerSystemNameIndexName).OnTable(customerTableName)
            .OnColumn(nameof(Customer.SystemName)).Ascending()
            .WithOptions().NonClustered();

        var customerEmailIndexName = _dataProvider.GetIndexName(customerTableName, nameof(Customer.Email));
        Create.Index(customerEmailIndexName).OnTable(customerTableName)
            .OnColumn(nameof(Customer.Email)).Ascending()
            .WithOptions().NonClustered();

        var customerCustomerGuidIndexName = _dataProvider.GetIndexName(customerTableName, nameof(Customer.CustomerGuid));
        Create.Index(customerCustomerGuidIndexName).OnTable(customerTableName)
            .OnColumn(nameof(Customer.CustomerGuid)).Ascending()
            .WithOptions().NonClustered();

        var customerCreatedOnUtcIndexName = _dataProvider.GetIndexName(customerTableName, nameof(Customer.CreatedOnUtc));
        Create.Index(customerCreatedOnUtcIndexName).OnTable(customerTableName)
            .OnColumn(nameof(Customer.CreatedOnUtc)).Descending()
            .WithOptions().NonClustered();

        var customerDisplayOrderIndexName = _dataProvider.GetIndexName(nameof(Currency), nameof(Currency.DisplayOrder));
        Create.Index(customerDisplayOrderIndexName).OnTable(nameof(Currency))
            .OnColumn(nameof(Currency.DisplayOrder)).Ascending()
            .WithOptions().NonClustered();

        var countryDisplayOrderIndexName = _dataProvider.GetIndexName(nameof(Country), nameof(Country.DisplayOrder));
        Create.Index(countryDisplayOrderIndexName).OnTable(nameof(Country))
            .OnColumn(nameof(Country.DisplayOrder)).Ascending()
            .WithOptions().NonClustered();

        var categoryTableName = nameof(Category);
        var categoryParentCategoryIdIndexName = _dataProvider.GetIndexName(categoryTableName, nameof(Category.ParentCategoryId));
        Create.Index(categoryParentCategoryIdIndexName).OnTable(categoryTableName)
            .OnColumn(nameof(Category.ParentCategoryId)).Ascending()
            .WithOptions().NonClustered();

        var categoryLimitedToStoresIndexName = _dataProvider.GetIndexName(categoryTableName, nameof(Category.LimitedToStores));
        Create.Index(categoryLimitedToStoresIndexName).OnTable(categoryTableName)
            .OnColumn(nameof(Category.LimitedToStores)).Ascending()
            .WithOptions().NonClustered();

        var categoryDisplayOrderIndexName = _dataProvider.GetIndexName(categoryTableName, nameof(Category.DisplayOrder));
        Create.Index(categoryDisplayOrderIndexName).OnTable(categoryTableName)
            .OnColumn(nameof(Category.DisplayOrder)).Ascending()
            .WithOptions().NonClustered();

        var categoryDeletedIndexName = _dataProvider.GetIndexName(categoryTableName, nameof(Category.Deleted));
        Create.Index(categoryDeletedIndexName).OnTable(categoryTableName)
            .OnColumn(nameof(Category.Deleted)).Ascending()
            .WithOptions().NonClustered()
            .Include(nameof(Category.Id))
            .Include(nameof(Category.Name))
            .Include(nameof(Category.SubjectToAcl)).Include(nameof(Category.LimitedToStores))
            .Include(nameof(Category.Published));

        var categorySubjectToAclIndexName = _dataProvider.GetIndexName(categoryTableName, nameof(Category.SubjectToAcl));
        Create.Index(categorySubjectToAclIndexName).OnTable(categoryTableName)
            .OnColumn(nameof(Category.SubjectToAcl)).Ascending()
            .WithOptions().NonClustered();

        var activityLogCreatedOnUtcIndexName = _dataProvider.GetIndexName(nameof(ActivityLog), nameof(ActivityLog.CreatedOnUtc));
        Create.Index(activityLogCreatedOnUtcIndexName).OnTable(nameof(ActivityLog))
            .OnColumn(nameof(ActivityLog.CreatedOnUtc)).Descending()
            .WithOptions().NonClustered();


        var aclRecordEntityIdEntityNameIndexName = _dataProvider.GetIndexName(nameof(AclRecord), $"{nameof(AclRecord.EntityId)}_{nameof(AclRecord.EntityName)}");
        Create.Index(aclRecordEntityIdEntityNameIndexName).OnTable(nameof(AclRecord))
            .OnColumn(nameof(AclRecord.EntityId)).Ascending()
            .OnColumn(nameof(AclRecord.EntityName)).Ascending()
            .WithOptions().NonClustered();

        var customerDeletedIndexName = _dataProvider.GetIndexName(customerTableName, nameof(Customer.Deleted));
        Create.Index(customerDeletedIndexName)
            .OnTable(customerTableName)
            .OnColumn(nameof(Customer.Deleted)).Ascending()
            .WithOptions().NonClustered();

        var topicTableName = nameof(Topic);
        var topicEndDateColumnName = nameof(Topic.AvailableEndDateTimeUtc);
        var topicStartDateColumnName = nameof(Topic.AvailableStartDateTimeUtc);
        var topicSystemNameColumnName = nameof(Topic.SystemName);

        var topicSystemNameIndexName = _dataProvider.GetIndexName(topicTableName, topicSystemNameColumnName);
        var topicAvailableDatesIndexName = _dataProvider.GetIndexName(topicTableName, $"{topicEndDateColumnName}_{topicStartDateColumnName}");

        Create.Index(topicSystemNameIndexName)
            .OnTable(topicTableName)
            .OnColumn(topicSystemNameColumnName).Ascending()
            .WithOptions().NonClustered();

        Create.Index(topicAvailableDatesIndexName)
                .OnTable(topicTableName)
                .OnColumn(topicEndDateColumnName).Descending()
                .OnColumn(topicStartDateColumnName).Descending()
                .WithOptions().NonClustered();
    }

    #endregion
}