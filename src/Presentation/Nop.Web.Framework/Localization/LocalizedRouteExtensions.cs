using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Web.Framework.Localization
{
    public static class LocalizedRouteExtensions
    {
        //Override for localized route
        public static Route MapLocalizedRoute(this RouteCollection routes, string name, string url)
        {
            return MapLocalizedRoute(routes, name, url, null /* defaults */, (object)null /* constraints */);
        }
        public static Route MapLocalizedRoute(this RouteCollection routes, string name, string url, object defaults)
        {
            return MapLocalizedRoute(routes, name, url, defaults, (object)null /* constraints */);
        }
        public static Route MapLocalizedRoute(this RouteCollection routes, string name, string url, object defaults, object constraints)
        {
            return MapLocalizedRoute(routes, name, url, defaults, constraints, null /* namespaces */);
        }
        public static Route MapLocalizedRoute(this RouteCollection routes, string name, string url, string[] namespaces)
        {
            return MapLocalizedRoute(routes, name, url, null /* defaults */, null /* constraints */, namespaces);
        }
        public static Route MapLocalizedRoute(this RouteCollection routes, string name, string url, object defaults, string[] namespaces)
        {
            return MapLocalizedRoute(routes, name, url, defaults, null /* constraints */, namespaces);
        }
        public static Route MapLocalizedRoute(this RouteCollection routes, string name, string url, object defaults, object constraints, string[] namespaces)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            var route = new LocalizedRoute(url, new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints),
                DataTokens = new RouteValueDictionary()
            };

            if ((namespaces != null) && (namespaces.Length > 0))
            {
                route.DataTokens["Namespaces"] = namespaces;
            }

            routes.Add(name, route);

            return route;
        }
        
        public static void ClearSeoFriendlyUrlsCachedValueForRoutes(this RouteCollection routes)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }
            foreach (var route in routes)
            {
                if (route is LocalizedRoute)
                {
                    ((LocalizedRoute) route).ClearSeoFriendlyUrlsCachedValue();
                }
            }
        }
    }
}