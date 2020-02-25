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
        /// {0} : category ID
        /// </remarks>
        public static string CategoriesByIdCacheKey => "Nop.category.id-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static string CategoriesByParentCategoryIdCacheKey => "Nop.category.byparent-{0}-{1}-{2}-{3}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string CategoriesAllDisplayedOnHomepageCacheKey => "Nop.category.homepage";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : roles of the current user
        /// </remarks>
        public static string CategoriesDisplayedOnHomepageWithoutHiddenCacheKey => "Nop.category.homepage.withouthidden-{0}-{1}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : comma separated list of customer roles
        /// {2} : show hidden records?
        /// </remarks>
        public static string CategoriesAllCacheKey => "Nop.category.all-{0}-{1}-{2}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// {3} : show hidden records?
        /// </remarks>
        public static string CategoriesChildIdentifiersCacheKey => "Nop.category.childidentifiers-{0}-{1}-{2}-{3}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : category ID
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        public static string ProductCategoriesAllByCategoryIdCacheKey => "Nop.productcategory.allbycategoryid-{0}-{1}-{2}-{3}-{4}-{5}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : product ID
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static string ProductCategoriesAllByProductIdCacheKey => "Nop.productcategory.allbyproductid-{0}-{1}-{2}-{3}";

        /// <summary>
        /// Key for caching of category breadcrumb
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// </remarks>
        public static string CategoryBreadcrumbKey => "Nop.category.breadcrumb-{0}-{1}-{2}-{3}";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer roles ID hash
        /// {1} : current store ID
        /// {2} : categories ID hash
        /// </remarks>
        public static string CategoryNumberOfProductsModelKey => "Nop.productcategory.numberofproducts-{0}-{1}-{2}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoriesPrefixCacheKey => "Nop.category.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductCategoriesPrefixCacheKey => "Nop.productcategory.";

        #endregion

        #region Manufacturers

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer ID
        /// </remarks>
        public static string ManufacturersByIdCacheKey => "Nop.manufacturer.id-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : manufacturer ID
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        public static string ProductManufacturersAllByManufacturerIdCacheKey => "Nop.productmanufacturer.allbymanufacturerid-{0}-{1}-{2}-{3}-{4}-{5}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : product ID
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static string ProductManufacturersAllByProductIdCacheKey => "Nop.productmanufacturer.allbyproductid-{0}-{1}-{2}-{3}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ManufacturersPrefixCacheKey => "Nop.manufacturer.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductManufacturersPrefixCacheKey => "Nop.productmanufacturer.";

        #endregion

        #region Products

        /// <summary>
        /// Key for "related" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : show hidden records?
        /// </remarks>
        public static string ProductsRelatedIdsKey => "Nop.product.related-{0}-{1}";


        /// <summary>
        /// Key for "related" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// </remarks>
        public static string ProductTierPrices => "Nop.product.tierprices-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductTierPricesPrefixCacheKey => "Nop.product.tierprices";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductsRelatedIdsPrefixCacheKey => "Nop.product.related";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static string ProductsByIdCacheKey => "Nop.product.id-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductsPrefixCacheKey => "Nop.product.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product IDs hash
        /// </remarks>
        public static string ProductsByIdsCacheKey => "Nop.product.ids-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product atribute ID
        /// </remarks>
        public static string ProductsByProductAtributeCacheKey => "Nop.product.productatribute-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string ProductsAllDisplayedOnHomepageCacheKey => "Nop.product.homepage";

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
        public static string ProductPriceModelCacheKey => "Nop.totals.productprice-{0}-{1}-{2}-{3}-{4}-{5}-{6}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductPricePrefixCacheKey => "Nop.totals.productprice";

        /// <summary>
        /// Gets a key for category IDs of a product
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static string ProductCategoryIdsModelCacheKey => "Nop.totals.product.categoryids-{0}-{1}-{2}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductCategoryIdsPrefixCacheKey => "Nop.totals.product.categoryids";

        /// <summary>
        /// Gets a key for manufacturer IDs of a product
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static string ProductManufacturerIdsModelCacheKey => "Nop.totals.product.manufacturerids-{0}-{1}-{2}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductManufacturerIdsPrefixCacheKey => "Nop.totals.product.manufacturerids";
        
        #endregion

        #region Product attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : page index
        /// {1} : page size
        /// </remarks>
        public static string ProductAttributesAllCacheKey => "Nop.productattribute.all-{0}-{1}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product attribute ID
        /// </remarks>
        public static string ProductAttributesByIdCacheKey => "Nop.productattribute.id-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static string ProductAttributeMappingsAllCacheKey => "Nop.productattributemapping.all-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product attribute mapping ID
        /// </remarks>
        public static string ProductAttributeMappingsByIdCacheKey => "Nop.productattributemapping.id-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product attribute mapping ID
        /// </remarks>
        public static string ProductAttributeValuesAllCacheKey => "Nop.productattributevalue.all-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product attribute value ID
        /// </remarks>
        public static string ProductAttributeValuesByIdCacheKey => "Nop.productattributevalue.id-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static string ProductAttributeCombinationsAllCacheKey => "Nop.productattributecombination.all-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Product attribute ID
        /// </remarks>
        public static string PredefinedProductAttributeValuesAllCacheKey => "Nop.predefinedproductattributevalues.all-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributesPrefixCacheKey => "Nop.productattribute.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributeMappingsPrefixCacheKey => "Nop.productattributemapping.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributeValuesPrefixCacheKey => "Nop.productattributevalue.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributeCombinationsPrefixCacheKey => "Nop.productattributecombination.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string PredefinedProductAttributeValuesPrefixCacheKey => "Nop.predefinedproductattributevalues.";

        #endregion

        #region Product tags

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : comma separated list of customer roles
        /// {2} : show hidden records?
        /// </remarks>
        public static string ProductTagCountCacheKey => "Nop.producttag.count-{0}-{1}-{2}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static string ProductTagAllByProductIdCacheKey => "Nop.producttag.allbyproductid-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string ProductTagAllCacheKey => "Nop.producttag.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductTagPrefixCacheKey => "Nop.producttag.";

        #endregion

        #region Review type

        /// <summary>
        /// Key for caching all review types
        /// </summary>
        public static string ReviewTypeAllKey => "Nop.reviewType.all";

        /// <summary>
        /// Key for caching review type
        /// </summary>
        /// <remarks>
        /// {0} : review type ID
        /// </remarks>
        public static string ReviewTypeByIdKey => "Nop.reviewType.id-{0}";

        /// <summary>
        /// Key pattern to clear cache review types
        /// </summary>
        public static string ReviewTypeByPrefixCacheKey => "Nop.reviewType.";

        /// <summary>
        /// Key for caching product review and review type mapping
        /// </summary>
        /// <remarks>
        /// {0} : product review ID
        /// </remarks>
        public static string ProductReviewReviewTypeMappingAllKey => "Nop.productReviewReviewTypeMapping.all-{0}";

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
        public static string ProductSpecificationAttributeAllByProductIdCacheKey => "Nop.productspecificationattribute.allbyproductid-{0}-{1}-{2}-{3}";

        /// <summary>
        /// Key for specification attributes caching (product details page)
        /// </summary>
        public static string SpecAttributesWithOptionsKey => "Nop.productspecificationattribute.with.options";

        /// <summary>
        /// Key for specification attributes caching
        /// </summary>
        public static string SpecAttributesAllCacheKey => "Nop.productspecificationattribute.all";

        /// <summary>
        /// Key for specification attributes caching
        /// <remarks>
        /// {0} : specification attribute ID
        /// </remarks>
        /// </summary>
        public static string SpecAttributesOptionsCacheKey => "Nop.productspecificationattribute.options-{0}";
        
        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductSpecificationAttributePrefixCacheKey => "Nop.productspecificationattribute.";

        #endregion

        #region Category template

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string CategoryTemplatesAllCacheKey => "Nop.categorytemplate.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoryTemplatesPrefixCacheKey => "Nop.categorytemplate.";

        #endregion

        #region Manufacturer template

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string ManufacturerTemplatesAllCacheKey => "Nop.manufacturertemplate.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ManufacturerTemplatesPrefixCacheKey => "Nop.manufacturertemplate.";

        #endregion

        #region Product template

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string ProductTemplatesAllCacheKey => "Nop.producttemplates.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductTemplatesPrefixCacheKey => "Nop.producttemplates.";

        #endregion
    }
}