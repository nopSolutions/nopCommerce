using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace Nop.Web.Framework.Events
{
    /// <summary>
    /// Represents a page rendering event
    /// </summary>
    public class PageRenderingEvent
    {
        #region Ctor

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

        #endregion

        #region Properties

        /// <summary>
        /// Gets HTML helper
        /// </summary>
        public IHtmlHelper Helper { get; private set; }

        /// <summary>
        /// Gets overridden route name
        /// </summary>
        public string OverriddenRouteName { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get the route name associated with the request rendering this page
        /// </summary>
        /// <returns>Route name</returns>
        public string GetRouteName()
        {
            //if an overridden route name is specified, then use it
            //we use it to specify a custom route name when some custom page uses a custom route. But we still need this event to be invoked
            if (!string.IsNullOrEmpty(OverriddenRouteName))
                return OverriddenRouteName;

            //or try to get a registered endpoint route name
            var endpointFeature = Helper.ViewContext.HttpContext.Features.Get<IEndpointFeature>();
            var routeNameMetadata = endpointFeature.Endpoint.Metadata.GetMetadata<RouteNameMetadata>();
            return routeNameMetadata.RouteName;
        }

        #endregion
    }
}
