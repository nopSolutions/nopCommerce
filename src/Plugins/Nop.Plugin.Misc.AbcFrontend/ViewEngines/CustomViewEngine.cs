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
                    // this will need to be fixed to allow both Category and Product to be used.
                    "~/Areas/Admin/Views/Category/{0}.cshtml",
                    "~/Areas/Admin/Views/Product/{0}.cshtml"
                }, false);
        }
    }
}
