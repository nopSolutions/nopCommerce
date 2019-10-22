using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Seo;

namespace Nop.Web.Infrastructure
{
    /// <summary>
    /// Represents provider that provided generic routes
    /// </summary>
    public partial class GenericUrlRouteProvider : IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //and default one
            routeBuilder.MapRoute("Default", "{controller}/{action}/{id?}");

            //generic URLs
            routeBuilder.MapGenericPathRoute("GenericUrl", "{SeName}",
                new { controller = "Common", action = "GenericUrl" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority
        {
            //it should be the last route. we do not set it to -int.MaxValue so it could be overridden (if required)
            get { return -1000000; }
        }

        #endregion
    }
}
