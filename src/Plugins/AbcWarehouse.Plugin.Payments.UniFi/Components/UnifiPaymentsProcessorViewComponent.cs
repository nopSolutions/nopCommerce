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
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Core.Domain.Orders;
using Nop.Services.Customers;
using Nop.Services.Directory;
using System.Net.Http.Json;
using Nop.Services.Common;
using Nop.Plugin.Misc.AbcCore.Services;

namespace AbcWarehouse.Plugin.Payments.UniFi.Components
{
    [ViewComponent(Name = "UniFiPaymentsProcessor")]

    public class UnifiPaymentsProcessorViewComponent : NopViewComponent
    {
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly ITermLookupService _termLookupService;
        private readonly IWorkContext _workContext;
        private readonly UniFiPaymentsSettings _uniFiPaymentsSettings;
        private readonly UniFiSettings _uniFiSettings;

        public UnifiPaymentsProcessorViewComponent(
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IShoppingCartService shoppingCartService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            ITermLookupService termLookupService,
            IWorkContext workContext,
            UniFiPaymentsSettings uniFiPaymentsSettings,
            UniFiSettings uniFiSettings)
        {
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _shoppingCartService = shoppingCartService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _termLookupService = termLookupService;
            _workContext = workContext;
            _uniFiPaymentsSettings = uniFiPaymentsSettings;
            _uniFiSettings = uniFiSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var bearerToken = await GetBearerTokenAsync();
            await _genericAttributeService.SaveAttributeAsync(
                await _workContext.GetCurrentCustomerAsync(),
                "UniFiBearerToken",
                bearerToken);
            var transactionToken = await GetTransactionTokenAsync(bearerToken);

            var customer = await _workContext.GetCurrentCustomerAsync();
            var address = await _customerService.GetCustomerAddressAsync(customer.Id, customer.BillingAddressId.Value);
            var stateAbbreviation = 
                (await _stateProvinceService.GetStateProvinceByIdAsync(address.StateProvinceId.Value)).Abbreviation;

            var cart = await _shoppingCartService.GetShoppingCartAsync(
                customer,
                ShoppingCartType.ShoppingCart,
                (await _storeContext.GetCurrentStoreAsync()).Id);
            var orderTotalsModel = await _shoppingCartModelFactory.PrepareOrderTotalsModelAsync(cart, false);
            var termLookup = await _termLookupService.GetTermAsync(cart);

            var model = new PaymentInfoModel() {
                TransactionToken = transactionToken,
                TokenId = transactionToken,
                PartnerId = _uniFiSettings.PartnerId,
                ClientTransactionId = Guid.NewGuid().ToString("N"),
                Address1 = address.Address1,
                Address2 = address.Address2,
                City = address.City,
                State = stateAbbreviation,
                Zip = address.ZipPostalCode,
                TransactionAmount = orderTotalsModel.OrderTotal.Replace("$", "").Replace(",", ""),
                Tags = termLookup.termNo ?? "",
            };

            return View("~/Plugins/Payments.UniFi/Views/PaymentInfo.cshtml", model);
        }

        private async Task<string> GetTransactionTokenAsync(string bearerToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var transactionTokenEndpoint = _uniFiPaymentsSettings.UseIntegration ?
                "https://api-stg.syf.com/v1/dpos/utility/token" :
                "https://api.syf.com/v1/dpos/utility/token";
            var response = await client.PostAsJsonAsync(
                transactionTokenEndpoint,
                new { syfPartnerId = _uniFiSettings.PartnerId }
            );

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new NopException("Payments.UniFi: Failure to retrieve transaction token.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseContent);
            var transactionToken = responseJson.RootElement.GetProperty("transactionToken").GetString();

            return transactionToken;
        }

        private async Task<string> GetBearerTokenAsync()
        {
            var client = new HttpClient();
            var oauth2Endpoint = _uniFiPaymentsSettings.UseIntegration ?
                "https://api-stg.syf.com/oauth2/v1/token" :
                "https://api.syf.com/oauth2/v1/token";

            var response = await client.PostAsync(
                oauth2Endpoint,
                new FormUrlEncodedContent(new Dictionary<string, string>() {
                    { "grant_type", "client_credentials" },
                    { "client_id", _uniFiPaymentsSettings.ClientId },
                    { "client_secret", _uniFiPaymentsSettings.ClientSecret },
                }));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new NopException("Payments.UniFi: Failure to retrieve bearer token.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseContent);
            var bearerToken = responseJson.RootElement.GetProperty("access_token").GetString();

            return bearerToken;
        }
    }
}