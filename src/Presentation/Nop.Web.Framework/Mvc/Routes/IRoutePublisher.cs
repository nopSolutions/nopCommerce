using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Nop.Web.Framework.Mvc.Routes
{
    public interface IRoutePublisher
    {
        void Publish(RouteCollection routeCollection, IEnumerable<RouteDescriptor> routes);

        void PublishAll(RouteCollection routeCollection);
    }
}
