using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Factories;
using Nop.Plugin.Misc.AbcCore.Models;
using Nop.Web.Framework.Components;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Components
{
    public class CartSlideoutPickupInStoreViewComponent : NopViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<ProductStock> productStock)
        {
            return View("~/Plugins/Widgets.CartSlideout/Views/_PickupInStore.cshtml");
        }
    }
}
