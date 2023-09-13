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
            var providerId = _settings.ProviderId;
            if (string.IsNullOrWhiteSpace(providerId))
            {
                await _logger.ErrorAsync("Widgets.UniFi: Provider ID not defined, add in Settings.");
                return Content(string.Empty);
            }

            return View("~/Plugins/Widgets.UniFi/Views/Framework.cshtml", providerId);
        }
    }
}