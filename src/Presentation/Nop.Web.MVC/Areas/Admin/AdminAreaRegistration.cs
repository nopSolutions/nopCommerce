using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Web.MVC.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.Add(
                new Route(
                    "Admin/Categories",
                    new RouteValueDictionary 
                    {
                        {"area", "Admin"},
                        {"controller", "Category"},
                        {"action", "List"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary 
                    {
                        {"area", "Admin"}
                    },
                    new MvcRouteHandler()));

            context.Routes.Add(
                new Route(
                    "Admin/Categories/Add",
                    new RouteValueDictionary 
                    {
                        {"area", "Admin"},
                        {"controller", "Category"},
                        {"action", "Add"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary 
                    {
                        {"area", "Admin"}
                    },
                    new MvcRouteHandler()));

            context.Routes.Add(
                new Route(
                    "Admin/Categories/TestListGridData",
                    new RouteValueDictionary 
                    {
                        {"area", "Admin"},
                        {"controller", "Category"},
                        {"action", "TestListGridData"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary 
                    {
                        {"area", "Admin"}
                    },
                    new MvcRouteHandler()));
        }
    }
}
