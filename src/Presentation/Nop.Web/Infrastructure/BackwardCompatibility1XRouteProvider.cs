using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Web.Infrastructure
{
    /// <summary>
    /// Represents provider that provided routes used for backward compatibility with 1.x versions of nopCommerce
    /// </summary>
    public partial class BackwardCompatibility1XRouteProvider : IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            if (DataSettingsManager.DatabaseIsInstalled && !EngineContext.Current.Resolve<CommonSettings>().SupportPreviousNopcommerceVersions)
                return;

            //all old aspx URLs
            routeBuilder.MapRoute("", "{oldfilename}.aspx",
                new { controller = "BackwardCompatibility1X", action = "GeneralRedirect" });

            //products
            routeBuilder.MapRoute("", "products/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectProduct" });

            //categories
            routeBuilder.MapRoute("", "category/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectCategory" });

            //manufacturers
            routeBuilder.MapRoute("", "manufacturer/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectManufacturer" });

            //product tags
            routeBuilder.MapRoute("", "producttag/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectProductTag" });

            //news
            routeBuilder.MapRoute("", "news/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectNewsItem" });

            //blog posts
            routeBuilder.MapRoute("", "blog/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectBlogPost" });

            //topics
            routeBuilder.MapRoute("", "topic/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectTopic" });

            //forums
            routeBuilder.MapRoute("", "boards/fg/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectForumGroup" });

            routeBuilder.MapRoute("", "boards/f/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectForum" });

            routeBuilder.MapRoute("", "boards/t/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectForumTopic" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => -1000; //register it after all other IRouteProvider are processed

        #endregion
    }
}