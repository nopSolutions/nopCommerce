using Nop.Core.Caching;

namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to catalog services
    /// </summary>
    public static partial class NopCatalogCachingDefaults
    {
        #region Categories

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey CategoriesByParentCategoryIdCacheKey => new CacheKey("Nop.category.byparent-{0}-{1}-{2}-{3}", CategoriesByParentCategoryPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// </remarks>
        public static string CategoriesByParentCategoryPrefixCacheKey => "Nop.category.byparent-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// {3} : show hidden records?
        /// </remarks>
        public static CacheKey CategoriesChildIdentifiersCacheKey => new CacheKey("Nop.category.childidentifiers-{0}-{1}-{2}-{3}", CategoriesChildIdentifiersPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// </remarks>
        public static string CategoriesChildIdentifiersPrefixCacheKey => "Nop.category.childidentifiers-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey CategoriesAllDisplayedOnHomepageCacheKey => new CacheKey("Nop.category.homepage.all", CategoriesDisplayedOnHomepagePrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : roles of the current user
        /// </remarks>
        public static CacheKey CategoriesDisplayedOnHomepageWithoutHiddenCacheKey => new CacheKey("Nop.category.homepage.withouthidden-{0}-{1}", CategoriesDisplayedOnHomepagePrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoriesDisplayedOnHomepagePrefixCacheKey => "Nop.category.homepage";

        /// <summary>
        /// Key for caching of category breadcrumb
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// </remarks>
        public static CacheKey CategoryBreadcrumbCacheKey => new CacheKey("Nop.category.breadcrumb-{0}-{1}-{2}-{3}", CategoryBreadcrumbPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoryBreadcrumbPrefixCacheKey => "Nop.category.breadcrumb";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : comma separated list of customer roles
        /// {2} : show hidden records?
        /// </remarks>
        public static CacheKey CategoriesAllCacheKey => new CacheKey("Nop.category.all-{0}-{1}-{2}", CategoriesAllPrefixCacheKey);
        
        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoriesAllPrefixCacheKey => "Nop.category.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category ID
        /// {1} : show hidden records?
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        public static CacheKey ProductCategoriesAllByCategoryIdCacheKey => new CacheKey("Nop.productcategory.allbycategoryid-{0}-{1}-{2}-{3}-{4}-{5}", ProductCategoriesByCategoryPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductCategoriesByCategoryPrefixCacheKey => "Nop.productcategory.allbycategoryid-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey ProductCategoriesAllByProductIdCacheKey => new CacheKey("Nop.productcategory.allbyproductid-{0}-{1}-{2}-{3}", ProductCategoriesByProductPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductCategoriesByProductPrefixCacheKey => "Nop.productcategory.allbyproductid-{0}";
        
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer roles ID hash
        /// {1} : current store ID
        /// {2} : categories ID hash
        /// </remarks>
        public static CacheKey CategoryNumberOfProductsCacheKey => new CacheKey("Nop.productcategory.numberofproducts-{0}-{1}-{2}", CategoryNumberOfProductsPrefixCacheKey);
        
        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoryNumberOfProductsPrefixCacheKey => "Nop.productcategory.numberofproducts";

        #endregion

        #region Manufacturers

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer ID
        /// {1} : show hidden records?
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        public static CacheKey ProductManufacturersAllByManufacturerIdCacheKey => new CacheKey("Nop.productmanufacturer.allbymanufacturerid-{0}-{1}-{2}-{3}-{4}-{5}", ProductManufacturersByManufacturerPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer ID
        /// </remarks>
        public static string ProductManufacturersByManufacturerPrefixCacheKey => "Nop.productmanufacturer.allbymanufacturerid-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey ProductManufacturersAllByProductIdCacheKey => new CacheKey("Nop.productmanufacturer.allbyproductid-{0}-{1}-{2}-{3}", ProductManufacturersByProductPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static string ProductManufacturersByProductPrefixCacheKey => "Nop.productmanufacturer.allbyproductid-{0}";

        #endregion

        #region Products

        /// <summary>
        /// Key for "related" product displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : show hidden records?
        /// </remarks>
        public static CacheKey ProductsRelatedCacheKey => new CacheKey("Nop.product.related-{0}-{1}", ProductsRelatedPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductsRelatedPrefixCacheKey => "Nop.product.related-{0}";

        /// <summary>
        /// Key for "related" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// </remarks>
        public static CacheKey ProductTierPricesCacheKey => new CacheKey("Nop.product.tierprices-{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ProductsAllDisplayedOnHomepageCacheKey => new CacheKey("Nop.product.homepage");

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product IDs hash
        /// </remarks>
        public static CacheKey ProductsByIdsCacheKey => new CacheKey("Nop.product.ids-{0}", ProductsByIdsPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductsByIdsPrefixCacheKey => "Nop.product.ids";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product atribute ID
        /// </remarks>
        public static CacheKey ProductsByProductAtributeCacheKey => new CacheKey("Nop.product.productatribute-{0}");
        
        /// <summary>
        /// Gets a key for product prices
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : overridden product price
        /// {2} : additional charge
        /// {3} : include discounts (true, false)
        /// {4} : quantity
        /// {5} : roles of the current user
        /// {6} : current store ID
        /// </remarks>
        public static CacheKey ProductPriceCacheKey => new CacheKey("Nop.totals.productprice-{0}-{1}-{2}-{3}-{4}-{5}-{6}", ProductPricePrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// </remarks>
        public static string ProductPricePrefixCacheKey => "Nop.totals.productprice-{0}";
        
        #endregion

        #region Product attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : page index
        /// {1} : page size
        /// </remarks>
        public static CacheKey ProductAttributesAllCacheKey => new CacheKey("Nop.productattribute.all-{0}-{1}", ProductAttributesAllPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributesAllPrefixCacheKey => "Nop.productattribute.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static CacheKey ProductAttributeMappingsAllCacheKey => new CacheKey("Nop.productattributemapping.all-{0}", ProductAttributeMappingsPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributeMappingsPrefixCacheKey => "Nop.productattributemapping.";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product attribute mapping ID
        /// </remarks>
        public static CacheKey ProductAttributeValuesAllCacheKey => new CacheKey("Nop.productattributevalue.all-{0}", ProductAttributeValuesAllPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributeValuesAllPrefixCacheKey => "Nop.productattributevalue.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static CacheKey ProductAttributeCombinationsAllCacheKey => new CacheKey("Nop.productattributecombination.all-{0}", ProductAttributeCombinationsAllPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributeCombinationsAllPrefixCacheKey => "Nop.productattributecombination.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Product attribute ID
        /// </remarks>
        public static CacheKey PredefinedProductAttributeValuesAllCacheKey => new CacheKey("Nop.predefinedproductattributevalues.all-{0}");

        #endregion

        #region Product tags

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ProductTagAllCacheKey => new CacheKey("Nop.producttag.all", ProductTagPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : hash of list of customer roles IDs
        /// {2} : show hidden records?
        /// </remarks>
        public static CacheKey ProductTagCountCacheKey => new CacheKey("Nop.producttag.all.count-{0}-{1}-{2}", ProductTagPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static CacheKey ProductTagAllByProductIdCacheKey => new CacheKey("Nop.producttag.allbyproductid-{0}", ProductTagPrefixCacheKey);
        
        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductTagPrefixCacheKey => "Nop.producttag.";

        #endregion

        #region Review type

        /// <summary>
        /// Key for caching all review types
        /// </summary>
        public static CacheKey ReviewTypeAllCacheKey => new CacheKey("Nop.reviewType.all");
        
        /// <summary>
        /// Key for caching product review and review type mapping
        /// </summary>
        /// <remarks>
        /// {0} : product review ID
        /// </remarks>
        public static CacheKey ProductReviewReviewTypeMappingAllCacheKey => new CacheKey("Nop.productReviewReviewTypeMapping.all-{0}", ProductReviewReviewTypeMappingAllPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductReviewReviewTypeMappingAllPrefixCacheKey => "Nop.productReviewReviewTypeMapping.all";

        #endregion

        #region Specification attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// {1} : specification attribute option ID
        /// {2} : allow filtering
        /// {3} : show on product page
        /// </remarks>
        public static CacheKey ProductSpecificationAttributeAllByProductIdCacheKey => new CacheKey("Nop.productspecificationattribute.allbyproductid-{0}-{1}-{2}-{3}", ProductSpecificationAttributeAllByProductPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static string ProductSpecificationAttributeAllByProductPrefixCacheKey => "Nop.Nop.productspecificationattribute.allbyproductid-{0}";
        
        /// <summary>
        /// Key for specification attributes caching (product details page)
        /// </summary>
        public static CacheKey SpecAttributesWithOptionsCacheKey => new CacheKey("Nop.productspecificationattribute.with.options");

        /// <summary>
        /// Key for specification attributes caching
        /// </summary>
        public static CacheKey SpecAttributesAllCacheKey => new CacheKey("Nop.productspecificationattribute.all");

        /// <summary>
        /// Key for specification attributes caching
        /// <remarks>
        /// {0} : specification attribute ID
        /// </remarks>
        /// </summary>
        public static CacheKey SpecAttributesOptionsCacheKey => new CacheKey("Nop.productspecificationattribute.options-{0}");
        
        #endregion

        #region Category template

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey CategoryTemplatesAllCacheKey => new CacheKey("Nop.categorytemplate.all");

        #endregion

        #region Manufacturer template

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ManufacturerTemplatesAllCacheKey => new CacheKey("Nop.manufacturertemplate.all");

        #endregion

        #region Product template

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ProductTemplatesAllCacheKey => new CacheKey("Nop.producttemplates.all");

        #endregion
    }
}