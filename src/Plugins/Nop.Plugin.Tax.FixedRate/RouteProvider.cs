using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Tax.FixedRate
{
    public partial class RouteProvider : IRouteProvider
    {
        protected IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] 
            {
                new RouteDescriptor 
                {
                     Route = new Route("Plugins/FixedTaxRate/Configure",
                         new RouteValueDictionary 
                         {
                             {"controller", "Config"},
                             {"action", "Configure"}
                         },
                         new RouteValueDictionary(),
                         new RouteValueDictionary()
                         {
                             {"Namespaces", "Nop.Plugin.Tax.FixedRate"}
                         },
                         new MvcRouteHandler())
                }


            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }
    }
}
