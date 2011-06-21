

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Topics;
using Nop.Core.Infrastructure;
using Nop.Data.Mapping.Localization;


namespace Nop.Data
{
    /// <summary>
    /// Object context
    /// </summary>
    public class NopObjectContext : DbContext, IDbContext
    {
        public NopObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            //((IObjectContextAdapter) this).ObjectContext.ContextOptions.LazyLoadingEnabled = true;
        }

        public DbSet<ActivityLogType> ActivityLogType { get; set; }
        public DbSet<ActivityLog> ActivityLog { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CheckoutAttribute> CheckoutAttributes { get; set; }
        public DbSet<CheckoutAttributeValue> CheckoutAttributeValues { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CrossSellProduct> CrossSellProducts { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CustomerAttribute> CustomerAttributes { get; set; }
        //public DbSet<CustomerContent> CustomerContent { get; set; }
        public DbSet<CustomerRole> CustomerRoles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<DiscountRequirement> DiscountRequirement { get; set; }
        public DbSet<DiscountUsageHistory> DiscountUsageHistory { get; set; }
        public DbSet<Download> Downloads { get; set; }
        public DbSet<EmailAccount> EmailAccounts { get; set; }
        public DbSet<ForumGroup> ForumGroups { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<ForumTopic> ForumTopics { get; set; }
        public DbSet<GiftCard> GiftCards { get; set; }
        public DbSet<GiftCardUsageHistory> GiftCardUsageHistory { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<LocaleStringResource> LocaleStringResources { get; set; }
        public DbSet<LocalizedProperty> LocalizedProperties { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<MeasureDimension> MeasureDimensions { get; set; }
        public DbSet<MeasureWeight> MeasureWeights { get; set; }
        public DbSet<MessageTemplate> MessageTemplates { get; set; }
        public DbSet<NewsLetterSubscription> NewsLetterSubscriptions { get; set; }
        public DbSet<OrderNote> OrderNotes { get; set; }
        public DbSet<OrderProductVariant> OrderProductVariants { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PermissionRecord> PermissionRecords { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<PollAnswer> PollAnswers { get; set; }
        public DbSet<PollVotingRecord> PollVotingRecords { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductManufacturer> ProductManufacturers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPicture> ProductPictures { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ProductReviewHelpfulness> ProductReviewHelpfulness { get; set; }
        public DbSet<ProductSpecificationAttribute> ProductSpecificationAttributes { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<ProductVariantAttribute> ProductVariantAttributes { get; set; }
        public DbSet<ProductVariantAttributeCombination> ProductVariantAttributeCombinations { get; set; }
        public DbSet<ProductVariantAttributeValue> ProductVariantAttributeValues { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<QueuedEmail> QueuedEmails { get; set; }
        public DbSet<RecurringPayment> RecurringPayments { get; set; }
        public DbSet<RecurringPaymentHistory> RecurringPaymentHistory { get; set; }
        public DbSet<RelatedProduct> RelatedProducts { get; set; }
        public DbSet<ReturnRequest> ReturnRequests { get; set; }
        public DbSet<RewardPointsHistory> RewardPointsHistory { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<SpecificationAttribute> SpecificationAttributes { get; set; }
        public DbSet<SpecificationAttributeOption> SpecificationAttributeOptions { get; set; }
        public DbSet<StateProvince> StateProvinces { get; set; }
        public DbSet<TaxCategory> TaxCategories { get; set; }
        public DbSet<TierPrice> TierPrices { get; set; }
        public DbSet<Topic> Topics { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //dynamically load all configuration
            System.Type configType = typeof(LanguageMap);   //any of your configuration classes here
            var typesToRegister = Assembly.GetAssembly(configType).GetTypes()
            .Where(type => !String.IsNullOrEmpty(type.Namespace))
            .Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            //...or do it manually below. For example,
            //modelBuilder.Configurations.Add(new LanguageMap());



            base.OnModelCreating(modelBuilder);
        }

        public string CreateDatabaseScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }
    }
}