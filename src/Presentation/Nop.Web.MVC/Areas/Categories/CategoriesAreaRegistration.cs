using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Web.MVC.Areas.Categories
{
    public class CategoriesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Categories";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.Add(
                new Route(
                    "Admin/Categories",
                    new RouteValueDictionary 
                    {
                        {"area", "Categories"},
                        {"controller", "CategoryAdmin"},
                        {"action", "List"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary 
                    {
                        {"area", "Categories"}
                    },
                    new MvcRouteHandler()));

            context.Routes.Add(
                new Route(
                    "Admin/Categories/Add",
                    new RouteValueDictionary 
                    {
                        {"area", "Categories"},
                        {"controller", "CategoryAdmin"},
                        {"action", "Add"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary 
                    {
                        {"area", "Categories"}
                    },
                    new MvcRouteHandler()));

            context.Routes.Add(
                new Route(
                    "Admin/Categories/TestListGridData",
                    new RouteValueDictionary 
                    {
                        {"area", "Categories"},
                        {"controller", "CategoryAdmin"},
                        {"action", "TestListGridData"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary 
                    {
                        {"area", "Categories"}
                    },
                    new MvcRouteHandler()));
        }
    }
}
