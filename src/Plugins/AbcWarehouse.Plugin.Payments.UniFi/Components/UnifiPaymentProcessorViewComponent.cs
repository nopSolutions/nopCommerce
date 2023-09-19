using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;
using Nop.Core.Http.Extensions;
using Nop.Core.Domain.Logging;
using System.Threading.Tasks;
using AbcWarehouse.Plugin.Widgets.UniFi;
using AbcWarehouse.Plugin.Payments.UniFi.Models;

namespace AbcWarehouse.Plugin.Payments.UniFi.Components
{
    public class UnifiPaymentProcessorViewComponent : NopViewComponent
    {
        private readonly UniFiSettings _uniFiSettings;

        public UnifiPaymentProcessorViewComponent(UniFiSettings uniFiSettings)
        {
            _uniFiSettings = uniFiSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new PaymentInfoModel() {
                PartnerId = _uniFiSettings.PartnerId,
            };

            // var cart = await _shoppingCartService.GetShoppingCartAsync(
            //     await _workContext.GetCurrentCustomerAsync(),
            //     ShoppingCartType.ShoppingCart,
            //     (await _storeContext.GetCurrentStoreAsync()).Id);

            // var orderTotalsModel = await _shoppingCartModelFactory.PrepareOrderTotalsModelAsync(cart, false);

            return View("~/Plugins/Payments.UniFi/Views/PaymentInfo.cshtml", model);
        }
    }
}