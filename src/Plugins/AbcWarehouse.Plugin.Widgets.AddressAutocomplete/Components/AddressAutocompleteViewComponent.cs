using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbcWarehouse.Plugin.Widgets.AddressAutocomplete.Models;
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
using Nop.Plugin.Misc.AbcCore.Infrastructure;

namespace AbcWarehouse.Plugin.Widgets.AddressAutocomplete.Components
{
    public class AddressAutocompleteViewComponent : NopViewComponent
    {
        private readonly AddressAutocompleteSettings _settings;
        private readonly ILogger _logger;

        public AddressAutocompleteViewComponent(
            AddressAutocompleteSettings settings,
            ILogger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            if (string.IsNullOrWhiteSpace(_settings.GooglePlacesApiKey))
            {
                await _logger.ErrorAsync("Widgets.AddressAutocomplete: Google Places API key not defined, add in Settings.");
                return Content(string.Empty);
            }

            return Content("<div>works!</div>");
        }
    }
}