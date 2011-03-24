using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Web.Framework.Mvc.Routes
{
    public interface IRoutePublisher
    {
        void Publish(IEnumerable<RouteDescriptor> routes);
    }
}
