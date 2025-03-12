using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.Logging;
using Nop.Services.Messages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Data;
using Nop.Web.Framework.Components;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Core.Domain.Security;
using Nop.Services.Customers;
using Nop.Services.Catalog;
using System.IO;
using Nop.Plugin.Misc.AbcCore;
using Nop.Web.Models.Catalog;
using System.Threading.Tasks;
using Nop.Services.Common;
using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Plugin.Misc.AbcCore.Models;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.AbcCore.Components
{
    [ViewComponent(Name = "AbcCore")]
    public class AbcCoreViewComponent : NopViewComponent
    {
        private readonly CoreSettings _coreSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly INotificationService _notificationService;

        public AbcCoreViewComponent(
            CoreSettings coreSettings,
            IGenericAttributeService genericAttributeService,
            INotificationService notificationService
        ) {
            _coreSettings = coreSettings;
            _genericAttributeService = genericAttributeService;
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, ProductModel additionalData = null)
        {
            if (widgetZone == AdminWidgetZones.ProductDetailsBlock)
            {
                var productId = additionalData.Id;
                var plpDescription = await _genericAttributeService.GetAttributeAsync<Product, string>(
                    productId, "PLPDescription"
                );

                var model = new ABCProductDetailsModel
                {
                    ProductId = productId,
                    PLPDescription = plpDescription
                };

                return View("~/Plugins/Misc.AbcCore/Views/ProductDetails.cshtml", model);
            }
            else if (widgetZone == AdminWidgetZones.HeaderBefore && !_coreSettings.IsValid())
            {
                _notificationService.ErrorNotification(
                    string.Format(
                        "Misc.AbcCore - Please <a href=\"{0}\">configure</a> the plugin to ensure working functionality.",
                        Url.Action("Configure", "AbcCore")),
                        false);
            }
            else if (widgetZone == PublicWidgetZones.Footer)
            {
                string sha = File.ReadAllText("Plugins/Misc.AbcCore/sha.txt").Trim();
                string branch = File.ReadAllText("Plugins/Misc.AbcCore/branch.txt").Trim();
                return View("~/Plugins/Misc.AbcCore/Views/BuildInfo.cshtml", (sha, branch));
            }

            return Content("");
        }

    }
}
