using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbcWarehouse.Plugin.Widgets.GA4.Models;
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
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Checkout;

namespace AbcWarehouse.Plugin.Widgets.GA4.Components
{
    public class GA4ViewComponent : NopViewComponent
    {
        private readonly GA4Settings _settings;
        private readonly ILogger _logger;

        public GA4ViewComponent(
            GA4Settings settings,
            ILogger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            if (string.IsNullOrWhiteSpace(_settings.GoogleTag))
            {
                await _logger.ErrorAsync("Widgets.GA4: Google tag not defined, add in Settings.");
                return Content(string.Empty);
            }

            return View("~/Plugins/Widgets.GA4/Views/Tag.cshtml", _settings.GoogleTag);
        }
    }
}