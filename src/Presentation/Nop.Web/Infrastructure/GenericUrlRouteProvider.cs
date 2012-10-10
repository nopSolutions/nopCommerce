using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Framework.Seo;

namespace Nop.Web.Infrastructure
{
    public partial class GenericUrlRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //generic URLs
            routes.MapGenericPathRoute("GenericUrl",
                                       "{generic_se_name}",
                                       new {controller = "Common", action = "GenericUrl"},
                                       new[] {"Nop.Web.Controllers"});

            //define this routes to use in UI views (in case if you want to customize some of them later)
            routes.MapLocalizedRoute("Product",
                                     "{SeName}",
                                     new {controller = "Catalog", action = "Product"},
                                     new[] {"Nop.Web.Controllers"});
            routes.MapLocalizedRoute("Category",
                            "{SeName}",
                            new { controller = "Catalog", action = "Category" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapLocalizedRoute("Manufacturer",
                            "{SeName}",
                            new { controller = "Catalog", action = "Manufacturer" },
                            new[] { "Nop.Web.Controllers" });



            routes.MapLocalizedRoute(
                "404-PageNotFound",
                "{*url}",
                new { controller = "Common", action = "PageNotFound" },
                new[] { "Nop.Web.Controllers" });
        }

        public int Priority
        {
            get
            {
                //it should be the last route
                return -500;
            }
        }
    }
}
