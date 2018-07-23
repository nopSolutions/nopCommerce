namespace Nop.Services.Catalog
{
    /// <summary>
    /// Represents default values related to catalog services
    /// </summary>
    public static partial class NopCatalogDefaults
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
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoriesPatternCacheKey => "Nop.category.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductCategoriesPatternCacheKey => "Nop.productcategory.";

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
        public static string ManufacturersPatternCacheKey => "Nop.manufacturer.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductManufacturersPatternCacheKey => "Nop.productmanufacturer.";

        #endregion

        #region Products

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
        public static string ProductsPatternCacheKey => "Nop.product.";

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
        public static string ProductPricePatternCacheKey => "Nop.totals.productprice";

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
        public static string ProductCategoryIdsPatternCacheKey => "Nop.totals.product.categoryids";

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
        public static string ProductManufacturerIdsPatternCacheKey => "Nop.totals.product.manufacturerids";

        /// <summary>
        /// Gets a template of product name on copying
        /// </summary>
        /// <remarks>
        /// {0} : product name
        /// </remarks>
        public static string ProductCopyNameTemplate => "Copy of {0}";

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
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributesPatternCacheKey => "Nop.productattribute.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributeMappingsPatternCacheKey => "Nop.productattributemapping.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributeValuesPatternCacheKey => "Nop.productattributevalue.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributeCombinationsPatternCacheKey => "Nop.productattributecombination.";

        #endregion

        #region Product tags

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        public static string ProductTagCountCacheKey => "Nop.producttag.count-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static string ProductTagAllByProductIdCacheKey => "Nop.producttag.allbyproductid-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductTagPatternCacheKey => "Nop.producttag.";

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
        public static string ReviewTypeByPatternKey => "Nop.reviewType.";

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
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductSpecificationAttributePatternCacheKey => "Nop.productspecificationattribute.";

        #endregion
    }
}