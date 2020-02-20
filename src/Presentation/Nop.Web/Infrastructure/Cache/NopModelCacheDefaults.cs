namespace Nop.Web.Infrastructure.Cache
{
    public static partial class NopModelCacheDefaults
    {
        /// <summary>
        /// Key for ManufacturerNavigationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : current manufacturer id
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public static string ManufacturerNavigationModelKey => "Nop.pres.manufacturer.navigation-{0}-{1}-{2}-{3}";
        public static string ManufacturerNavigationPrefixCacheKey => "Nop.pres.manufacturer.navigation";

        /// <summary>
        /// Key for VendorNavigationModel caching
        /// </summary>
        public static string VendorNavigationModelKey => "Nop.pres.vendor.navigation";
        public static string VendorNavigationPrefixCacheKey => "Nop.pres.vendor.navigation";

        /// <summary>
        /// Key for caching of a value indicating whether a manufacturer has featured products
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static string ManufacturerHasFeaturedProductsKey => "Nop.pres.manufacturer.hasfeaturedproducts-{0}-{1}-{2}";
        public static string ManufacturerHasFeaturedProductsPrefixCacheKeyById => "Nop.pres.manufacturer.hasfeaturedproducts-{0}-";

        /// <summary>
        /// Key for list of CategorySimpleModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// </remarks>
        public static string CategoryAllModelKey => "Nop.pres.category.all-{0}-{1}-{2}";
        public static string CategoryAllPrefixCacheKey => "Nop.pres.category.all";

        /// <summary>
        /// Key for caching of a value indicating whether a category has featured products
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static string CategoryHasFeaturedProductsKey => "Nop.pres.category.hasfeaturedproducts-{0}-{1}-{2}";
        public static string CategoryHasFeaturedProductsPrefixCacheKeyById => "Nop.pres.category.hasfeaturedproducts-{0}-";
        
        /// <summary>
        /// Key for caching of categories displayed on home page
        /// </summary>
        /// <remarks>
        /// {0} : picture size
        /// {1} : language ID
        /// {2} : is connection SSL secured (included in a category picture URL)
        /// </remarks>
        public static string CategoryHomepageKey => "Nop.pres.category.homepage-{0}-{1}-{2}";
        public static string CategoryHomepagePrefixCacheKey => "Nop.pres.category.homepage";

        /// <summary>
        /// Key for Xml document of CategorySimpleModels caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// </remarks>
        public static string CategoryXmlAllModelKey => "Nop.pres.categoryXml.all-{0}-{1}-{2}";
        public static string CategoryXmlAllPrefixCacheKey => "Nop.pres.categoryXml.all";

        /// <summary>
        /// Key for SpecificationAttributeOptionFilter caching
        /// </summary>
        /// <remarks>
        /// {0} : comma separated list of specification attribute option IDs
        /// {1} : language id
        /// </remarks>
        public static string SpecsFilterModelKey => "Nop.pres.filter.specs-{0}-{1}";
        public static string SpecsFilterPrefixCacheKey => "Nop.pres.filter.specs";

        /// <summary>
        /// Key for bestsellers identifiers displayed on the home page
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public static string HomepageBestsellersIdsKey => "Nop.pres.bestsellers.homepage-{0}";
        public static string HomepageBestsellersIdsPrefixCacheKey => "Nop.pres.bestsellers.homepage";

        /// <summary>
        /// Key for "also purchased" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : current store ID
        /// </remarks>
        public static string ProductsAlsoPurchasedIdsKey => "Nop.pres.alsopuchased-{0}-{1}";
        public static string ProductsAlsoPurchasedIdsPrefixCacheKey => "Nop.pres.alsopuchased";

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
        public static string ProductDefaultPictureModelKey => "Nop.pres.product.detailspictures-{0}-{1}-{2}-{3}-{4}-{5}";
        public static string ProductDefaultPicturePrefixCacheKey => "Nop.pres.product.detailspictures";
        public static string ProductDefaultPicturePrefixCacheKeyById => "Nop.pres.product.detailspictures-{0}-";

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
        public static string ProductDetailsPicturesModelKey => "Nop.pres.product.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public static string ProductDetailsPicturesPrefixCacheKey => "Nop.pres.product.picture";
        public static string ProductDetailsPicturesPrefixCacheKeyById => "Nop.pres.product.picture-{0}-";

        /// <summary>
        /// Key for product reviews caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : current store ID
        /// </remarks>
        public static string ProductReviewsModelKey => "Nop.pres.product.reviews-{0}-{1}";
        public static string ProductReviewsPrefixCacheKey => "Nop.pres.product.reviews";
        public static string ProductReviewsPrefixCacheKeyById => "Nop.pres.product.reviews-{0}-";

        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public static string ProductAttributePictureModelKey => "Nop.pres.productattribute.picture-{0}-{1}-{2}";
        public static string ProductAttributePicturePrefixCacheKey => "Nop.pres.productattribute.picture";

        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public static string ProductAttributeImageSquarePictureModelKey => "Nop.pres.productattribute.imagesquare.picture-{0}-{1}-{2}";
        public static string ProductAttributeImageSquarePicturePrefixCacheKey => "Nop.pres.productattribute.imagesquare.picture";

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
        public static string CategoryPictureModelKey => "Nop.pres.category.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public static string CategoryPicturePrefixCacheKey => "Nop.pres.category.picture";
        public static string CategoryPicturePrefixCacheKeyById => "Nop.pres.category.picture-{0}-";

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
        public static string ManufacturerPictureModelKey => "Nop.pres.manufacturer.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public static string ManufacturerPicturePrefixCacheKey => "Nop.pres.manufacturer.picture";
        public static string ManufacturerPicturePrefixCacheKeyById => "Nop.pres.manufacturer.picture-{0}-";

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
        public static string VendorPictureModelKey => "Nop.pres.vendor.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public static string VendorPicturePrefixCacheKey => "Nop.pres.vendor.picture";
        public static string VendorPicturePrefixCacheKeyById => "Nop.pres.vendor.picture-{0}-";

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
        public static string CartPictureModelKey => "Nop.pres.cart.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public static string CartPicturePrefixCacheKey => "Nop.pres.cart.picture";

        /// <summary>
        /// Key for home page polls
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static string HomepagePollsModelKey => "Nop.pres.poll.homepage-{0}-{1}";
        /// <summary>
        /// Key for polls by system name
        /// </summary>
        /// <remarks>
        /// {0} : poll system name
        /// {1} : language ID
        /// {2} : current store ID
        /// </remarks>
        public static string PollBySystemNameModelKey => "Nop.pres.poll.systemname-{0}-{1}-{2}";
        public static string PollsPrefixCacheKey => "Nop.pres.poll";

        /// <summary>
        /// Key for blog archive (years, months) block model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static string BlogMonthsModelKey => "Nop.pres.blog.months-{0}-{1}";
        public static string BlogPrefixCacheKey => "Nop.pres.blog";
        
        /// <summary>
        /// Key for home page news
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static string HomepageNewsModelKey => "Nop.pres.news.homepage-{0}-{1}";
        public static string NewsPrefixCacheKey => "Nop.pres.news";

        /// <summary>
        /// Key for logo
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : current theme
        /// {2} : is connection SSL secured (included in a picture URL)
        /// </remarks>
        public static string StoreLogoPath => "Nop.pres.logo-{0}-{1}-{2}";
        public static string StoreLogoPathPrefixCacheKey => "Nop.pres.logo";

        /// <summary>
        /// Key for sitemap on the sitemap page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static string SitemapPageModelKey => "Nop.pres.sitemap.page-{0}-{1}-{2}";
        /// <summary>
        /// Key for sitemap on the sitemap SEO page
        /// </summary>
        /// <remarks>
        /// {0} : sitemap identifier
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public static string SitemapSeoModelKey => "Nop.pres.sitemap.seo-{0}-{1}-{2}-{3}";
        public static string SitemapPrefixCacheKey => "Nop.pres.sitemap";

        /// <summary>
        /// Key for widget info
        /// </summary>
        /// <remarks>
        /// {0} : current customer ID
        /// {1} : current store ID
        /// {2} : widget zone
        /// {3} : current theme name
        /// </remarks>
        public static string WidgetModelKey => "Nop.pres.widget-{0}-{1}-{2}-{3}";
        public static string WidgetPrefixCacheKey => "Nop.pres.widget";

    }
}
