using System.Collections.Generic;

namespace Nop.Plugin.Misc.AbcSynchronyPayments.ViewEngines
{
    public class AbcSynchronyPaymentsViewEngine : SevenSpikes.Nop.Framework.ViewLocations.ViewLocationsManager
    {
        public AbcSynchronyPaymentsViewEngine()
        {
            this.AddViewLocationFormats(
                new List<string> {
                    "~/Plugins/Widgets.AbcSynchronyPayments/Views/{0}.cshtml"
                }, true);
        }
    }
}
