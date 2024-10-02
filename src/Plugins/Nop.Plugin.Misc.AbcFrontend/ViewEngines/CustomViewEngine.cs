using System.Collections.Generic;

namespace Nop.Plugin.Misc.AbcFrontend.ViewEngines
{
    public class CustomViewEngine : SevenSpikes.Nop.Framework.ViewLocations.ViewLocationsManager
    {
        public CustomViewEngine()
        {
            this.AddViewLocationFormats(
                new List<string> {
                    "~/Plugins/Misc.AbcFrontend/Views/{1}/{0}.cshtml",
                    "~/Plugins/Misc.AbcFrontend/Views/Shared/{0}.cshtml",
                    "~/Plugins/Misc.AbcFrontend/Views/{0}.cshtml",
                    // used to connect with Nop checkout views and custom controllers
                    "/Views/Checkout/{0}.cshtml"
                }, true);

            this.AddAdminViewLocationFormats(
                new List<string> {
                    "~/Areas/Admin/Views/Product/{0}.cshtml"
                }, false);
        }
    }
}
