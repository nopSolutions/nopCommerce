using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Seo;

namespace Nop.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// Represents the helper implementation to build specific URLs within an application 
    /// </summary>
    public partial class NopUrlHelper : INopUrlHelper
    {
        public static string ProductRoute = "Product";
        public static string CategoryRoute = "Category";
        public static string ManufacturerRoute = "Manufacturer";
        public static string VendorRoute = "Vendor";
        public static string NewsItemRoute = "NewsItem";
        public static string BlogPostRoute = "BlogPost";
        public static string TopicRoute = "Topic";
        public static string ProductsByTagRoute = "ProductsByTag";

        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ICategoryService _categoryService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public NopUrlHelper(CatalogSettings catalogSettings,
            IActionContextAccessor actionContextAccessor,
            ICategoryService categoryService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService)
        {
            _catalogSettings = catalogSettings;
            _actionContextAccessor = actionContextAccessor;
            _categoryService = categoryService;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Generate a URL for a product with the specified route values
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        /// <param name="values">An object that contains route values</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
        /// <param name="host">The host name for the URL</param>
        /// <param name="fragment">The fragment for the URL</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the generated URL
        /// </returns>
        protected virtual async Task<string> RouteProductUrlAsync(IUrlHelper urlHelper,
            object values = null, string protocol = null, string host = null, string fragment = null)
        {
            return urlHelper.RouteUrl(ProductRoute, values, protocol, host, fragment);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generate a generic URL for the specified entity type and route values
        /// </summary>
        /// <typeparam name="TEntity">Entity type that supports slug</typeparam>
        /// <param name="values">An object that contains route values</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
        /// <param name="host">The host name for the URL</param>
        /// <param name="fragment">The fragment for the URL</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the generated URL
        /// </returns>
        public virtual async Task<string> RouteGenericUrlAsync<TEntity>(object values = null, string protocol = null, string host = null, string fragment = null)
            where TEntity : BaseEntity, ISlugSupported
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            return typeof(TEntity) switch
            {
                var entityType when entityType == typeof(Product) => await RouteProductUrlAsync(urlHelper, values, protocol, host, fragment),
                var entityType when entityType == typeof(Category) => urlHelper.RouteUrl(CategoryRoute, values, protocol, host, fragment),
                var entityType when entityType == typeof(Manufacturer) => urlHelper.RouteUrl(ManufacturerRoute, values, protocol, host, fragment),
                var entityType when entityType == typeof(Vendor) => urlHelper.RouteUrl(VendorRoute, values, protocol, host, fragment),
                var entityType when entityType == typeof(NewsItem) => urlHelper.RouteUrl(NewsItemRoute, values, protocol, host, fragment),
                var entityType when entityType == typeof(BlogPost) => urlHelper.RouteUrl(BlogPostRoute, values, protocol, host, fragment),
                var entityType when entityType == typeof(Topic) => urlHelper.RouteUrl(TopicRoute, values, protocol, host, fragment),
                var entityType when entityType == typeof(ProductTag) => urlHelper.RouteUrl(ProductsByTagRoute, values, protocol, host, fragment),
                var entityType => urlHelper.RouteUrl(entityType.Name, values, protocol, host, fragment)
            };
        }

        #endregion
    }
}