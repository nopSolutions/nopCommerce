using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.DropShipping.AliExpress.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.DropShipping.AliExpress.Components;

/// <summary>
/// View component for AliExpress product selector
/// </summary>
[ViewComponent(Name = "AliExpressProductSelector")]
public class AliExpressProductSelectorViewComponent : NopViewComponent
{
    public IViewComponentResult Invoke(string widgetZone, object additionalData)
    {
        var model = new ProductSelectorModel
        {
            ProductId = additionalData as int? ?? 0
        };

        return View("~/Plugins/DropShipping.AliExpress/Views/ProductSelector.cshtml", model);
    }
}
