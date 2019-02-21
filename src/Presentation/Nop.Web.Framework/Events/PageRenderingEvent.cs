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
        /// <param name="overriddenRouteName">Overridden route name</param>
        public PageRenderingEvent(IHtmlHelper helper, string overriddenRouteName = null)
        {
            Helper = helper;
            OverriddenRouteName = overriddenRouteName;
        }

        /// <summary>
        /// HTML helper
        /// </summary>
        public IHtmlHelper Helper { get; private set; }

        /// <summary>
        /// Overridden route name
        /// </summary>
        public string OverriddenRouteName { get; private set; }

        public IEnumerable<string> GetRouteNames()
        {
            //if an overridden route name is specified, then use it
            //we use it to specify a custom route name when some custom page uses a custom route. But we still need this event to be invoked
            if (!string.IsNullOrEmpty(OverriddenRouteName))
            {
                return new List<string>() { OverriddenRouteName };
            }

            var matchedRoutes = Helper.ViewContext.RouteData.Routers.OfType<INamedRouter>();
            return matchedRoutes.Select(r => r.Name);
        }
    }
}
