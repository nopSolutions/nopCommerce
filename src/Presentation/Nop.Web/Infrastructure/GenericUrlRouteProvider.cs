using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Seo;

namespace Nop.Web.Infrastructure
{
    /// <summary>
    /// Represents provider that provided generic routes
    /// </summary>
    public partial class GenericUrlRouteProvider : IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRoute">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRoute)
        {
            //and default one
            endpointRoute.MapDefaultControllerRoute();

            //generic URLs
            endpointRoute.MapDynamicControllerRoute<GenericRouteTransformer>("{GenericSeName}");

            //define this routes to use in UI views (in case if you want to customize some of them later)
            endpointRoute.MapControllerRoute("Product", "{SeName}",
                new { controller = "Product", action = "ProductDetails" });

            endpointRoute.MapControllerRoute("Category", "{SeName}",
                new { controller = "Catalog", action = "Category" });

            endpointRoute.MapControllerRoute("Manufacturer", "{SeName}",
                new { controller = "Catalog", action = "Manufacturer" });

            endpointRoute.MapControllerRoute("Vendor", "{SeName}",
                new { controller = "Catalog", action = "Vendor" });

            endpointRoute.MapControllerRoute("NewsItem", "{SeName}",
                new { controller = "News", action = "NewsItem" });

            endpointRoute.MapControllerRoute("BlogPost", "{SeName}",
                new { controller = "Blog", action = "BlogPost" });

            endpointRoute.MapControllerRoute("Topic", "{SeName}",
                new { controller = "Topic", action = "TopicDetails" });

            //product tags
            endpointRoute.MapControllerRoute("ProductsByTag", "{SeName}",
                new { controller = "Catalog", action = "ProductsByTag" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// It should be the last route. we do not set it to -int.MaxValue so it could be overridden (if required)
        /// </summary>
        public int Priority => -1000000;

        #endregion
    }
}
