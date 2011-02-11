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
            ModelBinders.Binders.Add(typeof (Nop.Web.Framework.TelerikGridContext),
                                     new Nop.Web.Framework.TelerikGridModelBinder());
            context.MapRoute(
                "admin_categories_list",
                "Admin/Categories/List",
                new { controller = "Category", action = "List" }
            );

            context.MapRoute(
                "admin_categories_edit",
                "Admin/Categories/Edit/{id}",
                new { controller = "Category", action = "Edit" }
            );

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
