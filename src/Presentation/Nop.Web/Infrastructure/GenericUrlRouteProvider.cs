using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Mvc.Routing;

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
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var pattern = "{SeName}";
            if (DataSettingsManager.DatabaseIsInstalled)
            {
                var localizationSettings = endpointRouteBuilder.ServiceProvider.GetRequiredService<LocalizationSettings>();
                if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    var langservice = endpointRouteBuilder.ServiceProvider.GetRequiredService<ILanguageService>();
                    var languages = langservice.GetAllLanguages().ToList();
                    pattern = "{language:lang=" + languages.FirstOrDefault().UniqueSeoCode + "}/{SeName}";
                }
            }
            endpointRouteBuilder.MapDynamicControllerRoute<SlugRouteTransformer>(pattern);

            //and default one
            endpointRouteBuilder.MapControllerRoute(
                name: "Default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //generic URLs
            endpointRouteBuilder.MapControllerRoute(
                name: "GenericUrl",
                pattern: "{GenericSeName}",
                new { controller = "Common", action = "GenericUrl" });

            //define this routes to use in UI views (in case if you want to customize some of them later)
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

            //product tags
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
