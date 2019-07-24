using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Nop.Web.Framework.Localization
{
    /// <summary>
    /// Represents extensions of LocalizedRoute
    /// </summary>
    public static class LocalizedRouteExtensions
    {
        /// <summary>
        /// Adds a route to the route builder with the specified name and template
        /// </summary>
        /// <param name="routeBuilder">The route builder to add the route to</param>
        /// <param name="name">The name of the route</param>
        /// <param name="template">The URL pattern of the route</param>
        /// <returns>Route builder</returns>
        public static IRouteBuilder MapLocalizedRoute(this IRouteBuilder routeBuilder, string name, string template)
        {
            return MapLocalizedRoute(routeBuilder, name, template, defaults: null);
        }

        /// <summary>
        /// Adds a route to the route builder with the specified name, template, and default values
        /// </summary>
        /// <param name="routeBuilder">The route builder to add the route to</param>
        /// <param name="name">The name of the route</param>
        /// <param name="template">The URL pattern of the route</param>
        /// <param name="defaults">An object that contains default values for route parameters. 
        /// The object's properties represent the names and values of the default values</param>
        /// <returns>Route builder</returns>
        public static IRouteBuilder MapLocalizedRoute(this IRouteBuilder routeBuilder, string name, string template, object defaults)
        {
            return MapLocalizedRoute(routeBuilder, name, template, defaults, constraints: null);
        }

        /// <summary>
        /// Adds a route to the route builder with the specified name, template, default values, and constraints.
        /// </summary>
        /// <param name="routeBuilder">The route builder to add the route to</param>
        /// <param name="name">The name of the route</param>
        /// <param name="template">The URL pattern of the route</param>
        /// <param name="defaults"> An object that contains default values for route parameters. 
        /// The object's properties represent the names and values of the default values</param>
        /// <param name="constraints">An object that contains constraints for the route. 
        /// The object's properties represent the names and values of the constraints</param>
        /// <returns>Route builder</returns>
        public static IRouteBuilder MapLocalizedRoute(this IRouteBuilder routeBuilder,
            string name, string template, object defaults, object constraints)
        {
            return MapLocalizedRoute(routeBuilder, name, template, defaults, constraints, dataTokens: null);
        }

        /// <summary>
        /// Adds a route to the route builder with the specified name, template, default values, constraints anddata tokens.
        /// </summary>
        /// <param name="routeBuilder">The route builder to add the route to</param>
        /// <param name="name">The name of the route</param>
        /// <param name="template">The URL pattern of the route</param>
        /// <param name="defaults"> An object that contains default values for route parameters. 
        /// The object's properties represent the names and values of the default values</param>
        /// <param name="constraints">An object that contains constraints for the route. 
        /// The object's properties represent the names and values of the constraints</param>
        /// <param name="dataTokens">An object that contains data tokens for the route. 
        /// The object's properties represent the names and values of the data tokens</param>
        /// <returns>Route builder</returns>
        public static IRouteBuilder MapLocalizedRoute(this IRouteBuilder routeBuilder,
            string name, string template, object defaults, object constraints, object dataTokens)
        {
            if (routeBuilder.DefaultHandler == null)
                throw new ArgumentNullException(nameof(routeBuilder));

            //get registered InlineConstraintResolver
            var inlineConstraintResolver = routeBuilder.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();

            //create new generic route
            routeBuilder.Routes.Add(new LocalizedRoute(routeBuilder.DefaultHandler, name, template,
                new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new RouteValueDictionary(dataTokens),
                inlineConstraintResolver));

            return routeBuilder;
        }

        /// <summary>
        /// Clear _seoFriendlyUrlsForLanguagesEnabled cached value for the routes
        /// </summary>
        /// <param name="routers">Routers</param>
        public static void ClearSeoFriendlyUrlsCachedValueForRoutes(this IEnumerable<IRouter> routers)
        {
            if (routers == null)
                throw new ArgumentNullException(nameof(routers));

            //clear cached settings
            foreach (var router in routers)
            {
                var routeCollection = router as RouteCollection;
                if (routeCollection == null)
                    continue;

                for (var i = 0; i < routeCollection.Count; i++)
                {
                    var route = routeCollection[i];
                    (route as LocalizedRoute)?.ClearSeoFriendlyUrlsCachedValue();
                }
            }
        }
    }
}