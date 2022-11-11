using Nop.Core.Caching;

namespace Nop.Web.Areas.Admin.Infrastructure.Cache
{
    public static partial class NopModelCacheDefaults
    {
        /// <summary>
        /// Key for nopCommerce.com news cache
        /// </summary>
        public static CacheKey OfficialNewsModelKey => new("Nop.pres.admin.official.news");
        
        /// <summary>
        /// Key for categories caching
        /// </summary>
        public static CacheKey CategoriesListKey => new("Nop.pres.admin.categories.list");

        /// <summary>
        /// Key for manufacturers caching
        /// </summary>
        public static CacheKey ManufacturersListKey => new("Nop.pres.admin.manufacturers.list");

        /// <summary>
        /// Key for vendors caching
        /// </summary>
        public static CacheKey VendorsListKey => new("Nop.pres.admin.vendors.list");
    }
}
