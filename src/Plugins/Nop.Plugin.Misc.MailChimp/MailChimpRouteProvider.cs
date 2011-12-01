using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Misc.MailChimp {
    public class MailChimpRouteProvider : IRouteProvider {
        #region Implementation of IRouteProvider

        public void RegisterRoutes(RouteCollection routes) {
            routes.MapRoute("Plugin.Misc.MailChimp.Configure", "Plugins/MiscMailChimp/Configure",
                            new {controller = "Settings", action = "Index"},
                            new[] {"Nop.Plugin.Misc.MailChimp.Controllers"}
                );
        }

        #endregion

        #region Implementation of IRouteProvider

        public int Priority {
            get { return 0; }
        }

        #endregion
    }
}