using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout
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
                "CartSlideoutUpdateItem",
                "CartSlideout/UpdateShoppingCartItem",
                new { controller = "CartSlideout", action = "UpdateShoppingCartItem" });
        }
    }
}