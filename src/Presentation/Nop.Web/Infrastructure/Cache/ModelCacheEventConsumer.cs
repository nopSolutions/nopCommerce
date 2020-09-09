using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Cms;
using Nop.Services.Events;
using Nop.Services.Plugins;

namespace Nop.Web.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer :
        //languages
        IConsumer<EntityInsertedEvent<Language>>,
        IConsumer<EntityUpdatedEvent<Language>>,
        IConsumer<EntityDeletedEvent<Language>>,
        //settings
        IConsumer<EntityUpdatedEvent<Setting>>,
        //manufacturers
        IConsumer<EntityInsertedEvent<Manufacturer>>,
        IConsumer<EntityUpdatedEvent<Manufacturer>>,
        IConsumer<EntityDeletedEvent<Manufacturer>>,
        //vendors
        IConsumer<EntityInsertedEvent<Vendor>>,
        IConsumer<EntityUpdatedEvent<Vendor>>,
        IConsumer<EntityDeletedEvent<Vendor>>,
        //product manufacturers
        IConsumer<EntityInsertedEvent<ProductManufacturer>>,
        IConsumer<EntityUpdatedEvent<ProductManufacturer>>,
        IConsumer<EntityDeletedEvent<ProductManufacturer>>,
        //categories
        IConsumer<EntityInsertedEvent<Category>>,
        IConsumer<EntityUpdatedEvent<Category>>,
        IConsumer<EntityDeletedEvent<Category>>,
        //product categories
        IConsumer<EntityInsertedEvent<ProductCategory>>,
        IConsumer<EntityUpdatedEvent<ProductCategory>>,
        IConsumer<EntityDeletedEvent<ProductCategory>>,
        //products
        IConsumer<EntityInsertedEvent<Product>>,
        IConsumer<EntityUpdatedEvent<Product>>,
        IConsumer<EntityDeletedEvent<Product>>,
        //product tags
        IConsumer<EntityInsertedEvent<ProductTag>>,
        IConsumer<EntityUpdatedEvent<ProductTag>>,
        IConsumer<EntityDeletedEvent<ProductTag>>,
        //specification attributes
        IConsumer<EntityUpdatedEvent<SpecificationAttribute>>,
        IConsumer<EntityDeletedEvent<SpecificationAttribute>>,
        //specification attribute options
        IConsumer<EntityUpdatedEvent<SpecificationAttributeOption>>,
        IConsumer<EntityDeletedEvent<SpecificationAttributeOption>>,
        //Product specification attribute
        IConsumer<EntityInsertedEvent<ProductSpecificationAttribute>>,
        IConsumer<EntityUpdatedEvent<ProductSpecificationAttribute>>,
        IConsumer<EntityDeletedEvent<ProductSpecificationAttribute>>,
        //Product attribute values
        IConsumer<EntityUpdatedEvent<ProductAttributeValue>>,
        //Topics
        IConsumer<EntityInsertedEvent<Topic>>,
        IConsumer<EntityUpdatedEvent<Topic>>,
        IConsumer<EntityDeletedEvent<Topic>>,
        //Orders
        IConsumer<EntityInsertedEvent<Order>>,
        IConsumer<EntityUpdatedEvent<Order>>,
        IConsumer<EntityDeletedEvent<Order>>,
        //Picture
        IConsumer<EntityInsertedEvent<Picture>>,
        IConsumer<EntityUpdatedEvent<Picture>>,
        IConsumer<EntityDeletedEvent<Picture>>,
        //Product picture mapping
        IConsumer<EntityInsertedEvent<ProductPicture>>,
        IConsumer<EntityUpdatedEvent<ProductPicture>>,
        IConsumer<EntityDeletedEvent<ProductPicture>>,
        //Product review
        IConsumer<EntityDeletedEvent<ProductReview>>,
        //polls
        IConsumer<EntityInsertedEvent<Poll>>,
        IConsumer<EntityUpdatedEvent<Poll>>,
        IConsumer<EntityDeletedEvent<Poll>>,
        //blog posts
        IConsumer<EntityInsertedEvent<BlogPost>>,
        IConsumer<EntityUpdatedEvent<BlogPost>>,
        IConsumer<EntityDeletedEvent<BlogPost>>,
        //news items
        IConsumer<EntityInsertedEvent<NewsItem>>,
        IConsumer<EntityUpdatedEvent<NewsItem>>,
        IConsumer<EntityDeletedEvent<NewsItem>>,
        //shopping cart items
        IConsumer<EntityUpdatedEvent<ShoppingCartItem>>,
        //plugins
        IConsumer<PluginUpdatedEvent>
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public ModelCacheEventConsumer(CatalogSettings catalogSettings, IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
            _catalogSettings = catalogSettings;
        }

        #endregion

        #region Methods

        #region Languages

        public async Task HandleEvent(EntityInsertedEvent<Language> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SpecsFilterPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        public async Task HandleEvent(EntityUpdatedEvent<Language> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SpecsFilterPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        public async Task HandleEvent(EntityDeletedEvent<Language> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SpecsFilterPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        #endregion
        
        #region Setting

        public async Task HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            //clear models which depend on settings
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ManufacturerNavigationPrefixCacheKey); //depends on CatalogSettings.ManufacturersBlockItemsToDisplay
            await _staticCacheManager.Remove(NopModelCacheDefaults.VendorNavigationModelKey); //depends on VendorSettings.VendorBlockItemsToDisplay
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryAllPrefixCacheKey); //depends on CatalogSettings.ShowCategoryProductNumber and CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey); //depends on CatalogSettings.NumberOfBestsellersOnHomepage
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey); //depends on CatalogSettings.ProductsAlsoPurchasedNumber
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.BlogPrefixCacheKey); //depends on BlogSettings.NumberOfTags
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.NewsPrefixCacheKey); //depends on NewsSettings.MainPageNewsCount
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey); //depends on distinct sitemap settings
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.WidgetPrefixCacheKey); //depends on WidgetSettings and certain settings of widgets
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.StoreLogoPathPrefixCacheKey); //depends on StoreInformationSettings.LogoPictureId
        }

        #endregion

        #region Vendors

        public async Task HandleEvent(EntityInsertedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.Remove(NopModelCacheDefaults.VendorNavigationModelKey);
        }

        public async Task HandleEvent(EntityUpdatedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.Remove(NopModelCacheDefaults.VendorNavigationModelKey);
            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.VendorPicturePrefixCacheKeyById, eventMessage.Entity.Id));
        }

        public async Task HandleEvent(EntityDeletedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.Remove(NopModelCacheDefaults.VendorNavigationModelKey);
        }

        #endregion

        #region  Manufacturers

        public async Task HandleEvent(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEvent(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.ManufacturerPicturePrefixCacheKeyById, eventMessage.Entity.Id));
        }

        public async Task HandleEvent(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region  Product manufacturers

        public async Task HandleEvent(EntityInsertedEvent<ProductManufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.ManufacturerHasFeaturedProductsPrefixCacheKeyById, eventMessage.Entity.ManufacturerId));
        }

        public async Task HandleEvent(EntityUpdatedEvent<ProductManufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.ManufacturerHasFeaturedProductsPrefixCacheKeyById, eventMessage.Entity.ManufacturerId));
        }

        public async Task HandleEvent(EntityDeletedEvent<ProductManufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.ManufacturerHasFeaturedProductsPrefixCacheKeyById, eventMessage.Entity.ManufacturerId));
        }

        #endregion

        #region Categories

        public async Task HandleEvent(EntityInsertedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryHomepagePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEvent(EntityUpdatedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryHomepagePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.CategoryPicturePrefixCacheKeyById, eventMessage.Entity.Id));
        }

        public async Task HandleEvent(EntityDeletedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryHomepagePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Product categories

        public async Task HandleEvent(EntityInsertedEvent<ProductCategory> eventMessage)
        {
            if (_catalogSettings.ShowCategoryProductNumber)
            {
                //depends on CatalogSettings.ShowCategoryProductNumber (when enabled)
                //so there's no need to clear this cache in other cases
                await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryAllPrefixCacheKey);
                await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            }

            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.CategoryHasFeaturedProductsPrefixCacheKeyById, eventMessage.Entity.CategoryId));
        }

        public async Task HandleEvent(EntityUpdatedEvent<ProductCategory> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.CategoryHasFeaturedProductsPrefixCacheKeyById, eventMessage.Entity.CategoryId));
        }

        public async Task HandleEvent(EntityDeletedEvent<ProductCategory> eventMessage)
        {
            if (_catalogSettings.ShowCategoryProductNumber)
            {
                //depends on CatalogSettings.ShowCategoryProductNumber (when enabled)
                //so there's no need to clear this cache in other cases
                await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryAllPrefixCacheKey);
                await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            }

            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.CategoryHasFeaturedProductsPrefixCacheKeyById, eventMessage.Entity.CategoryId));
        }

        #endregion

        #region Products

        public async Task HandleEvent(EntityInsertedEvent<Product> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEvent(EntityUpdatedEvent<Product> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.ProductReviewsPrefixCacheKeyById, eventMessage.Entity.Id));
        }

        public async Task HandleEvent(EntityDeletedEvent<Product> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Product tags

        public async Task HandleEvent(EntityInsertedEvent<ProductTag> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEvent(EntityUpdatedEvent<ProductTag> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEvent(EntityDeletedEvent<ProductTag> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Specification attributes

        public async Task HandleEvent(EntityUpdatedEvent<SpecificationAttribute> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        public async Task HandleEvent(EntityDeletedEvent<SpecificationAttribute> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        #endregion

        #region Specification attribute options

        public async Task HandleEvent(EntityUpdatedEvent<SpecificationAttributeOption> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        public async Task HandleEvent(EntityDeletedEvent<SpecificationAttributeOption> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        #endregion

        #region Product specification attribute

        public async Task HandleEvent(EntityInsertedEvent<ProductSpecificationAttribute> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        public async Task HandleEvent(EntityUpdatedEvent<ProductSpecificationAttribute> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        public async Task HandleEvent(EntityDeletedEvent<ProductSpecificationAttribute> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        #endregion

        #region Product attributes

        public async Task HandleEvent(EntityUpdatedEvent<ProductAttributeValue> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductAttributeImageSquarePicturePrefixCacheKey);
        }

        #endregion

        #region Topics

        public async Task HandleEvent(EntityInsertedEvent<Topic> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEvent(EntityUpdatedEvent<Topic> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEvent(EntityDeletedEvent<Topic> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Orders

        public async Task HandleEvent(EntityInsertedEvent<Order> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
        }

        public async Task HandleEvent(EntityUpdatedEvent<Order> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
        }

        public async Task HandleEvent(EntityDeletedEvent<Order> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
        }

        #endregion

        #region Pictures

        public async Task HandleEvent(EntityInsertedEvent<Picture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CartPicturePrefixCacheKey);
        }

        public async Task HandleEvent(EntityUpdatedEvent<Picture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductDetailsPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductDefaultPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ManufacturerPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.VendorPicturePrefixCacheKey);
        }

        public async Task HandleEvent(EntityDeletedEvent<Picture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductDetailsPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductDefaultPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CategoryPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ManufacturerPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.VendorPicturePrefixCacheKey);
        }

        #endregion

        #region Product picture mappings

        public async Task HandleEvent(EntityInsertedEvent<ProductPicture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.ProductDefaultPicturePrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.ProductDetailsPicturesPrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CartPicturePrefixCacheKey);
        }

        public async Task HandleEvent(EntityUpdatedEvent<ProductPicture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.ProductDefaultPicturePrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.ProductDetailsPicturesPrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CartPicturePrefixCacheKey);
        }

        public async Task HandleEvent(EntityDeletedEvent<ProductPicture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.ProductDefaultPicturePrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.ProductDetailsPicturesPrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CartPicturePrefixCacheKey);
        }

        #endregion

        #region Polls

        public async Task HandleEvent(EntityInsertedEvent<Poll> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.PollsPrefixCacheKey);
        }

        public async Task HandleEvent(EntityUpdatedEvent<Poll> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.PollsPrefixCacheKey);
        }

        public async Task HandleEvent(EntityDeletedEvent<Poll> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.PollsPrefixCacheKey);
        }

        #endregion

        #region Blog posts

        public async Task HandleEvent(EntityInsertedEvent<BlogPost> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.BlogPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEvent(EntityUpdatedEvent<BlogPost> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.BlogPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEvent(EntityDeletedEvent<BlogPost> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.BlogPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region News items

        public async Task HandleEvent(EntityInsertedEvent<NewsItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.NewsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEvent(EntityUpdatedEvent<NewsItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.NewsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEvent(EntityDeletedEvent<NewsItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.NewsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion
        
        #region Shopping cart items

        public async Task HandleEvent(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.CartPicturePrefixCacheKey);
        }

        #endregion

        #region Product reviews

        public async Task HandleEvent(EntityDeletedEvent<ProductReview> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefix(string.Format(NopModelCacheDefaults.ProductReviewsPrefixCacheKeyById, eventMessage.Entity.ProductId));
        }

        #endregion

        #region Plugin

        /// <summary>
        /// Handle plugin updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public async Task HandleEvent(PluginUpdatedEvent eventMessage)
        {
            if (eventMessage?.Plugin?.Instance<IWidgetPlugin>() != null)
                await _staticCacheManager.RemoveByPrefix(NopModelCacheDefaults.WidgetPrefixCacheKey);
        }

        #endregion

        #endregion
    }
}