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
using Nop.Core.Infrastructure;
using Nop.Services.Events;

namespace Nop.Web.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer: 
        //languages
        IConsumer<EntityInserted<Language>>,
        IConsumer<EntityUpdated<Language>>,
        IConsumer<EntityDeleted<Language>>,
        //currencies
        IConsumer<EntityInserted<Currency>>,
        IConsumer<EntityUpdated<Currency>>,
        IConsumer<EntityDeleted<Currency>>,
        //settings
        IConsumer<EntityUpdated<Setting>>,
        //manufacturers
        IConsumer<EntityInserted<Manufacturer>>,
        IConsumer<EntityUpdated<Manufacturer>>,
        IConsumer<EntityDeleted<Manufacturer>>,
        //vendors
        IConsumer<EntityInserted<Vendor>>,
        IConsumer<EntityUpdated<Vendor>>,
        IConsumer<EntityDeleted<Vendor>>,
        //product manufacturers
        IConsumer<EntityInserted<ProductManufacturer>>,
        IConsumer<EntityUpdated<ProductManufacturer>>,
        IConsumer<EntityDeleted<ProductManufacturer>>,
        //categories
        IConsumer<EntityInserted<Category>>,
        IConsumer<EntityUpdated<Category>>,
        IConsumer<EntityDeleted<Category>>,
        //product categories
        IConsumer<EntityInserted<ProductCategory>>,
        IConsumer<EntityUpdated<ProductCategory>>,
        IConsumer<EntityDeleted<ProductCategory>>,
        //products
        IConsumer<EntityInserted<Product>>,
        IConsumer<EntityUpdated<Product>>,
        IConsumer<EntityDeleted<Product>>,
        //related product
        IConsumer<EntityInserted<RelatedProduct>>,
        IConsumer<EntityUpdated<RelatedProduct>>,
        IConsumer<EntityDeleted<RelatedProduct>>,
        //product tags
        IConsumer<EntityInserted<ProductTag>>,
        IConsumer<EntityUpdated<ProductTag>>,
        IConsumer<EntityDeleted<ProductTag>>,
        //specification attributes
        IConsumer<EntityUpdated<SpecificationAttribute>>,
        IConsumer<EntityDeleted<SpecificationAttribute>>,
        //specification attribute options
        IConsumer<EntityUpdated<SpecificationAttributeOption>>,
        IConsumer<EntityDeleted<SpecificationAttributeOption>>,
        //Product specification attribute
        IConsumer<EntityInserted<ProductSpecificationAttribute>>,
        IConsumer<EntityUpdated<ProductSpecificationAttribute>>,
        IConsumer<EntityDeleted<ProductSpecificationAttribute>>,
        //Product attributes
        IConsumer<EntityDeleted<ProductAttribute>>,
        //Product attributes
        IConsumer<EntityInserted<ProductAttributeMapping>>,
        IConsumer<EntityDeleted<ProductAttributeMapping>>,
        //Product attribute values
        IConsumer<EntityUpdated<ProductAttributeValue>>,
        //Topics
        IConsumer<EntityInserted<Topic>>,
        IConsumer<EntityUpdated<Topic>>,
        IConsumer<EntityDeleted<Topic>>,
        //Orders
        IConsumer<EntityInserted<Order>>,
        IConsumer<EntityUpdated<Order>>,
        IConsumer<EntityDeleted<Order>>,
        //Picture
        IConsumer<EntityInserted<Picture>>,
        IConsumer<EntityUpdated<Picture>>,
        IConsumer<EntityDeleted<Picture>>,
        //Product picture mapping
        IConsumer<EntityInserted<ProductPicture>>,
        IConsumer<EntityUpdated<ProductPicture>>,
        IConsumer<EntityDeleted<ProductPicture>>,
        //Product review
        IConsumer<EntityDeleted<ProductReview>>,
        //polls
        IConsumer<EntityInserted<Poll>>,
        IConsumer<EntityUpdated<Poll>>,
        IConsumer<EntityDeleted<Poll>>,
        //blog posts
        IConsumer<EntityInserted<BlogPost>>,
        IConsumer<EntityUpdated<BlogPost>>,
        IConsumer<EntityDeleted<BlogPost>>,
        //news items
        IConsumer<EntityInserted<NewsItem>>,
        IConsumer<EntityUpdated<NewsItem>>,
        IConsumer<EntityDeleted<NewsItem>>,
        //states/province
        IConsumer<EntityInserted<StateProvince>>,
        IConsumer<EntityUpdated<StateProvince>>,
        IConsumer<EntityDeleted<StateProvince>>,
        //return requests
        IConsumer<EntityInserted<ReturnRequestAction>>,
        IConsumer<EntityUpdated<ReturnRequestAction>>,
        IConsumer<EntityDeleted<ReturnRequestAction>>,
        IConsumer<EntityInserted<ReturnRequestReason>>,
        IConsumer<EntityUpdated<ReturnRequestReason>>,
        IConsumer<EntityDeleted<ReturnRequestReason>>,
        //templates
        IConsumer<EntityInserted<CategoryTemplate>>,
        IConsumer<EntityUpdated<CategoryTemplate>>,
        IConsumer<EntityDeleted<CategoryTemplate>>,
        IConsumer<EntityInserted<ManufacturerTemplate>>,
        IConsumer<EntityUpdated<ManufacturerTemplate>>,
        IConsumer<EntityDeleted<ManufacturerTemplate>>,
        IConsumer<EntityInserted<ProductTemplate>>,
        IConsumer<EntityUpdated<ProductTemplate>>,
        IConsumer<EntityDeleted<ProductTemplate>>,
        IConsumer<EntityInserted<TopicTemplate>>,
        IConsumer<EntityUpdated<TopicTemplate>>,
        IConsumer<EntityDeleted<TopicTemplate>>,
        //checkout attributes
        IConsumer<EntityInserted<CheckoutAttribute>>,
        IConsumer<EntityUpdated<CheckoutAttribute>>,
        IConsumer<EntityDeleted<CheckoutAttribute>>,
        //shopping cart items
        IConsumer<EntityUpdated<ShoppingCartItem>>
    {

        #region Cache keys 

        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public ModelCacheEventConsumer()
        {
            //TODO inject static cache manager using constructor
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        }
        
        #endregion 

        #region Cache keys 

        /// <summary>
        /// Key for categories on the search page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string SEARCH_CATEGORIES_MODEL_KEY = "Nop.pres.search.categories-{0}-{1}-{2}";
        public const string SEARCH_CATEGORIES_PATTERN_KEY = "Nop.pres.search.categories";

        /// <summary>
        /// Key for ManufacturerNavigationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : current manufacturer id
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public const string MANUFACTURER_NAVIGATION_MODEL_KEY = "Nop.pres.manufacturer.navigation-{0}-{1}-{2}-{3}";
        public const string MANUFACTURER_NAVIGATION_PATTERN_KEY = "Nop.pres.manufacturer.navigation";

        /// <summary>
        /// Key for VendorNavigationModel caching
        /// </summary>
        public const string VENDOR_NAVIGATION_MODEL_KEY = "Nop.pres.vendor.navigation";
        public const string VENDOR_NAVIGATION_PATTERN_KEY = "Nop.pres.vendor.navigation";

        /// <summary>
        /// Key for caching of a value indicating whether a manufacturer has featured products
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string MANUFACTURER_HAS_FEATURED_PRODUCTS_KEY = "Nop.pres.manufacturer.hasfeaturedproducts-{0}-{1}-{2}";
        public const string MANUFACTURER_HAS_FEATURED_PRODUCTS_PATTERN_KEY = "Nop.pres.manufacturer.hasfeaturedproducts";
        
        /// <summary>
        /// Key for CategoryNavigationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// </remarks>
        public const string CATEGORY_NAVIGATION_MODEL_KEY = "Nop.pres.category.navigation-{0}-{1}-{2}";
        public const string CATEGORY_NAVIGATION_PATTERN_KEY = "Nop.pres.category.navigation";

        /// <summary>
        /// Key for TopMenuModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// </remarks>
        public const string CATEGORY_MENU_MODEL_KEY = "Nop.pres.category.menu-{0}-{1}-{2}";
        public const string CATEGORY_MENU_PATTERN_KEY = "Nop.pres.category.menu";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : comma separated list of customer roles
        /// {1} : current store ID
        /// {2} : category ID
        /// </remarks>
        public const string CATEGORY_NUMBER_OF_PRODUCTS_MODEL_KEY = "Nop.pres.category.numberofproducts-{0}-{1}-{2}";
        public const string CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY = "Nop.pres.category.numberofproducts";

        /// <summary>
        /// Key for caching of a value indicating whether a category has featured products
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string CATEGORY_HAS_FEATURED_PRODUCTS_KEY = "Nop.pres.category.hasfeaturedproducts-{0}-{1}-{2}";
        public const string CATEGORY_HAS_FEATURED_PRODUCTS_PATTERN_KEY = "Nop.pres.category.hasfeaturedproducts";

        /// <summary>
        /// Key for caching of category breadcrumb
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// </remarks>
        public const string CATEGORY_BREADCRUMB_KEY = "Nop.pres.category.breadcrumb-{0}-{1}-{2}-{3}";
        public const string CATEGORY_BREADCRUMB_PATTERN_KEY = "Nop.pres.category.breadcrumb";

        /// <summary>
        /// Key for caching of subcategories of certain category
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// {4} : is connection SSL secured (included in a category picture URL)
        /// </remarks>
        public const string CATEGORY_SUBCATEGORIES_KEY = "Nop.pres.category.subcategories-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CATEGORY_SUBCATEGORIES_PATTERN_KEY = "Nop.pres.category.subcategories";

        /// <summary>
        /// Key for caching of categories displayed on home page
        /// </summary>
        /// <remarks>
        /// {0} : roles of the current user
        /// {1} : current store ID
        /// {2} : language ID
        /// {3} : is connection SSL secured (included in a category picture URL)
        /// </remarks>
        public const string CATEGORY_HOMEPAGE_KEY = "Nop.pres.category.homepage-{0}-{1}-{2}-{3}-{4}";
        public const string CATEGORY_HOMEPAGE_PATTERN_KEY = "Nop.pres.category.homepage";
        
        /// <summary>
        /// Key for GetChildCategoryIds method results caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// </remarks>
        public const string CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY = "Nop.pres.category.childidentifiers-{0}-{1}-{2}";
        public const string CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY = "Nop.pres.category.childidentifiers";

        /// <summary>
        /// Key for SpecificationAttributeOptionFilter caching
        /// </summary>
        /// <remarks>
        /// {0} : comma separated list of specification attribute option IDs
        /// {1} : language id
        /// </remarks>
        public const string SPECS_FILTER_MODEL_KEY = "Nop.pres.filter.specs-{0}-{1}";
        public const string SPECS_FILTER_PATTERN_KEY = "Nop.pres.filter.specs";

        /// <summary>
        /// Key for ProductBreadcrumbModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : comma separated list of customer roles
        /// {3} : current store ID
        /// </remarks>
        public const string PRODUCT_BREADCRUMB_MODEL_KEY = "Nop.pres.product.breadcrumb-{0}-{1}-{2}-{3}";
        public const string PRODUCT_BREADCRUMB_PATTERN_KEY = "Nop.pres.product.breadcrumb";

        /// <summary>
        /// Key for ProductTagModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : current store ID
        /// </remarks>
        public const string PRODUCTTAG_BY_PRODUCT_MODEL_KEY = "Nop.pres.producttag.byproduct-{0}-{1}-{2}";
        public const string PRODUCTTAG_BY_PRODUCT_PATTERN_KEY = "Nop.pres.producttag.byproduct";

        /// <summary>
        /// Key for PopularProductTagsModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// </remarks>
        public const string PRODUCTTAG_POPULAR_MODEL_KEY = "Nop.pres.producttag.popular-{0}-{1}";
        public const string PRODUCTTAG_POPULAR_PATTERN_KEY = "Nop.pres.producttag.popular";

        /// <summary>
        /// Key for ProductManufacturers model caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public const string PRODUCT_MANUFACTURERS_MODEL_KEY = "Nop.pres.product.manufacturers-{0}-{1}-{2}-{3}";
        public const string PRODUCT_MANUFACTURERS_PATTERN_KEY = "Nop.pres.product.manufacturers";

        /// <summary>
        /// Key for ProductSpecificationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// </remarks>
        public const string PRODUCT_SPECS_MODEL_KEY = "Nop.pres.product.specs-{0}-{1}";
        public const string PRODUCT_SPECS_PATTERN_KEY = "Nop.pres.product.specs";

        /// <summary>
        /// Key for caching of a value indicating whether a product has product attributes
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// </remarks>
        public const string PRODUCT_HAS_PRODUCT_ATTRIBUTES_KEY = "Nop.pres.product.hasproductattributes-{0}";
        public const string PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY = "Nop.pres.product.hasproductattributes";

        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : language id
        /// {2} : store id
        /// {3} : comma separated list of customer roles
        /// </remarks>
        public const string TOPIC_MODEL_BY_SYSTEMNAME_KEY = "Nop.pres.topic.details.bysystemname-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic id
        /// {1} : language id
        /// {2} : store id
        /// {3} : comma separated list of customer roles
        /// </remarks>
        public const string TOPIC_MODEL_BY_ID_KEY = "Nop.pres.topic.details.byid-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : language id
        /// {2} : store id
        /// </remarks>
        public const string TOPIC_SENAME_BY_SYSTEMNAME = "Nop.pres.topic.sename.bysystemname-{0}-{1}-{2}";
        /// <summary>
        /// Key for TopMenuModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// {2} : comma separated list of customer roles
        /// </remarks>
        public const string TOPIC_TOP_MENU_MODEL_KEY = "Nop.pres.topic.topmenu-{0}-{1}-{2}";
        /// <summary>
        /// Key for TopMenuModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// {2} : comma separated list of customer roles
        /// </remarks>
        public const string TOPIC_FOOTER_MODEL_KEY = "Nop.pres.topic.footer-{0}-{1}-{2}";
        public const string TOPIC_PATTERN_KEY = "Nop.pres.topic";

        /// <summary>
        /// Key for CategoryTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : category template id
        /// </remarks>
        public const string CATEGORY_TEMPLATE_MODEL_KEY = "Nop.pres.categorytemplate-{0}";
        public const string CATEGORY_TEMPLATE_PATTERN_KEY = "Nop.pres.categorytemplate";

        /// <summary>
        /// Key for ManufacturerTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer template id
        /// </remarks>
        public const string MANUFACTURER_TEMPLATE_MODEL_KEY = "Nop.pres.manufacturertemplate-{0}";
        public const string MANUFACTURER_TEMPLATE_PATTERN_KEY = "Nop.pres.manufacturertemplate";

        /// <summary>
        /// Key for ProductTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : product template id
        /// </remarks>
        public const string PRODUCT_TEMPLATE_MODEL_KEY = "Nop.pres.producttemplate-{0}";
        public const string PRODUCT_TEMPLATE_PATTERN_KEY = "Nop.pres.producttemplate";

        /// <summary>
        /// Key for TopicTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : topic template id
        /// </remarks>
        public const string TOPIC_TEMPLATE_MODEL_KEY = "Nop.pres.topictemplate-{0}";
        public const string TOPIC_TEMPLATE_PATTERN_KEY = "Nop.pres.topictemplate";

        /// <summary>
        /// Key for bestsellers identifiers displayed on the home page
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public const string HOMEPAGE_BESTSELLERS_IDS_KEY = "Nop.pres.bestsellers.homepage-{0}";
        public const string HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY = "Nop.pres.bestsellers.homepage";

        /// <summary>
        /// Key for "also purchased" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : current store ID
        /// </remarks>
        public const string PRODUCTS_ALSO_PURCHASED_IDS_KEY = "Nop.pres.alsopuchased-{0}-{1}";
        public const string PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY = "Nop.pres.alsopuchased";

        /// <summary>
        /// Key for "related" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : current store ID
        /// </remarks>
        public const string PRODUCTS_RELATED_IDS_KEY = "Nop.pres.related-{0}-{1}";
        public const string PRODUCTS_RELATED_IDS_PATTERN_KEY = "Nop.pres.related";

        /// <summary>
        /// Key for default product picture caching (all pictures)
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : picture size
        /// {2} : isAssociatedProduct?
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string PRODUCT_DEFAULTPICTURE_MODEL_KEY = "Nop.pres.product.detailspictures-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string PRODUCT_DEFAULTPICTURE_PATTERN_KEY = "Nop.pres.product.detailspictures";

        /// <summary>
        /// Key for product picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string PRODUCT_DETAILS_PICTURES_MODEL_KEY = "Nop.pres.product.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string PRODUCT_DETAILS_PICTURES_PATTERN_KEY = "Nop.pres.product.picture";

        /// <summary>
        /// Key for product reviews caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : current store ID
        /// </remarks>
        public const string PRODUCT_REVIEWS_MODEL_KEY = "Nop.pres.product.reviews-{0}-{1}";
        public const string PRODUCT_REVIEWS_PATTERN_KEY = "Nop.pres.product.reviews";

        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public const string PRODUCTATTRIBUTE_PICTURE_MODEL_KEY = "Nop.pres.productattribute.picture-{0}-{1}-{2}";
        public const string PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY = "Nop.pres.productattribute.picture";

        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public const string PRODUCTATTRIBUTE_IMAGESQUARE_PICTURE_MODEL_KEY = "Nop.pres.productattribute.imagesquare.picture-{0}-{1}-{2}";
        public const string PRODUCTATTRIBUTE_IMAGESQUARE_PICTURE_PATTERN_KEY = "Nop.pres.productattribute.imagesquare.picture";

        /// <summary>
        /// Key for category picture caching
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized category name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string CATEGORY_PICTURE_MODEL_KEY = "Nop.pres.category.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CATEGORY_PICTURE_PATTERN_KEY = "Nop.pres.category.picture";

        /// <summary>
        /// Key for manufacturer picture caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized manufacturer name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string MANUFACTURER_PICTURE_MODEL_KEY = "Nop.pres.manufacturer.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string MANUFACTURER_PICTURE_PATTERN_KEY = "Nop.pres.manufacturer.picture";

        /// <summary>
        /// Key for vendor picture caching
        /// </summary>
        /// <remarks>
        /// {0} : vendor id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized category name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string VENDOR_PICTURE_MODEL_KEY = "Nop.pres.vendor.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string VENDOR_PICTURE_PATTERN_KEY = "Nop.pres.vendor.picture";

        /// <summary>
        /// Key for cart picture caching
        /// </summary>
        /// <remarks>
        /// {0} : shopping cart item id
        /// P.S. we could cache by product ID. it could increase performance.
        /// but it won't work for product attributes with custom images
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string CART_PICTURE_MODEL_KEY = "Nop.pres.cart.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CART_PICTURE_PATTERN_KEY = "Nop.pres.cart.picture";

        /// <summary>
        /// Key for home page polls
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public const string HOMEPAGE_POLLS_MODEL_KEY = "Nop.pres.poll.homepage-{0}";
        /// <summary>
        /// Key for polls by system name
        /// </summary>
        /// <remarks>
        /// {0} : poll system name
        /// {1} : language ID
        /// </remarks>
        public const string POLL_BY_SYSTEMNAME__MODEL_KEY = "Nop.pres.poll.systemname-{0}-{1}";
        public const string POLLS_PATTERN_KEY = "Nop.pres.poll";

        /// <summary>
        /// Key for blog tag list model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string BLOG_TAGS_MODEL_KEY = "Nop.pres.blog.tags-{0}-{1}";
        /// <summary>
        /// Key for blog archive (years, months) block model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string BLOG_MONTHS_MODEL_KEY = "Nop.pres.blog.months-{0}-{1}";
        public const string BLOG_PATTERN_KEY = "Nop.pres.blog";

        /// <summary>
        /// Key for home page news
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string HOMEPAGE_NEWSMODEL_KEY = "Nop.pres.news.homepage-{0}-{1}";
        public const string NEWS_PATTERN_KEY = "Nop.pres.news";
        
        /// <summary>
        /// Key for states by country id
        /// </summary>
        /// <remarks>
        /// {0} : country ID
        /// {1} : "empty" or "select" item
        /// {2} : language ID
        /// </remarks>
        public const string STATEPROVINCES_BY_COUNTRY_MODEL_KEY = "Nop.pres.stateprovinces.bycountry-{0}-{1}-{2}";
        public const string STATEPROVINCES_PATTERN_KEY = "Nop.pres.stateprovinces";

        /// <summary>
        /// Key for return request reasons
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public const string RETURNREQUESTREASONS_MODEL_KEY = "Nop.pres.returnrequesreasons-{0}";
        public const string RETURNREQUESTREASONS_PATTERN_KEY = "Nop.pres.returnrequesreasons";

        /// <summary>
        /// Key for return request actions
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public const string RETURNREQUESTACTIONS_MODEL_KEY = "Nop.pres.returnrequestactions-{0}";
        public const string RETURNREQUESTACTIONS_PATTERN_KEY = "Nop.pres.returnrequestactions";

        /// <summary>
        /// Key for logo
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : current theme
        /// {2} : is connection SSL secured (included in a picture URL)
        /// </remarks>
        public const string STORE_LOGO_PATH = "Nop.pres.logo-{0}-{1}-{2}";
        public const string STORE_LOGO_PATH_PATTERN_KEY = "Nop.pres.logo";

        /// <summary>
        /// Key for available languages
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public const string AVAILABLE_LANGUAGES_MODEL_KEY = "Nop.pres.languages.all-{0}";
        public const string AVAILABLE_LANGUAGES_PATTERN_KEY = "Nop.pres.languages";

        /// <summary>
        /// Key for available currencies
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string AVAILABLE_CURRENCIES_MODEL_KEY = "Nop.pres.currencies.all-{0}-{1}";
        public const string AVAILABLE_CURRENCIES_PATTERN_KEY = "Nop.pres.currencies";

        /// <summary>
        /// Key for caching of a value indicating whether we have checkout attributes
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : true - all attributes, false - only shippable attributes
        /// </remarks>
        public const string CHECKOUTATTRIBUTES_EXIST_KEY = "Nop.pres.checkoutattributes.exist-{0}-{1}";
        public const string CHECKOUTATTRIBUTES_PATTERN_KEY = "Nop.pres.checkoutattributes";

        /// <summary>
        /// Key for sitemap on the sitemap page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string SITEMAP_PAGE_MODEL_KEY = "Nop.pres.sitemap.page-{0}-{1}-{2}";
        /// <summary>
        /// Key for sitemap on the sitemap SEO page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string SITEMAP_SEO_MODEL_KEY = "Nop.pres.sitemap.seo-{0}-{1}-{2}";
        public const string SITEMAP_PATTERN_KEY = "Nop.pres.sitemap";

        /// <summary>
        /// Key for widget info
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : widget zone
        /// {2} : current theme name
        /// </remarks>
        public const string WIDGET_MODEL_KEY = "Nop.pres.widget-{0}-{1}-{2}";
        public const string WIDGET_PATTERN_KEY = "Nop.pres.widget";

        #endregion

        #region Methods

        //languages
        public void HandleEvent(EntityInserted<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        
        //currencies
        public void HandleEvent(EntityInserted<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            //clear models which depend on settings
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY); //depends on CatalogSettings.NumberOfProductTags
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY); //depends on CatalogSettings.ManufacturersBlockItemsToDisplay
            _cacheManager.RemoveByPattern(VENDOR_NAVIGATION_PATTERN_KEY); //depends on VendorSettings.VendorBlockItemsToDisplay
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY); //depends on CatalogSettings.ShowCategoryProductNumber and CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            _cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY); //depends on CatalogSettings.ShowCategoryProductNumber and CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY); //depends on CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY); //depends on CatalogSettings.NumberOfBestsellersOnHomepage
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY); //depends on CatalogSettings.ProductsAlsoPurchasedNumber
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY); //depends on BlogSettings.NumberOfTags
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY); //depends on NewsSettings.MainPageNewsCount
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY); //depends on distinct sitemap settings
            _cacheManager.RemoveByPattern(WIDGET_PATTERN_KEY); //depends on WidgetSettings and certain settings of widgets
            _cacheManager.RemoveByPattern(STORE_LOGO_PATH_PATTERN_KEY); //depends on StoreInformationSettings.LogoPictureId
        }

        //vendors
        public void HandleEvent(EntityInserted<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(VENDOR_NAVIGATION_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(VENDOR_NAVIGATION_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(VENDOR_NAVIGATION_PATTERN_KEY);
        }

        //manufacturers
        public void HandleEvent(EntityInserted<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }

        //product manufacturers
        public void HandleEvent(EntityInserted<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_HAS_FEATURED_PRODUCTS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_HAS_FEATURED_PRODUCTS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_HAS_FEATURED_PRODUCTS_PATTERN_KEY);
        }
        
        //categories
         public void HandleEvent(EntityInserted<Category> eventMessage)
        {
             _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
             _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY); 
             _cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY);
             _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
             _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
             _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
             _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }

        //product categories
        public void HandleEvent(EntityInserted<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY); 
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HAS_FEATURED_PRODUCTS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HAS_FEATURED_PRODUCTS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HAS_FEATURED_PRODUCTS_PATTERN_KEY);
         }

        //products
        public void HandleEvent(EntityInserted<Product> eventMessage)
        {
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Product> eventMessage)
        {
            //_cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            //_cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            //_cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY);
            //_cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_REVIEWS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Product> eventMessage)
        {
            //_cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            //_cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY);
            //_cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        
        //product tags
        public void HandleEvent(EntityInserted<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTTAG_BY_PRODUCT_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTTAG_BY_PRODUCT_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTTAG_BY_PRODUCT_PATTERN_KEY);
        }
        
        //related products
        public void HandleEvent(EntityInserted<RelatedProduct> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<RelatedProduct> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<RelatedProduct> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
        }
        
        //specification attributes
        public void HandleEvent(EntityUpdated<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        
        //specification attribute options
        public void HandleEvent(EntityUpdated<SpecificationAttributeOption> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<SpecificationAttributeOption> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        
        //Product specification attribute
        public void HandleEvent(EntityInserted<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
        }
        
        //Product attributes
        public void HandleEvent(EntityDeleted<ProductAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY);
        }
        //Product attributes
        public void HandleEvent(EntityInserted<ProductAttributeMapping> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ProductAttributeMapping> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY);
        }
        //Product attributes
        public void HandleEvent(EntityUpdated<ProductAttributeValue> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_IMAGESQUARE_PICTURE_PATTERN_KEY);
        }

        //Topics
        public void HandleEvent(EntityInserted<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        
        //Orders
        public void HandleEvent(EntityInserted<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY);
        }

        //Pictures
        public void HandleEvent(EntityInserted<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_DETAILS_PICTURES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDOR_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_DETAILS_PICTURES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDOR_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_DETAILS_PICTURES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDOR_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }

        //Product picture mappings
        public void HandleEvent(EntityInserted<ProductPicture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_DETAILS_PICTURES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ProductPicture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_DETAILS_PICTURES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ProductPicture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_DETAILS_PICTURES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }

        //Polls
        public void HandleEvent(EntityInserted<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
        }

        //Blog posts
        public void HandleEvent(EntityInserted<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY);
        }

        //News items
        public void HandleEvent(EntityInserted<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);
        }

        //State/province
        public void HandleEvent(EntityInserted<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }

        //return requests
        public void HandleEvent(EntityInserted<ReturnRequestAction> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTACTIONS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ReturnRequestAction> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTACTIONS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ReturnRequestAction> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTACTIONS_PATTERN_KEY);
        }
        public void HandleEvent(EntityInserted<ReturnRequestReason> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTREASONS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ReturnRequestReason> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTREASONS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ReturnRequestReason> eventMessage)
        {
            _cacheManager.RemoveByPattern(RETURNREQUESTREASONS_PATTERN_KEY);
        }

        //templates
        public void HandleEvent(EntityInserted<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityInserted<ManufacturerTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ManufacturerTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ManufacturerTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityInserted<ProductTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<ProductTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<ProductTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityInserted<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_TEMPLATE_PATTERN_KEY);
        }

        //checkout attributes
        public void HandleEvent(EntityInserted<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
        }

        //shopping cart items
        public void HandleEvent(EntityUpdated<ShoppingCartItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }

        //product reviews
        public void HandleEvent(EntityDeleted<ProductReview> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_REVIEWS_PATTERN_KEY);
        }

        #endregion
    }
}
