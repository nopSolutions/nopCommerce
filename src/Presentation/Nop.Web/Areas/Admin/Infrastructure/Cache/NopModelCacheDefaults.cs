namespace Nop.Web.Areas.Admin.Infrastructure.Cache
{
    public static partial class NopModelCacheDefaults
    {
        /// <summary>
        /// Key for nopCommerce.com news cache
        /// </summary>
        public static string OfficialNewsModelKey => "Nop.pres.admin.official.news";
        public static string OfficialNewsPrefixCacheKey => "Nop.pres.admin.official.news";

        /// <summary>
        /// Key for specification attributes caching (product details page)
        /// </summary>
        public static string SpecAttributesModelKey => "Nop.pres.admin.product.specs";
        public static string SpecAttributesPrefixCacheKey => "Nop.pres.admin.product.specs";

        /// <summary>
        /// Key for categories caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static string CategoriesListKey => "Nop.pres.admin.categories.list-{0}";
        public static string CategoriesListPrefixCacheKey => "Nop.pres.admin.categories.list";

        /// <summary>
        /// Key for manufacturers caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static string ManufacturersListKey => "Nop.pres.admin.manufacturers.list-{0}";
        public static string ManufacturersListPrefixCacheKey => "Nop.pres.admin.manufacturers.list";

        /// <summary>
        /// Key for vendors caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static string VendorsListKey => "Nop.pres.admin.vendors.list-{0}";
        public static string VendorsListPrefixCacheKey => "Nop.pres.admin.vendors.list";
    }
}
