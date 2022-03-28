using System.Xml.Linq;
using System.Data.Common;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Data;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Localization;
using Nop.Services.Localization;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.FreshAddressNewsletterIntegration.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority
        {
            get
            {
                return int.MaxValue;
            }
        }

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                "CustomSubscribeNewsletter",
                 $"subscribenewsletter",
                 new { controller = "CustomNewsletter", action = "SubscribeNewsletter" }
            );
        }
    }
}
