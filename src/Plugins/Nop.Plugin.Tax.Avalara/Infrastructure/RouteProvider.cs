using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Tax.Avalara.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : BaseRouteProvider, IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var lang = GetLanguageRoutePattern();

            endpointRouteBuilder.MapControllerRoute(name: AvalaraTaxDefaults.ConfigurationRouteName,
                pattern: "Admin/Avalara/Configure",
                defaults: new { controller = "Avalara", action = "Configure" });

            //override some of default routes in Admin area
            endpointRouteBuilder.MapControllerRoute(name: AvalaraTaxDefaults.TaxCategoriesRouteName,
                pattern: "Admin/Tax/Categories",
                defaults: new { controller = "AvalaraTax", action = "Categories", area = AreaNames.Admin });

            endpointRouteBuilder.MapControllerRoute(name: AvalaraTaxDefaults.ExemptionCertificatesRouteName,
                pattern: $"{lang}/customer/exemption-certificates",
                defaults: new { controller = "AvalaraPublic", action = "ExemptionCertificates" });

            endpointRouteBuilder.MapControllerRoute(name: AvalaraTaxDefaults.DownloadCertificateRouteName,
                pattern: "download-tax-exemption-certificate/{id:min(0)}",
                defaults: new { controller = "AvalaraPublic", action = "DownloadCertificate" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 1; //set a value that is greater than the default one in Nop.Web to override routes
    }
}