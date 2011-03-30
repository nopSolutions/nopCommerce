using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Mvc.Routes
{
    public class RoutePublisher : IRoutePublisher
    {
        private readonly ITypeFinder _typeFinder;

        public RoutePublisher(
            ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }

        public void Publish(RouteCollection routeCollection, IEnumerable<RouteDescriptor> routes)
        {
            var routesArray = routes.OrderByDescending(r => r.Priority).ToArray();

            // this is not called often, but is intended to surface problems before
            // the actual collection is modified
            //var preloading = new RouteCollection();
            //foreach (var route in routesArray)
            //    preloading.Add(route.Name, route.Route);

            using (routeCollection.GetWriteLock())
            {
                foreach (var routeDescriptor in routesArray)
                {
                    routeCollection.Add(routeDescriptor.Name, routeDescriptor.Route);
                }
            }
        }

        public void PublishAll(RouteCollection routeCollection)
        {
            var routes = new List<RouteDescriptor>();
            var routeProviders = _typeFinder.FindClassesOfType<IRouteProvider>();
            foreach (var providerType in routeProviders)
            {
                var provider = Activator.CreateInstance(providerType) as IRouteProvider;
                if (provider != null)
                    provider.GetRoutes(routes);
            }
            Publish(routeCollection, routes);
        }
    }
}
