using System.Web.Mvc;
using Nop.Web.Framework;

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
            ModelBinders.Binders.DefaultBinder = new NopModelBinder();

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = "" }
            );

            //context.MapRoute(
            //    "admin_categories_list",
            //    "Admin/Categories/List",
            //    new { controller = "Category", action = "List" }
            //);

            //context.MapRoute(
            //    "admin_categories_edit",
            //    "Admin/Categories/Edit/{id}",
            //    new { controller = "Category", action = "Edit" }
            //);

            //context.MapRoute(
            //    "admin_categories_json_allCategories",
            //    "Admin/Categories/AllCategories",
            //    new { controller = "Category", action = "AllCategories" });

            //context.MapRoute(
            //    "admin_default",
            //    "Admin/{controller}/{action}",
            //    new { controller = "Category", action = "List" });



            //context.Routes.Add(
            //    new Route(
            //        "Admin/Categories",
            //        new RouteValueDictionary 
            //        {
            //            {"area", "Admin"},
            //            {"controller", "Category"},
            //            {"action", "List"}
            //        },
            //        new RouteValueDictionary(),
            //        new RouteValueDictionary 
            //        {
            //            {"area", "Admin"}
            //        },
            //        new MvcRouteHandler()));

            //context.Routes.Add(
            //    new Route(
            //        "Admin/Categories/Add",
            //        new RouteValueDictionary 
            //        {
            //            {"area", "Admin"},
            //            {"controller", "Category"},
            //            {"action", "Add"}
            //        },
            //        new RouteValueDictionary(),
            //        new RouteValueDictionary 
            //        {
            //            {"area", "Admin"}
            //        },
            //        new MvcRouteHandler()));

            //context.Routes.Add(
            //    new Route(
            //        "Admin/Categories/List",
            //        new RouteValueDictionary 
            //        {
            //            {"area", "Admin"},
            //            {"controller", "Category"},
            //            {"action", "List"}
            //        },
            //        new RouteValueDictionary(),
            //        new RouteValueDictionary 
            //        {
            //            {"area", "Admin"}
            //        },
            //        new MvcRouteHandler()));
        }
    }
}
