using Microsoft.AspNetCore.Routing;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Web.Infrastructure
{
    /// <summary>
    /// Represents provider that provided routes used for backward compatibility with 2.x versions of nopCommerce
    /// </summary>
    public partial class BackwardCompatibility2XRouteProvider : IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            if (!EngineContext.Current.Resolve<NopConfig>().SupportPreviousNopcommerceVersions)
                return;

            //products
            routeBuilder.MapLocalizedRoute("", "p/{productId}/{SeName?}",
                new { controller = "BackwardCompatibility2X", action = "RedirectProductById" }, new { productId = @"\d+" });

            //categories
            routeBuilder.MapLocalizedRoute("", "c/{categoryId}/{SeName?}",
                new { controller = "BackwardCompatibility2X", action = "RedirectCategoryById" }, new { categoryId = @"\d+" });

            //manufacturers
            routeBuilder.MapLocalizedRoute("", "m/{manufacturerId}/{SeName?}",
                new { controller = "BackwardCompatibility2X", action = "RedirectManufacturerById" }, new { manufacturerId = @"\d+" });

            //news
            routeBuilder.MapLocalizedRoute("", "news/{newsItemId}/{SeName?}",
                new { controller = "BackwardCompatibility2X", action = "RedirectNewsItemById" }, new { newsItemId = @"\d+" });

            //blog
            routeBuilder.MapLocalizedRoute("", "blog/{blogPostId}/{SeName?}",
                new { controller = "BackwardCompatibility2X", action = "RedirectBlogPostById" }, new { blogPostId = @"\d+" });

            //topic
            routeBuilder.MapLocalizedRoute("", "t/{SystemName}",
                new { controller = "BackwardCompatibility2X", action = "RedirectTopicBySystemName" });

            //vendors
            routeBuilder.MapLocalizedRoute("", "vendor/{vendorId}/{SeName?}",
                new { controller = "BackwardCompatibility2X", action = "RedirectVendorById" }, new { vendorId = @"\d+" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider (more the better)
        /// </summary>
        public int Priority
        {
            //register it after all other IRouteProvider are processed
            get { return -1000; }
        }

        #endregion
    }
}
