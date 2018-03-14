using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace Nop.Web.Framework.Events
{
    /// <summary>
    /// Page rendering event
    /// </summary>
    public class PageRenderingEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="helper">HTML Helper</param>
        /// <param name="overridenRouteName">Overriden route name</param>
        public PageRenderingEvent(IHtmlHelper helper, string overridenRouteName = null)
        {
            this.Helper = helper;
            this.OverridenRouteName = overridenRouteName;
        }

        /// <summary>
        /// HTML helper
        /// </summary>
        public IHtmlHelper Helper { get; private set; }

        /// <summary>
        /// Overriden route name
        /// </summary>
        public string OverridenRouteName { get; private set; }

        public IEnumerable<string> GetRouteNames()
        {
            //if an overriden route name is specified, then use it
            //we use it to specify a custom route name when some custom page uses a custom route. But we still need this event to be invoked
            if (!string.IsNullOrEmpty(OverridenRouteName))
            {
                return new List<string>() { OverridenRouteName };
            }

            var matchedRoutes = this.Helper.ViewContext.RouteData.Routers.OfType<INamedRouter>();
            return matchedRoutes.Select(r => r.Name);
        }
    }
}
