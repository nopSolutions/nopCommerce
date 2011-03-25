using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Web.Framework.Mvc.Routes
{
    public interface IRouteProvider
    {
        void GetRoutes(ICollection<RouteDescriptor> routes);
    }
}
