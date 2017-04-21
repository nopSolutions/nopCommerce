using Microsoft.AspNetCore.Routing;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;

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
            routeBuilder.MapLocalizedRoute("", "p/{productId:min(0)}/{SeName?}",
                new { controller = "BackwardCompatibility2X", action = "RedirectProductById" });

            //categories
            routeBuilder.MapLocalizedRoute("", "c/{categoryId:min(0)}/{SeName?}",
                new { controller = "BackwardCompatibility2X", action = "RedirectCategoryById" });

            //manufacturers
            routeBuilder.MapLocalizedRoute("", "m/{manufacturerId:min(0)}/{SeName?}",
                new { controller = "BackwardCompatibility2X", action = "RedirectManufacturerById" });

            //news
            routeBuilder.MapLocalizedRoute("", "news/{newsItemId:min(0)}/{SeName?}",
                new { controller = "BackwardCompatibility2X", action = "RedirectNewsItemById" });

            //blog
            routeBuilder.MapLocalizedRoute("", "blog/{blogPostId:min(0)}/{SeName?}",
                new { controller = "BackwardCompatibility2X", action = "RedirectBlogPostById" });

            //topic
            routeBuilder.MapLocalizedRoute("", "t/{SystemName}",
                new { controller = "BackwardCompatibility2X", action = "RedirectTopicBySystemName" });

            //vendors
            routeBuilder.MapLocalizedRoute("", "vendor/{vendorId:min(0)}/{SeName?}",
                new { controller = "BackwardCompatibility2X", action = "RedirectVendorById" });
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
