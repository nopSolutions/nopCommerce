using System;
using System.Data;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using Nop.Core;
using Nop.Core.Data;
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

namespace Nop.Data
{
    public class DbNopCommerce : DataConnection
    {
        public DbNopCommerce() : base("nopCommerce") { }

        public TEntity LoadOriginalCopy<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            var entities = GetTable<TEntity>();
            return entities.FirstOrDefault(e => e.Id == Convert.ToInt32(entity.Id));
        }

        /// <summary>
        /// Check whether backups are supported
        /// </summary>
        protected virtual void CheckBackupSupported()
        {
            if (!(DefaultSettings.ConnectionStrings.FirstOrDefault(p => p.Name == ConfigurationString) is ConnectionStringConfigurationProvider connectionStringSettings))
                throw new ArgumentNullException(nameof(connectionStringSettings));

            switch (connectionStringSettings.ProviderType)
            {
                case DataProviderType.SqlServer:
                    return;
                default:
                    throw new DataException("This database does not support backup");
            }
        }

        /// <summary>
        /// Get the current identity value
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <returns>Integer identity; null if cannot get the result</returns>
        public virtual int? GetTableIdent<T>() where T : BaseEntity
        {
            if(!(DefaultSettings.ConnectionStrings.FirstOrDefault(p => p.Name == ConfigurationString) is ConnectionStringConfigurationProvider connectionStringSettings))
                throw new ArgumentNullException(nameof(connectionStringSettings));

            switch (connectionStringSettings.ProviderType)
            {
                case DataProviderType.SqlServer:
                    var tableName = GetTable<T>().TableName;

                    var result = this.Query<decimal?>($"SELECT IDENT_CURRENT('[{tableName}]') as Value").FirstOrDefault();

                    return result.HasValue ? Convert.ToInt32(result) : 1;
                default:
                    throw new NotSupportedException(connectionStringSettings.ProviderName);
            }
        }

        /// <summary>
        /// Set table identity (is supported)
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="ident">Identity value</param>
        public virtual void SetTableIdent<T>(int ident) where T : BaseEntity
        {
            var currentIdent = GetTableIdent<T>();
            if (!currentIdent.HasValue || ident <= currentIdent.Value)
                return;

            if (!(DefaultSettings.ConnectionStrings.FirstOrDefault(p => p.Name == ConfigurationString) is ConnectionStringConfigurationProvider connectionStringSettings))
                throw new ArgumentNullException(nameof(connectionStringSettings));

            switch (connectionStringSettings.ProviderType)
            {
                case DataProviderType.SqlServer:
                    var tableName = GetTable<T>().TableName;

                    this.Execute($"DBCC CHECKIDENT([{tableName}], RESEED, {ident})");

                    break;

                default:
                    throw new NotSupportedException(connectionStringSettings.ProviderName);
            }
        }

        /// <summary>
        /// Creates a backup of the database
        /// </summary>
        public virtual void BackupDatabase(string fileName)
        {
            CheckBackupSupported();
            //var fileName = _fileProvider.Combine(GetBackupDirectoryPath(), $"database_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(10)}.{NopCommonDefaults.DbBackupFileExtension}");

            if (!(DefaultSettings.ConnectionStrings.FirstOrDefault(p => p.Name == ConfigurationString) is ConnectionStringConfigurationProvider connectionStringSettings))
                throw new ArgumentNullException(nameof(connectionStringSettings));

            switch (connectionStringSettings.ProviderType)
            {
                case DataProviderType.SqlServer:
                    var commandText = $"BACKUP DATABASE [{Product.DatabaseName}] TO DISK = '{fileName}' WITH FORMAT";
                    this.Execute(commandText);

                    break;

                default:
                    throw new NotSupportedException(connectionStringSettings.ProviderName);
            }
        }

