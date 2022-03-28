using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Synchrony;
using Nop.Plugin.Payments.Synchrony.Models;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;
using Nop.Core.Http.Extensions;
using Nop.Core.Domain.Logging;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Synchrony.Components
{
    [ViewComponent(Name = "Synchrony")]
    public class SynchronyViewComponent : NopViewComponent
    {
        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        private readonly IHttpContextAccessor _httpContext;
        private readonly SynchronyPaymentSettings _settings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SynchronyViewComponent(
            ILogger logger,
            IStoreContext storeContext,
            IHttpContextAccessor httpContext,
            SynchronyPaymentSettings settings,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _logger = logger;
            _storeContext = storeContext;
            _httpContext = httpContext;
            _settings = settings;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            //Stored Merchant Id & Password
            var merchantId = _settings.MerchantId;
            var merchantPassword = _settings.MerchantPassword;

            AuthenticationTokenResponse model = new AuthenticationTokenResponse();

            // get authorization region end point
            string authorizationRegionURL = _settings.Integration == true
                ? SynchronyPaymentConstants.DemoAuthEndpoint
                : SynchronyPaymentConstants.LiveAuthEndpoint;

            // take reference from below link - Answer 1  by Seema As
            // https://stackoverflow.com/questions/39190018/how-to-get-object-using-httpclient-with-response-ok-in-web-api


            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(authorizationRegionURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0");

                List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();

                keyValues.Add(new KeyValuePair<string, string>("merchantId", merchantId));
                keyValues.Add(new KeyValuePair<string, string>("password", merchantPassword));

                if (_settings.IsDebugMode)
                {
                    await _logger.InsertLogAsync(
                        LogLevel.Debug,
                        "Synchrony.Payments: Auth Request",
                        "curl --request POST " +
                                $"--url {authorizationRegionURL} " +
                                "--header 'content-type: application/x-www-form-urlencoded' " +
                                $"--data merchantId={merchantId} " +
                                $"--data 'password={merchantPassword}'");
                }

                HttpContent content = new FormUrlEncodedContent(keyValues);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                content.Headers.ContentType.CharSet = "UTF-8";

                var result = client.PostAsync(authorizationRegionURL, content).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;

                if (_settings.IsDebugMode)
                {
                    await _logger.InsertLogAsync(
                        LogLevel.Debug,
                        "Synchrony.Payments: Auth Response",
                        resultContent);
                }

                AuthenticationTokenResponse authResponse;
                try
                {
                    authResponse = JsonSerializer.Deserialize<AuthenticationTokenResponse>(resultContent);
                }
                catch (JsonException e)
                {
                    await _logger.ErrorAsync($"Issue when converting Synchrony response to JSON, response was {resultContent}: ", e);

                    _httpContextAccessor.HttpContext.Session.SetString("PaymentMethodError", "There was an error when processing Synchrony payments. Please select another payment method.");
                    _httpContextAccessor.HttpContext.Response.Redirect("/checkout/paymentmethod");
                    return Content("");
                }

                string token = authResponse.clientToken;
                _httpContext.HttpContext.Session.Set("token", token);
                string postBackId = authResponse.postbackid;

                model.MerchantId = merchantId;
                model.MerchantPassword = merchantPassword;
                model.clientToken = token;
                model.postbackid = postBackId;

                Random r = new Random();
                model.clientTransId = r.Next(11, int.MaxValue).ToString();
                model.Integration = _settings.Integration;
            }

            return View("~/Plugins/Payments.Synchrony/Views/PaymentInfo.cshtml", model);
        }
    }
}