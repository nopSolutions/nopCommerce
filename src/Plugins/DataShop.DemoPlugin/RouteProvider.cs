using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;
using DataShop.DemoPlugin.Infrastructure;

namespace DataShop.DemoPlugin
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority { get { return 0; } }

        public void RegisterRoutes(RouteCollection routes)
        {
            ViewEngines.Engines.Insert(0, new MyViewEngine());

            routes.MapRoute("DataShop.DemoPlugin.Index",
                "DemoPlugin/DemoItems",
                new { controller = "DemoItems", action = "Index" },
                new[] { "DataShop.DemoPlugin.Controllers" }
            );

            var route1 = routes.MapRoute("DataShop.DemoPlugin.Admin.Index",
              "Admin/DemoPlugin/DemoItems",
              new { controller = "AdminDemoItems", action = "Index", area = "Admin" },
              new[] { "DataShop.DemoPlugin.Controllers" }
            );

            var route2 = routes.MapRoute("DataShop.DemoPlugin.Admin.GetDemoItem",
             "Admin/DemoPlugin/DemoItems/Item/{id}",
             new { controller = "AdminDemoItems", action = "GetDemoItem", area = "Admin" },
             new[] { "DataShop.DemoPlugin.Controllers" }
           );

            routes.Remove(route1);
            routes.Remove(route2);

            routes.Insert(0, route1);
            routes.Insert(0, route2);
        }
    }
}
