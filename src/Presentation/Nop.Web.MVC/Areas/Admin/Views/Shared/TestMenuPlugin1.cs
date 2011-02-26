using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.MVC.Areas.Admin.Views.Shared
{
    public class TestMenuPlugin1 : Nop.Core.Web.AdminMenuPlugin
    {
        public TestMenuPlugin1()
            : base("TestPlugin1", 2)
        {
        }

        public override void BuildMenuItem(Telerik.Web.Mvc.UI.MenuItemBuilder menuItemBuilder)
        {
            menuItemBuilder.Text("Plugin 1 (With image)").Url("/Link-to").ImageUrl("http://demos.telerik.com/aspnet-mvc/Content/Common/Icons/Suites/rep.png");
        }
    }
}