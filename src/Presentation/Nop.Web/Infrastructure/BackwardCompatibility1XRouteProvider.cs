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
        /// <param name="endpointRoute">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRoute)
        {
            if (DataSettingsManager.DatabaseIsInstalled && !EngineContext.Current.Resolve<CommonSettings>().SupportPreviousNopcommerceVersions)
                return;

            //all old aspx URLs
            endpointRoute.MapControllerRoute("", "{oldfilename}.aspx",
                new { controller = "BackwardCompatibility1X", action = "GeneralRedirect" });

            //products
            endpointRoute.MapControllerRoute("", "products/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectProduct" });

            //categories
            endpointRoute.MapControllerRoute("", "category/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectCategory" });

            //manufacturers
            endpointRoute.MapControllerRoute("", "manufacturer/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectManufacturer" });

            //product tags
            endpointRoute.MapControllerRoute("", "producttag/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectProductTag" });

            //news
            endpointRoute.MapControllerRoute("", "news/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectNewsItem" });

            //blog posts
            endpointRoute.MapControllerRoute("", "blog/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectBlogPost" });

            //topics
            endpointRoute.MapControllerRoute("", "topic/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectTopic" });

            //forums
            endpointRoute.MapControllerRoute("", "boards/fg/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectForumGroup" });

            endpointRoute.MapControllerRoute("", "boards/f/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectForum" });

            endpointRoute.MapControllerRoute("", "boards/t/{id}.aspx",
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