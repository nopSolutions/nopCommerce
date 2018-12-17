using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Events;

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
        //currencies
        IConsumer<EntityInsertedEvent<Currency>>,
        IConsumer<EntityUpdatedEvent<Currency>>,
        IConsumer<EntityDeletedEvent<Currency>>,
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
        //related product
        IConsumer<EntityInsertedEvent<RelatedProduct>>,
        IConsumer<EntityUpdatedEvent<RelatedProduct>>,
        IConsumer<EntityDeletedEvent<RelatedProduct>>,
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
        //Product attributes
        IConsumer<EntityDeletedEvent<ProductAttribute>>,
        //Product attributes
        IConsumer<EntityInsertedEvent<ProductAttributeMapping>>,
        IConsumer<EntityDeletedEvent<ProductAttributeMapping>>,
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
        //blog comments
        IConsumer<EntityDeletedEvent<BlogComment>>,
        //news items
        IConsumer<EntityInsertedEvent<NewsItem>>,
        IConsumer<EntityUpdatedEvent<NewsItem>>,
        IConsumer<EntityDeletedEvent<NewsItem>>,
        //news comments
        IConsumer<EntityDeletedEvent<NewsComment>>,
        //states/province
        IConsumer<EntityInsertedEvent<StateProvince>>,
        IConsumer<EntityUpdatedEvent<StateProvince>>,
        IConsumer<EntityDeletedEvent<StateProvince>>,
        //return requests
        IConsumer<EntityInsertedEvent<ReturnRequestAction>>,
        IConsumer<EntityUpdatedEvent<ReturnRequestAction>>,
        IConsumer<EntityDeletedEvent<ReturnRequestAction>>,
        IConsumer<EntityInsertedEvent<ReturnRequestReason>>,
        IConsumer<EntityUpdatedEvent<ReturnRequestReason>>,
        IConsumer<EntityDeletedEvent<ReturnRequestReason>>,
        //templates
        IConsumer<EntityInsertedEvent<CategoryTemplate>>,
        IConsumer<EntityUpdatedEvent<CategoryTemplate>>,
        IConsumer<EntityDeletedEvent<CategoryTemplate>>,
        IConsumer<EntityInsertedEvent<ManufacturerTemplate>>,
        IConsumer<EntityUpdatedEvent<ManufacturerTemplate>>,
        IConsumer<EntityDeletedEvent<ManufacturerTemplate>>,
        IConsumer<EntityInsertedEvent<ProductTemplate>>,
        IConsumer<EntityUpdatedEvent<ProductTemplate>>,
        IConsumer<EntityDeletedEvent<ProductTemplate>>,
        IConsumer<EntityInsertedEvent<TopicTemplate>>,
        IConsumer<EntityUpdatedEvent<TopicTemplate>>,
        IConsumer<EntityDeletedEvent<TopicTemplate>>,
        //checkout attributes
        IConsumer<EntityInsertedEvent<CheckoutAttribute>>,
        IConsumer<EntityUpdatedEvent<CheckoutAttribute>>,
        IConsumer<EntityDeletedEvent<CheckoutAttribute>>,
        //shopping cart items
        IConsumer<EntityUpdatedEvent<ShoppingCartItem>>,
        //plugins
        IConsumer<PluginUpdatedEvent>
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public ModelCacheEventConsumer(CatalogSettings catalogSettings, IStaticCacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
            this._catalogSettings = catalogSettings;
        }

        #endregion

        #region Methods

        //languages
        public void HandleEvent(EntityInsertedEvent<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SearchCategoriesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ManufacturerNavigationPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductSpecsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SpecsFilterPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.TopicPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductBreadcrumbPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryAllPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductManufacturersPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.StateProvincesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.AvailableLanguagesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.AvailableCurrenciesPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SearchCategoriesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ManufacturerNavigationPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductSpecsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SpecsFilterPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.TopicPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductBreadcrumbPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryAllPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductManufacturersPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.StateProvincesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.AvailableLanguagesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.AvailableCurrenciesPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SearchCategoriesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ManufacturerNavigationPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductSpecsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SpecsFilterPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.TopicPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductBreadcrumbPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryAllPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductManufacturersPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.StateProvincesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.AvailableLanguagesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.AvailableCurrenciesPatternKey);
        }

        //currencies
        public void HandleEvent(EntityInsertedEvent<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.AvailableCurrenciesPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.AvailableCurrenciesPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.AvailableCurrenciesPatternKey);
        }

        public void HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            //clear models which depend on settings
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductTagPopularPatternKey); //depends on CatalogSettings.NumberOfProductTags
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ManufacturerNavigationPatternKey); //depends on CatalogSettings.ManufacturersBlockItemsToDisplay
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.VendorNavigationPatternKey); //depends on VendorSettings.VendorBlockItemsToDisplay
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryAllPatternKey); //depends on CatalogSettings.ShowCategoryProductNumber and CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryNumberOfProductsPatternKey); //depends on CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.HomepageBestsellersIdsPatternKey); //depends on CatalogSettings.NumberOfBestsellersOnHomepage
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductsAlsoPurchasedIdsPatternKey); //depends on CatalogSettings.ProductsAlsoPurchasedNumber
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductsRelatedIdsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.BlogPatternKey); //depends on BlogSettings.NumberOfTags
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.NewsPatternKey); //depends on NewsSettings.MainPageNewsCount
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SitemapPatternKey); //depends on distinct sitemap settings
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.WidgetPatternKey); //depends on WidgetSettings and certain settings of widgets
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.StoreLogoPathPatternKey); //depends on StoreInformationSettings.LogoPictureId
        }

        //vendors
        public void HandleEvent(EntityInsertedEvent<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.VendorNavigationPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.VendorNavigationPatternKey);
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.VendorPicturePatternKeyById, eventMessage.Entity.Id));
        }
        public void HandleEvent(EntityDeletedEvent<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.VendorNavigationPatternKey);
        }

        //manufacturers
        public void HandleEvent(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ManufacturerNavigationPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SitemapPatternKey);

        }
        public void HandleEvent(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ManufacturerNavigationPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductManufacturersPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SitemapPatternKey);
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ManufacturerPicturePatternKeyById, eventMessage.Entity.Id));
        }
        public void HandleEvent(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ManufacturerNavigationPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductManufacturersPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SitemapPatternKey);
        }

        //product manufacturers
        public void HandleEvent(EntityInsertedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductManufacturersPatternKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ManufacturerHasFeaturedProductsPatternKeyById, eventMessage.Entity.ManufacturerId));
        }
        public void HandleEvent(EntityUpdatedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductManufacturersPatternKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ManufacturerHasFeaturedProductsPatternKeyById, eventMessage.Entity.ManufacturerId));
        }
        public void HandleEvent(EntityDeletedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductManufacturersPatternKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ManufacturerHasFeaturedProductsPatternKeyById, eventMessage.Entity.ManufacturerId));
        }

        //categories
        public void HandleEvent(EntityInsertedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SearchCategoriesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryAllPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategorySubcategoriesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryHomepagePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SitemapPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SearchCategoriesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductBreadcrumbPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryAllPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryBreadcrumbPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategorySubcategoriesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryHomepagePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SitemapPatternKey);
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.CategoryPicturePatternKeyById, eventMessage.Entity.Id));
        }
        public void HandleEvent(EntityDeletedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SearchCategoriesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductBreadcrumbPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryAllPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryBreadcrumbPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategorySubcategoriesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryHomepagePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SitemapPatternKey);
        }

        //product categories
        public void HandleEvent(EntityInsertedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductBreadcrumbPatternKeyById, eventMessage.Entity.ProductId));
            if (_catalogSettings.ShowCategoryProductNumber)
            {
                //depends on CatalogSettings.ShowCategoryProductNumber (when enabled)
                //so there's no need to clear this cache in other cases
                _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryAllPatternKey);
            }
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryNumberOfProductsPatternKey);
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.CategoryHasFeaturedProductsPatternKeyById, eventMessage.Entity.CategoryId));
        }
        public void HandleEvent(EntityUpdatedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductBreadcrumbPatternKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryNumberOfProductsPatternKey);
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.CategoryHasFeaturedProductsPatternKeyById, eventMessage.Entity.CategoryId));
        }
        public void HandleEvent(EntityDeletedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductBreadcrumbPatternKeyById, eventMessage.Entity.ProductId));
            if (_catalogSettings.ShowCategoryProductNumber)
            {
                //depends on CatalogSettings.ShowCategoryProductNumber (when enabled)
                //so there's no need to clear this cache in other cases
                _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryAllPatternKey);
            }
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryNumberOfProductsPatternKey);
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.CategoryHasFeaturedProductsPatternKeyById, eventMessage.Entity.CategoryId));
        }

        //products
        public void HandleEvent(EntityInsertedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SitemapPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.HomepageBestsellersIdsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductsAlsoPurchasedIdsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductsRelatedIdsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SitemapPatternKey);
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductReviewsPatternKeyById, eventMessage.Entity.Id));
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductTagByProductPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.HomepageBestsellersIdsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductsAlsoPurchasedIdsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductsRelatedIdsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SitemapPatternKey);
        }

        //product tags
        public void HandleEvent(EntityInsertedEvent<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductTagPopularPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductTagByProductPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductTagPopularPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductTagByProductPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductTagPopularPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductTagByProductPatternKey);
        }

        //related products
        public void HandleEvent(EntityInsertedEvent<RelatedProduct> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductsRelatedIdsPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<RelatedProduct> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductsRelatedIdsPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<RelatedProduct> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductsRelatedIdsPatternKey);
        }

        //specification attributes
        public void HandleEvent(EntityUpdatedEvent<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductSpecsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SpecsFilterPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductSpecsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SpecsFilterPatternKey);
        }

        //specification attribute options
        public void HandleEvent(EntityUpdatedEvent<SpecificationAttributeOption> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductSpecsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SpecsFilterPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<SpecificationAttributeOption> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductSpecsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SpecsFilterPatternKey);
        }

        //Product specification attribute
        public void HandleEvent(EntityInsertedEvent<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductSpecsPatternKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SpecsFilterPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductSpecsPatternKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SpecsFilterPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductSpecsPatternKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SpecsFilterPatternKey);
        }

        //Product attributes
        public void HandleEvent(EntityDeletedEvent<ProductAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductHasProductAttributesPatternKey);
        }
        //Product attributes
        public void HandleEvent(EntityInsertedEvent<ProductAttributeMapping> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductHasProductAttributesPatternKeyById, eventMessage.Entity.ProductId));
        }
        public void HandleEvent(EntityDeletedEvent<ProductAttributeMapping> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductHasProductAttributesPatternKeyById, eventMessage.Entity.ProductId));
        }
        //Product attributes
        public void HandleEvent(EntityUpdatedEvent<ProductAttributeValue> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductAttributePicturePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductAttributeImageSquarePicturePatternKey);
        }

        //Topics
        public void HandleEvent(EntityInsertedEvent<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.TopicPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SitemapPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.TopicPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SitemapPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.TopicPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.SitemapPatternKey);
        }

        //Orders
        public void HandleEvent(EntityInsertedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.HomepageBestsellersIdsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductsAlsoPurchasedIdsPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.HomepageBestsellersIdsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductsAlsoPurchasedIdsPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.HomepageBestsellersIdsPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductsAlsoPurchasedIdsPatternKey);
        }

        //Pictures
        public void HandleEvent(EntityInsertedEvent<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductAttributePicturePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CartPicturePatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductAttributePicturePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CartPicturePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductDetailsPicturesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductDefaultPicturePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryPicturePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ManufacturerPicturePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.VendorPicturePatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductAttributePicturePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CartPicturePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductDetailsPicturesPatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductDefaultPicturePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryPicturePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ManufacturerPicturePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.VendorPicturePatternKey);
        }

        //Product picture mappings
        public void HandleEvent(EntityInsertedEvent<ProductPicture> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductDefaultPicturePatternKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductDetailsPicturesPatternKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductAttributePicturePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CartPicturePatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductPicture> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductDefaultPicturePatternKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductDetailsPicturesPatternKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductAttributePicturePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CartPicturePatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<ProductPicture> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductDefaultPicturePatternKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductDetailsPicturesPatternKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductAttributePicturePatternKey);
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CartPicturePatternKey);
        }

        //Polls
        public void HandleEvent(EntityInsertedEvent<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.PollsPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.PollsPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.PollsPatternKey);
        }

        //Blog posts
        public void HandleEvent(EntityInsertedEvent<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.BlogPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.BlogPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.BlogPatternKey);
        }

        //Blog comments
        public void HandleEvent(EntityDeletedEvent<BlogComment> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.BlogCommentsPatternKey);
        }

        //News items
        public void HandleEvent(EntityInsertedEvent<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.NewsPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.NewsPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.NewsPatternKey);
        }
        //News comments
        public void HandleEvent(EntityDeletedEvent<NewsComment> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.NewsCommentsPatternKey);
        }

        //State/province
        public void HandleEvent(EntityInsertedEvent<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.StateProvincesPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.StateProvincesPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.StateProvincesPatternKey);
        }

        //return requests
        public void HandleEvent(EntityInsertedEvent<ReturnRequestAction> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ReturnRequestActionsPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<ReturnRequestAction> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ReturnRequestActionsPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<ReturnRequestAction> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ReturnRequestActionsPatternKey);
        }
        public void HandleEvent(EntityInsertedEvent<ReturnRequestReason> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ReturnRequestReasonsPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<ReturnRequestReason> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ReturnRequestReasonsPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<ReturnRequestReason> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ReturnRequestReasonsPatternKey);
        }

        //templates
        public void HandleEvent(EntityInsertedEvent<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryTemplatePatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryTemplatePatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CategoryTemplatePatternKey);
        }
        public void HandleEvent(EntityInsertedEvent<ManufacturerTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ManufacturerTemplatePatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<ManufacturerTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ManufacturerTemplatePatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<ManufacturerTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ManufacturerTemplatePatternKey);
        }
        public void HandleEvent(EntityInsertedEvent<ProductTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductTemplatePatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductTemplatePatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<ProductTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.ProductTemplatePatternKey);
        }
        public void HandleEvent(EntityInsertedEvent<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.TopicTemplatePatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.TopicTemplatePatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.TopicTemplatePatternKey);
        }

        //checkout attributes
        public void HandleEvent(EntityInsertedEvent<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CheckoutAttributesPatternKey);
        }
        public void HandleEvent(EntityUpdatedEvent<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CheckoutAttributesPatternKey);
        }
        public void HandleEvent(EntityDeletedEvent<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CheckoutAttributesPatternKey);
        }

        //shopping cart items
        public void HandleEvent(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NopModelCacheDefaults.CartPicturePatternKey);
        }

        //product reviews
        public void HandleEvent(EntityDeletedEvent<ProductReview> eventMessage)
        {
            _cacheManager.RemoveByPattern(string.Format(NopModelCacheDefaults.ProductReviewsPatternKeyById, eventMessage.Entity.ProductId));
        }

        /// <summary>
        /// Handle plugin updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(PluginUpdatedEvent eventMessage)
        {
            if (eventMessage.Plugin?.Instance() is IWidgetPlugin)
                _cacheManager.RemoveByPattern(NopModelCacheDefaults.WidgetPatternKey);
        }

        #endregion
    }
}