        /// <summary>
        /// Restores the database from a backup
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        public virtual void RestoreDatabase(string backupFileName)
        {
            CheckBackupSupported();

            if (!(DefaultSettings.ConnectionStrings.FirstOrDefault(p => p.Name == ConfigurationString) is ConnectionStringConfigurationProvider connectionStringSettings))
                throw new ArgumentNullException(nameof(connectionStringSettings));

            switch (connectionStringSettings.ProviderType)
            {
                case DataProviderType.SqlServer:
                    var commandText = string.Format(
                        "DECLARE @ErrorMessage NVARCHAR(4000)\n" +
                        "ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE\n" +
                        "BEGIN TRY\n" +
                        "RESTORE DATABASE [{0}] FROM DISK = '{1}' WITH REPLACE\n" +
                        "END TRY\n" +
                        "BEGIN CATCH\n" +
                        "SET @ErrorMessage = ERROR_MESSAGE()\n" +
                        "END CATCH\n" +
                        "ALTER DATABASE [{0}] SET MULTI_USER WITH ROLLBACK IMMEDIATE\n" +
                        "IF (@ErrorMessage is not NULL)\n" +
                        "BEGIN\n" +
                        "RAISERROR (@ErrorMessage, 16, 1)\n" +
                        "END",
                        Product.DatabaseName,
                        backupFileName);

                    this.Execute(commandText);

                    break;

                default:
                    throw new NotSupportedException(connectionStringSettings.ProviderName);
            }
        }

        /// <summary>
        /// Re-index database tables
        /// </summary>
        public virtual void ReIndexTables()
        {
            if (!(DefaultSettings.ConnectionStrings.FirstOrDefault(p => p.Name == ConfigurationString) is ConnectionStringConfigurationProvider connectionStringSettings))
                throw new ArgumentNullException(nameof(connectionStringSettings));

            switch (connectionStringSettings.ProviderType)
            {
                case DataProviderType.SqlServer:

                    var commandText = $@"
                        DECLARE @TableName sysname 
                        DECLARE cur_reindex CURSOR FOR
                        SELECT table_name
                        FROM [{Product.DatabaseName}].information_schema.tables
                        WHERE table_type = 'base table'
                        OPEN cur_reindex
                        FETCH NEXT FROM cur_reindex INTO @TableName
                        WHILE @@FETCH_STATUS = 0
                            BEGIN
                          exec('ALTER INDEX ALL ON [' + @TableName + '] REBUILD')
                                FETCH NEXT FROM cur_reindex INTO @TableName
                            END
                        CLOSE cur_reindex
                        DEALLOCATE cur_reindex";

                    this.Execute(commandText);

                    break;

                default:
                    throw new NotSupportedException(connectionStringSettings.ProviderName);
            }
        }

