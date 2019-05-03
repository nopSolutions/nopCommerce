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
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //and default one
            routeBuilder.MapRoute("Default", "{controller}/{action}/{id?}");

            //generic URLs
            routeBuilder.MapGenericPathRoute("GenericUrl", "{GenericSeName}",
                new { controller = "Common", action = "GenericUrl" });

            //define this routes to use in UI views (in case if you want to customize some of them later)
            routeBuilder.MapLocalizedRoute("Product", "{SeName}", 
                new { controller = "Product", action = "ProductDetails" });

            routeBuilder.MapLocalizedRoute("Category", "{SeName}", 
                new { controller = "Catalog", action = "Category" });

            routeBuilder.MapLocalizedRoute("Manufacturer", "{SeName}", 
                new { controller = "Catalog", action = "Manufacturer" });

            routeBuilder.MapLocalizedRoute("Vendor", "{SeName}", 
                new { controller = "Catalog", action = "Vendor" });
            
            routeBuilder.MapLocalizedRoute("NewsItem", "{SeName}", 
                new { controller = "News", action = "NewsItem" });

            routeBuilder.MapLocalizedRoute("BlogPost", "{SeName}", 
                new { controller = "Blog", action = "BlogPost" });

            routeBuilder.MapLocalizedRoute("Topic", "{SeName}", 
                new { controller = "Topic", action = "TopicDetails" });

            //product tags
            routeBuilder.MapLocalizedRoute("ProductsByTag", "{SeName}",
                new { controller = "Catalog", action = "ProductsByTag" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority
        {
            //it should be the last route. we do not set it to -int.MaxValue so it could be overridden (if required)
            get { return -1000000; }
        }

        #endregion
    }
}
