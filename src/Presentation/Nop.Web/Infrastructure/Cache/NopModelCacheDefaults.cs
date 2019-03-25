using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Infrastructure.Cache
{
    public static partial class NopModelCacheDefaults
    {
        /// <summary>
        /// Key for categories on the search page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static string SearchCategoriesModelKey => "Nop.pres.search.categories-{0}-{1}-{2}";
        public static string SearchCategoriesPatternKey => "Nop.pres.search.categories";

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
        public static string ManufacturerNavigationPatternKey => "Nop.pres.manufacturer.navigation";

        /// <summary>
        /// Key for VendorNavigationModel caching
        /// </summary>
        public static string VendorNavigationModelKey => "Nop.pres.vendor.navigation";
        public static string VendorNavigationPatternKey => "Nop.pres.vendor.navigation";

        /// <summary>
        /// Key for caching of a value indicating whether a manufacturer has featured products
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static string ManufacturerHasFeaturedProductsKey => "Nop.pres.manufacturer.hasfeaturedproducts-{0}-{1}-{2}";
        public static string ManufacturerHasFeaturedProductsPatternKeyById => "Nop.pres.manufacturer.hasfeaturedproducts-{0}-";

        /// <summary>
        /// Key for list of CategorySimpleModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// </remarks>
        public static string CategoryAllModelKey => "Nop.pres.category.all-{0}-{1}-{2}";
        public static string CategoryAllPatternKey => "Nop.pres.category.all";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : comma separated list of customer roles
        /// {1} : current store ID
        /// {2} : category ID
        /// </remarks>
        public static string CategoryNumberOfProductsModelKey => "Nop.pres.category.numberofproducts-{0}-{1}-{2}";
        public static string CategoryNumberOfProductsPatternKey => "Nop.pres.category.numberofproducts";

        /// <summary>
        /// Key for caching of a value indicating whether a category has featured products
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static string CategoryHasFeaturedProductsKey => "Nop.pres.category.hasfeaturedproducts-{0}-{1}-{2}";
        public static string CategoryHasFeaturedProductsPatternKeyById => "Nop.pres.category.hasfeaturedproducts-{0}-";

        /// <summary>
        /// Key for caching of category breadcrumb
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// </remarks>
        public static string CategoryBreadcrumbKey => "Nop.pres.category.breadcrumb-{0}-{1}-{2}-{3}";
        public static string CategoryBreadcrumbPatternKey => "Nop.pres.category.breadcrumb";

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
        public static string CategorySubcategoriesKey => "Nop.pres.category.subcategories-{0}-{1}-{2}-{3}-{4}-{5}";
        public static string CategorySubcategoriesPatternKey => "Nop.pres.category.subcategories";

        /// <summary>
        /// Key for caching of categories displayed on home page
        /// </summary>
        /// <remarks>
        /// {0} : roles of the current user
        /// {1} : current store ID
        /// {2} : language ID
        /// {3} : is connection SSL secured (included in a category picture URL)
        /// </remarks>
        public static string CategoryHomepageKey => "Nop.pres.category.homepage-{0}-{1}-{2}-{3}-{4}";
        public static string CategoryHomepagePatternKey => "Nop.pres.category.homepage";

        /// <summary>
        /// Key for Xml document of CategorySimpleModels caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// </remarks>
        public const string CategoryXmlAllModelKey = "Nop.pres.categoryXml.all-{0}-{1}-{2}";
        public const string CategoryXmlAllPatternKey = "Nop.pres.categoryXml.all";

        /// <summary>
        /// Key for SpecificationAttributeOptionFilter caching
        /// </summary>
        /// <remarks>
        /// {0} : comma separated list of specification attribute option IDs
        /// {1} : language id
        /// </remarks>
        public static string SpecsFilterModelKey => "Nop.pres.filter.specs-{0}-{1}";
        public static string SpecsFilterPatternKey => "Nop.pres.filter.specs";

        /// <summary>
        /// Key for ProductBreadcrumbModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : comma separated list of customer roles
        /// {3} : current store ID
        /// </remarks>
        public static string ProductBreadcrumbModelKey => "Nop.pres.product.breadcrumb-{0}-{1}-{2}-{3}";
        public static string ProductBreadcrumbPatternKey => "Nop.pres.product.breadcrumb";
        public static string ProductBreadcrumbPatternKeyById => "Nop.pres.product.breadcrumb-{0}-";

        /// <summary>
        /// Key for ProductTagModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : comma separated list of customer roles
        /// {3} : current store ID
        /// </remarks>
        public static string ProductTagByProductModelKey => "Nop.pres.producttag.byproduct-{0}-{1}-{2}-{3}";
        public static string ProductTagByProductPatternKey => "Nop.pres.producttag.byproduct";

        /// <summary>
        /// Key for PopularProductTagsModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// </remarks>
        public static string ProductTagPopularModelKey => "Nop.pres.producttag.popular-{0}-{1}";
        public static string ProductTagPopularPatternKey => "Nop.pres.producttag.popular";

        /// <summary>
        /// Key for ProductManufacturers model caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public static string ProductManufacturersModelKey => "Nop.pres.product.manufacturers-{0}-{1}-{2}-{3}";
        public static string ProductManufacturersPatternKey => "Nop.pres.product.manufacturers";
        public static string ProductManufacturersPatternKeyById => "Nop.pres.product.manufacturers-{0}-";

        /// <summary>
        /// Key for ProductSpecificationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// </remarks>
        public static string ProductSpecsModelKey => "Nop.pres.product.specs-{0}-{1}";
        public static string ProductSpecsPatternKey => "Nop.pres.product.specs";
        public static string ProductSpecsPatternKeyById => "Nop.pres.product.specs-{0}-";

        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : language id
        /// {2} : store id
        /// {3} : comma separated list of customer roles
        /// </remarks>
        public static string TopicModelBySystemNameKey => "Nop.pres.topic.details.bysystemname-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic id
        /// {1} : language id
        /// {2} : store id
        /// {3} : comma separated list of customer roles
        /// </remarks>
        public static string TopicModelByIdKey => "Nop.pres.topic.details.byid-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : language id
        /// {2} : store id
        /// {3} : comma separated list of customer roles
        /// </remarks>
        public static string TopicSenameBySystemName => "Nop.pres.topic.sename.bysystemname-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : language id
        /// {2} : store id
        /// {3} : comma separated list of customer roles
        /// </remarks>
        public static string TopicTitleBySystemName => "Nop.pres.topic.title.bysystemname-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for TopMenuModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// {2} : comma separated list of customer roles
        /// </remarks>
        public static string TopicTopMenuModelKey => "Nop.pres.topic.topmenu-{0}-{1}-{2}";
        /// <summary>
        /// Key for TopMenuModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// {2} : comma separated list of customer roles
        /// </remarks>
        public static string TopicFooterModelKey => "Nop.pres.topic.footer-{0}-{1}-{2}";
        public static string TopicPatternKey => "Nop.pres.topic";

        /// <summary>
        /// Key for CategoryTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : category template id
        /// </remarks>
        public static string CategoryTemplateModelKey => "Nop.pres.categorytemplate-{0}";
        public static string CategoryTemplatePatternKey => "Nop.pres.categorytemplate";

        /// <summary>
        /// Key for ManufacturerTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer template id
        /// </remarks>
        public static string ManufacturerTemplateModelKey => "Nop.pres.manufacturertemplate-{0}";
        public static string ManufacturerTemplatePatternKey => "Nop.pres.manufacturertemplate";

        /// <summary>
        /// Key for ProductTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : product template id
        /// </remarks>
        public static string ProductTemplateModelKey => "Nop.pres.producttemplate-{0}";
        public static string ProductTemplatePatternKey => "Nop.pres.producttemplate";

        /// <summary>
        /// Key for TopicTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : topic template id
        /// </remarks>
        public static string TopicTemplateModelKey => "Nop.pres.topictemplate-{0}";
        public static string TopicTemplatePatternKey => "Nop.pres.topictemplate";

        /// <summary>
        /// Key for bestsellers identifiers displayed on the home page
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public static string HomepageBestsellersIdsKey => "Nop.pres.bestsellers.homepage-{0}";
        public static string HomepageBestsellersIdsPatternKey => "Nop.pres.bestsellers.homepage";

        /// <summary>
        /// Key for "also purchased" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : current store ID
        /// </remarks>
        public static string ProductsAlsoPurchasedIdsKey => "Nop.pres.alsopuchased-{0}-{1}";
        public static string ProductsAlsoPurchasedIdsPatternKey => "Nop.pres.alsopuchased";

        /// <summary>
        /// Key for "related" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : current store ID
        /// </remarks>
        public static string ProductsRelatedIdsKey => "Nop.pres.related-{0}-{1}";
        public static string ProductsRelatedIdsPatternKey => "Nop.pres.related";

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
        public static string ProductDefaultPicturePatternKey => "Nop.pres.product.detailspictures";
        public static string ProductDefaultPicturePatternKeyById => "Nop.pres.product.detailspictures-{0}-";

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
        public static string ProductDetailsPicturesPatternKey => "Nop.pres.product.picture";
        public static string ProductDetailsPicturesPatternKeyById => "Nop.pres.product.picture-{0}-";

        /// <summary>
        /// Key for product reviews caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : current store ID
        /// </remarks>
        public static string ProductReviewsModelKey => "Nop.pres.product.reviews-{0}-{1}";
        public static string ProductReviewsPatternKey => "Nop.pres.product.reviews";
        public static string ProductReviewsPatternKeyById => "Nop.pres.product.reviews-{0}-";

        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public static string ProductAttributePictureModelKey => "Nop.pres.productattribute.picture-{0}-{1}-{2}";
        public static string ProductAttributePicturePatternKey => "Nop.pres.productattribute.picture";

        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public static string ProductAttributeImageSquarePictureModelKey => "Nop.pres.productattribute.imagesquare.picture-{0}-{1}-{2}";
        public static string ProductAttributeImageSquarePicturePatternKey => "Nop.pres.productattribute.imagesquare.picture";

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
        public static string CategoryPicturePatternKey => "Nop.pres.category.picture";
        public static string CategoryPicturePatternKeyById => "Nop.pres.category.picture-{0}-";

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
        public static string ManufacturerPicturePatternKey => "Nop.pres.manufacturer.picture";
        public static string ManufacturerPicturePatternKeyById => "Nop.pres.manufacturer.picture-{0}-";

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
        public static string VendorPicturePatternKey => "Nop.pres.vendor.picture";
        public static string VendorPicturePatternKeyById => "Nop.pres.vendor.picture-{0}-";

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
        public static string CartPicturePatternKey => "Nop.pres.cart.picture";

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
        public static string PollsPatternKey => "Nop.pres.poll";

        /// <summary>
        /// Key for blog tag list model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static string BlogTagsModelKey => "Nop.pres.blog.tags-{0}-{1}";
        /// <summary>
        /// Key for blog archive (years, months) block model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static string BlogMonthsModelKey => "Nop.pres.blog.months-{0}-{1}";
        public static string BlogPatternKey => "Nop.pres.blog";
        /// <summary>
        /// Key for number of blog comments
        /// </summary>
        /// <remarks>
        /// {0} : blog post ID
        /// {1} : store ID
        /// {2} : are only approved comments?
        /// </remarks>
        public static string BlogCommentsNumberKey => "Nop.pres.blog.comments.number-{0}-{1}-{2}";
        public static string BlogCommentsPatternKey => "Nop.pres.blog.comments";

        /// <summary>
        /// Key for home page news
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static string HomepageNewsModelKey => "Nop.pres.news.homepage-{0}-{1}";
        public static string NewsPatternKey => "Nop.pres.news";
        /// <summary>
        /// Key for number of news comments
        /// </summary>
        /// <remarks>
        /// {0} : news item ID
        /// {1} : store ID
        /// {2} : are only approved comments?
        /// </remarks>
        public static string NewsCommentsNumberKey => "Nop.pres.news.comments.number-{0}-{1}-{2}";
        public static string NewsCommentsPatternKey => "Nop.pres.news.comments";

        /// <summary>
        /// Key for states by country id
        /// </summary>
        /// <remarks>
        /// {0} : country ID
        /// {1} : "empty" or "select" item
        /// {2} : language ID
        /// </remarks>
        public static string StateProvincesByCountryModelKey => "Nop.pres.stateprovinces.bycountry-{0}-{1}-{2}";
        public static string StateProvincesPatternKey => "Nop.pres.stateprovinces";

        /// <summary>
        /// Key for return request reasons
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string ReturnRequestReasonsModelKey => "Nop.pres.returnrequesreasons-{0}";
        public static string ReturnRequestReasonsPatternKey => "Nop.pres.returnrequesreasons";

        /// <summary>
        /// Key for return request actions
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string ReturnRequestActionsModelKey => "Nop.pres.returnrequestactions-{0}";
        public static string ReturnRequestActionsPatternKey => "Nop.pres.returnrequestactions";

        /// <summary>
        /// Key for logo
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : current theme
        /// {2} : is connection SSL secured (included in a picture URL)
        /// </remarks>
        public static string StoreLogoPath => "Nop.pres.logo-{0}-{1}-{2}";
        public static string StoreLogoPathPatternKey => "Nop.pres.logo";

        /// <summary>
        /// Key for available languages
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public static string AvailableLanguagesModelKey => "Nop.pres.languages.all-{0}";
        public static string AvailableLanguagesPatternKey => "Nop.pres.languages";

        /// <summary>
        /// Key for available currencies
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static string AvailableCurrenciesModelKey => "Nop.pres.currencies.all-{0}-{1}";
        public static string AvailableCurrenciesPatternKey => "Nop.pres.currencies";

        /// <summary>
        /// Key for caching of a value indicating whether we have checkout attributes
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : true - all attributes, false - only shippable attributes
        /// </remarks>
        public static string CheckoutAttributesExistKey => "Nop.pres.checkoutattributes.exist-{0}-{1}";
        public static string CheckoutAttributesPatternKey => "Nop.pres.checkoutattributes";

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
        public static string SitemapPatternKey => "Nop.pres.sitemap";

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
        public static string WidgetPatternKey => "Nop.pres.widget";

    }
}
