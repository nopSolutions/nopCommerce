using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Web.Framework.Seo
{
    public static class GenericPathRouteExtensions
    {
        //Override for localized route
        public static Route MapGenericPathRoute(this RouteCollection routes, string name, string url)
        {
            return MapGenericPathRoute(routes, name, url, null /* defaults */, (object)null /* constraints */);
        }
        public static Route MapGenericPathRoute(this RouteCollection routes, string name, string url, object defaults)
        {
            return MapGenericPathRoute(routes, name, url, defaults, (object)null /* constraints */);
        }
        public static Route MapGenericPathRoute(this RouteCollection routes, string name, string url, object defaults, object constraints)
        {
            return MapGenericPathRoute(routes, name, url, defaults, constraints, null /* namespaces */);
        }
        public static Route MapGenericPathRoute(this RouteCollection routes, string name, string url, string[] namespaces)
        {
            return MapGenericPathRoute(routes, name, url, null /* defaults */, null /* constraints */, namespaces);
        }
        public static Route MapGenericPathRoute(this RouteCollection routes, string name, string url, object defaults, string[] namespaces)
        {
            return MapGenericPathRoute(routes, name, url, defaults, null /* constraints */, namespaces);
        }
        public static Route MapGenericPathRoute(this RouteCollection routes, string name, string url, object defaults, object constraints, string[] namespaces)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            var route = new GenericPathRoute(url, new MvcRouteHandler())
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
    }
}