using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using AbcWarehouse.Plugin.Widgets.UniFi.Models;
using Nop.Plugin.Misc.AbcCore.Infrastructure;

namespace AbcWarehouse.Plugin.Widgets.UniFi.Components
{
    public class UniFiViewComponent : NopViewComponent
    {
        private readonly ILogger _logger;
        private readonly UniFiSettings _settings;

        public UniFiViewComponent(
            ILogger logger,
            UniFiSettings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            if (widgetZone == PublicWidgetZones.BodyEndHtmlTagBefore)
            {
                var partnerId = _settings.PartnerId;
                if (string.IsNullOrWhiteSpace(partnerId))
                {
                    await _logger.ErrorAsync("Widgets.UniFi: Partner ID not defined, add in Settings.");
                    return Content(string.Empty);
                }

                var model = new UniFiModel
                {
                    PartnerId = partnerId
                };

                if (IsProductDetailsPage()) { model.FlowType = "PDP"; }

                return View("~/Plugins/Widgets.UniFi/Views/Framework.cshtml", model);
            }

            if (widgetZone == CustomPublicWidgetZones.ProductDetailsAfterPrice)
            {
                return View("~/Plugins/Widgets.UniFi/Views/SyncPrice.cshtml");
            }

            return Content(string.Empty);
        }

        private bool IsProductDetailsPage()
        {
            var routeData = Url.ActionContext.RouteData;
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];
            return controller.ToString() == "Product" && action.ToString() == "ProductDetails";
        }
    }
}