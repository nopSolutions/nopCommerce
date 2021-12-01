using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

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
            var httpContext = Helper.ViewContext.HttpContext;
            var routeName = httpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;

            //then try to get a generic one (actually it's an action name, not the route)
            if (string.IsNullOrEmpty(routeName) && httpContext.GetRouteValue(NopPathRouteDefaults.SeNameFieldKey) is not null)
            {
                routeName = httpContext.GetRouteValue(NopPathRouteDefaults.ActionFieldKey)?.ToString();

                //there are some cases when the action name doesn't match the route name
                //it's not easy to make them the same, so we'll just handle them here
                if (routeName == "ProductDetails")
                    routeName = "Product";
                if (routeName == "TopicDetails")
                    routeName = "Topic";
            }

            return routeName;
        }

        #endregion
    }
}