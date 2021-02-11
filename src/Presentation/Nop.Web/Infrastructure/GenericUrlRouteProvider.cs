using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Data;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Web.Infrastructure
{
    /// <summary>
    /// Represents provider that provided generic routes
    /// </summary>
    public partial class GenericUrlRouteProvider : BaseRouteProvider, IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var pattern = GetLanguageRoutePattern();
            pattern += "/";

            //default routes
            //these routes are not generic, they are just default to map requests that don't match other patterns, 
            //but we define them here since this route provider is with the lowest priority, to allow to add additional routes before them
            endpointRouteBuilder.MapControllerRoute(name: "Default",
                pattern: $"{pattern}{{controller=Home}}/{{action=Index}}/{{id?}}");

            endpointRouteBuilder.MapControllerRoute(name: "Default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //generic routes
            pattern += "{SeName}/";

            if (DataSettingsManager.IsDatabaseInstalled())
                endpointRouteBuilder.MapDynamicControllerRoute<SlugRouteTransformer>(pattern);

            endpointRouteBuilder.MapControllerRoute(name: "GenericUrl",
                pattern: "{genericSeName}",
                new { controller = "Common", action = "GenericUrl" });

            endpointRouteBuilder.MapControllerRoute(name: "GenericUrl",
                pattern: "{genericSeName}/{genericParameter}",
                new { controller = "Common", action = "GenericUrl" });

            endpointRouteBuilder.MapControllerRoute("Product", pattern,
                new { controller = "Product", action = "ProductDetails" });

            endpointRouteBuilder.MapControllerRoute("Category", pattern,
                new { controller = "Catalog", action = "Category" });

            endpointRouteBuilder.MapControllerRoute("Manufacturer", pattern,
                new { controller = "Catalog", action = "Manufacturer" });

            endpointRouteBuilder.MapControllerRoute("Vendor", pattern,
                new { controller = "Catalog", action = "Vendor" });

            endpointRouteBuilder.MapControllerRoute("NewsItem", pattern,
                new { controller = "News", action = "NewsItem" });

            endpointRouteBuilder.MapControllerRoute("BlogPost", pattern,
                new { controller = "Blog", action = "BlogPost" });

            endpointRouteBuilder.MapControllerRoute("Topic", pattern,
                new { controller = "Topic", action = "TopicDetails" });

            endpointRouteBuilder.MapControllerRoute("ProductsByTag", pattern,
                new { controller = "Catalog", action = "ProductsByTag" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        /// <remarks>
        /// it should be the last route. we do not set it to -int.MaxValue so it could be overridden (if required)
        /// </remarks>
        public int Priority => -1000000;

        #endregion
    }
}