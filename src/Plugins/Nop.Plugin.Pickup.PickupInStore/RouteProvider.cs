using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Pickup.PickupInStore
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Pickup.PickupInStore.Create",
                 "Plugins/PickupInStore/Create",
                 new { controller = "PickupInStore", action = "Create", },
                 new[] { "Nop.Plugin.Pickup.PickupInStore.Controllers" }
            );

            routes.MapRoute("Plugin.Pickup.PickupInStore.Edit",
                 "Plugins/PickupInStore/Edit",
                 new { controller = "PickupInStore", action = "Edit" },
                 new[] { "Nop.Plugin.Pickup.PickupInStore.Controllers" }
            );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
