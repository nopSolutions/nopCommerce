using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.LiveChat.LivePerson
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.LiveChat.LivePerson.Configure",
                 "Plugins/LiveChatLivePerson/Configure",
                 new { controller = "LiveChatLivePerson", action = "Configure" },
                 new[] { "Nop.Plugin.LiveChat.LivePerson.Controllers" }
            );

            routes.MapRoute("Plugin.LiveChat.LivePerson.PublicInfo",
                 "Plugins/LiveChatLivePerson/PublicInfo",
                 new { controller = "LiveChatLivePerson", action = "PublicInfo" },
                 new[] { "Nop.Plugin.LiveChat.LivePerson.Controllers" }
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