        public ITable<PictureBinary> PictureBinary => GetTable<PictureBinary>();
        public ITable<LocaleStringResource> LocaleStringResource => GetTable<LocaleStringResource>();
        public ITable<Log> Log => GetTable<Log>();
        public ITable<Setting> Setting => GetTable<Setting>();
        public ITable<Product> Product => GetTable<Product>();
        public ITable<MessageTemplate> MessageTemplate => GetTable<MessageTemplate>();
        public ITable<BlogPost> BlogPost => GetTable<BlogPost>();
        public ITable<ActivityLog> ActivityLog => GetTable<ActivityLog>();
        public ITable<QueuedEmail> QueuedEmail => GetTable<QueuedEmail>();
        public ITable<Picture> Picture => GetTable<Picture>();
        public ITable<Country> Country => GetTable<Country>();
        public ITable<ProductReview> ProductReview => GetTable<ProductReview>();
        public ITable<ActivityLogType> ActivityLogType => GetTable<ActivityLogType>();
        public ITable<ProductAttributeValue> ProductAttributeValue => GetTable<ProductAttributeValue>();
        public ITable<ProductAttribute> ProductAttribute => GetTable<ProductAttribute>();
        public ITable<NewsComment> NewsComment => GetTable<NewsComment>();
        public ITable<ProductSpecificationAttribute> ProductSpecificationAttribute => GetTable<ProductSpecificationAttribute>();
        public ITable<ProductProductTagMapping> ProductProductTagMapping => GetTable<ProductProductTagMapping>();
        public ITable<ProductAttributeMapping> ProductAttributeMapping => GetTable<ProductAttributeMapping>();
        public ITable<PermissionRecordCustomerRoleMapping> PermissionRecordCustomerRoleMapping => GetTable<PermissionRecordCustomerRoleMapping>();
        public ITable<OrderItem> OrderItem => GetTable<OrderItem>();
        public ITable<ProductManufacturer> ProductManufacturer => GetTable<ProductManufacturer>();
        public ITable<ProductCategory> ProductCategory => GetTable<ProductCategory>();
        public ITable<OrderNote> OrderNote => GetTable<OrderNote>();
        public ITable<NewsItem> NewsItem => GetTable<NewsItem>();
        public ITable<PollAnswer> PollAnswer => GetTable<PollAnswer>();
        public ITable<Poll> Poll => GetTable<Poll>();
        public ITable<ProductAvailabilityRange> ProductAvailabilityRange => GetTable<ProductAvailabilityRange>();
        public ITable<ProductPicture> ProductPicture => GetTable<ProductPicture>();
        public ITable<PermissionRecord> PermissionRecord => GetTable<PermissionRecord>();
        public ITable<ReturnRequestAction> ReturnRequestAction => GetTable<ReturnRequestAction>();
        public ITable<ProductTemplate> ProductTemplate => GetTable<ProductTemplate>();
        public ITable<Vendor> Vendor => GetTable<Vendor>();
        public ITable<UrlRecord> UrlRecord => GetTable<UrlRecord>();
        public ITable<TopicTemplate> TopicTemplate => GetTable<TopicTemplate>();
        public ITable<Topic> Topic => GetTable<Topic>();
        public ITable<TierPrice> TierPrice => GetTable<TierPrice>();
        public ITable<TaxCategory> TaxCategory => GetTable<TaxCategory>();
        public ITable<Store> Store => GetTable<Store>();
        public ITable<StockQuantityHistory> StockQuantityHistory => GetTable<StockQuantityHistory>();
        public ITable<ProductTag> ProductTag => GetTable<ProductTag>();
        public ITable<StateProvince> StateProvince => GetTable<StateProvince>();
        public ITable<SpecificationAttribute> SpecificationAttribute => GetTable<SpecificationAttribute>();
        public ITable<ShoppingCartItem> ShoppingCartItem => GetTable<ShoppingCartItem>();
        public ITable<ShippingMethod> ShippingMethod => GetTable<ShippingMethod>();
        public ITable<ShipmentItem> ShipmentItem => GetTable<ShipmentItem>();
        public ITable<Shipment> Shipment => GetTable<Shipment>();
        public ITable<SearchTerm> SearchTerm => GetTable<SearchTerm>();
        public ITable<ScheduleTask> ScheduleTask => GetTable<ScheduleTask>();
        public ITable<ReturnRequestReason> ReturnRequestReason => GetTable<ReturnRequestReason>();
        public ITable<RelatedProduct> RelatedProduct => GetTable<RelatedProduct>();
        public ITable<SpecificationAttributeOption> SpecificationAttributeOption => GetTable<SpecificationAttributeOption>();
        public ITable<MeasureWeight> MeasureWeight => GetTable<MeasureWeight>();
        public ITable<Order> Order => GetTable<Order>();
        public ITable<ManufacturerTemplate> ManufacturerTemplate => GetTable<ManufacturerTemplate>();
        public ITable<Discount> Discount => GetTable<Discount>();
        public ITable<MeasureDimension> MeasureDimension => GetTable<MeasureDimension>();
        public ITable<DeliveryDate> DeliveryDate => GetTable<DeliveryDate>();
        public ITable<CustomerRole> CustomerRole => GetTable<CustomerRole>();
        public ITable<CustomerPassword> CustomerPassword => GetTable<CustomerPassword>();
        public ITable<CustomerAddressMapping> CustomerAddressMapping => GetTable<CustomerAddressMapping>();
        public ITable<CustomerCustomerRoleMapping> CustomerCustomerRoleMapping => GetTable<CustomerCustomerRoleMapping>();
        public ITable<Download> Download => GetTable<Download>();
        public ITable<Customer> Customer => GetTable<Customer>();
        public ITable<CheckoutAttributeValue> CheckoutAttributeValue => GetTable<CheckoutAttributeValue>();
        public ITable<CheckoutAttribute> CheckoutAttribute => GetTable<CheckoutAttribute>();
        public ITable<CategoryTemplate> CategoryTemplate => GetTable<CategoryTemplate>();
        public ITable<Category> Category => GetTable<Category>();
        public ITable<BlogComment> BlogComment => GetTable<BlogComment>();
        public ITable<Affiliate> Affiliate => GetTable<Affiliate>();
        public ITable<Address> Address => GetTable<Address>();
        public ITable<Currency> Currency => GetTable<Currency>();
        public ITable<EmailAccount> EmailAccount => GetTable<EmailAccount>();
        public ITable<Warehouse> Warehouse => GetTable<Warehouse>();
        public ITable<GenericAttribute> GenericAttribute => GetTable<GenericAttribute>();
        public ITable<ForumGroup> ForumGroup => GetTable<ForumGroup>();
        public ITable<ForumPost> ForumPost => GetTable<ForumPost>();
        public ITable<ForumTopic> ForumTopic => GetTable<ForumTopic>();
        public ITable<Manufacturer> Manufacturer => GetTable<Manufacturer>();
        public ITable<GiftCard> GiftCard => GetTable<GiftCard>();
        public ITable<ForumSubscription> ForumSubscription => GetTable<ForumSubscription>();
        public ITable<Language> Language => GetTable<Language>();
        public ITable<Forum> Forum => GetTable<Forum>();
        public ITable<ShippingMethodCountryMapping> ShippingMethodCountryMapping => GetTable<ShippingMethodCountryMapping>();
        public ITable<CrossSellProduct> CrossSellProduct => GetTable<CrossSellProduct>();
        public ITable<GiftCardUsageHistory> GiftCardUsageHistory => GetTable<GiftCardUsageHistory>();
        public ITable<VendorNote> VendorNote => GetTable<VendorNote>();
        public ITable<Campaign> Campaign => GetTable<Campaign>();
        public ITable<BackInStockSubscription> BackInStockSubscription => GetTable<BackInStockSubscription>();
        public ITable<NewsLetterSubscription> NewsLetterSubscription => GetTable<NewsLetterSubscription>();
        public ITable<LocalizedProperty> LocalizedProperty => GetTable<LocalizedProperty>();
        public ITable<AddressAttributeValue> AddressAttributeValue => GetTable<AddressAttributeValue>();
        public ITable<AddressAttribute> AddressAttribute => GetTable<AddressAttribute>();
        public ITable<AclRecord> AclRecord => GetTable<AclRecord>();
        public ITable<VendorAttribute> VendorAttribute => GetTable<VendorAttribute>();
        public ITable<VendorAttributeValue> VendorAttributeValue => GetTable<VendorAttributeValue>();
        public ITable<StoreMapping> StoreMapping => GetTable<StoreMapping>();
        public ITable<ExternalAuthenticationRecord> ExternalAuthenticationRecord => GetTable<ExternalAuthenticationRecord>();
        public ITable<GdprConsent> GdprConsent => GetTable<GdprConsent>();
        public ITable<GdprLog> GdprLog => GetTable<GdprLog>();
        public ITable<ProductAttributeCombination> ProductAttributeCombination => GetTable<ProductAttributeCombination>();
        public ITable<DiscountUsageHistory> DiscountUsageHistory => GetTable<DiscountUsageHistory>();
        public ITable<DiscountRequirement> DiscountRequirement => GetTable<DiscountRequirement>();
        public ITable<DiscountProductMapping> DiscountProductMapping => GetTable<DiscountProductMapping>();
        public ITable<ProductReviewReviewTypeMapping> ProductReviewReviewTypeMapping => GetTable<ProductReviewReviewTypeMapping>();
        public ITable<ProductReviewHelpfulness> ProductReviewHelpfulness => GetTable<ProductReviewHelpfulness>();
        public ITable<DiscountManufacturerMapping> DiscountManufacturerMapping => GetTable<DiscountManufacturerMapping>();
        public ITable<DiscountCategoryMapping> DiscountCategoryMapping => GetTable<DiscountCategoryMapping>();
        public ITable<ProductWarehouseInventory> ProductWarehouseInventory => GetTable<ProductWarehouseInventory>();
        public ITable<ForumPostVote> ForumPostVote => GetTable<ForumPostVote>();
        public ITable<RecurringPayment> RecurringPayment => GetTable<RecurringPayment>();
        public ITable<PrivateMessage> Forums_PrivateMessage => GetTable<PrivateMessage>();
        public ITable<PredefinedProductAttributeValue> PredefinedProductAttributeValue => GetTable<PredefinedProductAttributeValue>();
        public ITable<ReturnRequest> ReturnRequest => GetTable<ReturnRequest>();
        public ITable<PollVotingRecord> PollVotingRecord => GetTable<PollVotingRecord>();
        public ITable<ReviewType> ReviewType => GetTable<ReviewType>();
        public ITable<RewardPointsHistory> RewardPointsHistory => GetTable<RewardPointsHistory>();
        public ITable<CustomerAttributeValue> CustomerAttributeValue => GetTable<CustomerAttributeValue>();
        public ITable<CustomerAttribute> CustomerAttribute => GetTable<CustomerAttribute>();
        public ITable<RecurringPaymentHistory> RecurringPaymentHistory => GetTable<RecurringPaymentHistory>();
    }
}
