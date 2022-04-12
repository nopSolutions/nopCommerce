using System.Collections.Generic;

namespace AbcWarehouse.Plugin.Misc.SLI.ViewEngines
{
    public class CustomViewEngine : SevenSpikes.Nop.Framework.ViewLocations.ViewLocationsManager
    {
        public CustomViewEngine()
        {
            AddViewLocationFormats(
                new List<string>
                {
                    "~/Plugins/Misc.SLI/Views/{1}/{0}.cshtml",
                    "~/Plugins/Misc.SLI/Views/Shared/{0}.cshtml",
                    "~/Plugins/Misc.SLI/Views/{0}.cshtml",
                }, true);
        }
    }
}
