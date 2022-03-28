using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.AbcHomeDeliveryStatus.Infrastructure
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

            endpointRouteBuilder.MapControllerRoute("AbcContactUs",
                            "AbcContactUs/DisplayContactUs",
                            new
                            {
                                controller = "AbcContactUs",
                                action = "DisplayContactUs"
                            });
        }
    }
}
