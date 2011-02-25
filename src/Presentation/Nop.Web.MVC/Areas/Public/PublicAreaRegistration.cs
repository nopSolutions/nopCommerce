using System.Web.Mvc;
using Nop.Web.Framework;

namespace Nop.Web.MVC.Areas.Public
{
    public class PublicAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Public";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Public_default",
                "Public/{controller}/{action}/{id}",
                new { action = "List", id = "" }
            );
        }
    }
}
