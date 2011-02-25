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
            //TODO move "ModelBinders.Binders.DefaultBinder..." to another place
            ModelBinders.Binders.DefaultBinder = new NopModelBinder();

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = "" }
            );
        }
    }
}
