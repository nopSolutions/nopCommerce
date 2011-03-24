using System.Web.Mvc;
using Nop.Web.Framework;

namespace Nop.Admin
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
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new {controller = "Admin", action = "Index", id = ""},
                new[] {"Nop.Admin.Controllers"}
            );
        }
    }
}
