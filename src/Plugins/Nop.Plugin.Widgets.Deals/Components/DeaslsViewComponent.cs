using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.Deals.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.Deals.Components;

public class DealsViewComponent : NopViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        var model = new PublicInfoModel();

        return View("~/Plugins/Widgets.Deals/Views/PublicInfo.cshtml", model);
    }
}