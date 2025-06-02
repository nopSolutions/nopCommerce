using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.MediaMigration.Components;
public class ProductMediaImportButtonViewComponent : NopViewComponent
{
    public IViewComponentResult Invoke(string widgetZone, object additionalData)
    {
        return View("~/Plugins/Misc.MediaMigration/Views/ProductMediaImportButton.cshtml");
    }
}

