using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Nop.Web.Framework.Themes;

namespace DataShop.DemoPlugin.Infrastructure
{
    //public class MyViewEngine : RazorViewEngine
    public class MyViewEngine : ThemeableRazorViewEngine
    {
        public MyViewEngine()
        {
            string viewPath = "~/Plugins/DataShop.DemoPlugin/Views/{1}/{0}.cshtml";
            ViewLocationFormats = new[] { viewPath };
            PartialViewLocationFormats = new[] { viewPath };
            AreaViewLocationFormats = new[] { viewPath };
            AreaPartialViewLocationFormats = new[] { viewPath };
        }
    }
}
