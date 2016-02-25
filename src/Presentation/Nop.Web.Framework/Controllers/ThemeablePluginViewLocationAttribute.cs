using System;

namespace Nop.Web.Framework.Controllers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ThemeablePluginViewLocationAttribute : Attribute
    {
        public string BaseViewLocation { get; set; }
    }
}
