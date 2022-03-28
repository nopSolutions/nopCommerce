using System;
using System.Collections.Generic;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;
using Nop.Core;
using Nop.Services.Stores;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Synchrony.Models;
using System.Net;
using System.IO;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Tax;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Core.Caching;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Messages;
using Nop.Core.Http.Extensions;
using Nop.Web.Models.Checkout;
using Nop.Web.Factories;
using Nop.Web.Models.ShoppingCart;
using System.Text.Json;
using Nop.Services.Customers;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.AbcCore.Services;
using System.Threading.Tasks;
using System.Net.Http;
using Nop.Core.Http;
using System.Text;

namespace Nop.Plugin.Payments.Synchrony.Controllers
{
    public class SynchronyPaymentController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly PaymentSettings _paymentSettings;
        private readonly SynchronyPaymentSettings _synchronyPaymentSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ITaxService _taxService;
        private readonly IPriceCalculationService _priceCalculationService;

        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IHttpContextAccessor _httpContext = EngineContext.Current.Resolve<IHttpContextAccessor>();
        private readonly IGenericAttributeService _genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        private readonly INotificationService _notificationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IAddressService _addressService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICustomerService _customerService;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly IPaymentPluginManager _paymentPluginManager;

        private readonly IGiftCardService _giftCardService;
        private readonly IIsamGiftCardService _isamGiftCardService;

        private readonly IHttpClientFactory _httpClientFactory;

        public SynchronyPaymentController(
            IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            IPaymentService paymentService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            ILogger logger,
            IWebHelper webHelper,
            PaymentSettings paymentSettings,
            SynchronyPaymentSettings synchronyPaymentSettings,
            OrderSettings orderSettings,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            ITaxService taxService,
            IPriceCalculationService priceCalculationService,
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            IStaticCacheManager staticCacheManager,
            INotificationService notificationService,
            IShoppingCartService shoppingCartService,
            IProductService productService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IAddressService addressService,
            IStateProvinceService stateProvinceService,
            ICustomerService customerService,
            ICheckoutModelFactory checkoutModelFactory,
            IPaymentPluginManager paymentPluginManager,
            IGiftCardService giftCardService,
            IIsamGiftCardService isamGiftCardService,
            IHttpClientFactory httpClientFactory
        )
        {
            _workContext = workContext;
            _storeService = storeService;
            _settingService = settingService;
            _paymentService = paymentService;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _logger = logger;
            _webHelper = webHelper;
            _paymentSettings = paymentSettings;
            _synchronyPaymentSettings = synchronyPaymentSettings;
            _orderSettings = orderSettings;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
            _taxService = taxService;
            _priceCalculationService = priceCalculationService;
            _productAttributeFormatter = productAttributeFormatter;
            _productAttributeParser = productAttributeParser;
            _staticCacheManager = staticCacheManager;
            _notificationService = notificationService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _addressService = addressService;
            _stateProvinceService = stateProvinceService;
            _customerService = customerService;
            _checkoutModelFactory = checkoutModelFactory;
            _paymentPluginManager = paymentPluginManager;
            _giftCardService = giftCardService;
            _isamGiftCardService = isamGiftCardService;
            _httpClientFactory = httpClientFactory;
        }

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var paySynchronyPaymentSettings = await _settingService.LoadSettingAsync<SynchronyPaymentSettings>(storeScope);

            return View(
                "~/Plugins/Payments.Synchrony/Views/Configure.cshtml",
                await paySynchronyPaymentSettings.ToModelAsync(storeScope)
            );
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var paySynchronyPaymentSettings = SynchronyPaymentSettings.FromModel(model);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(paySynchronyPaymentSettings, x => x.MerchantId, model.MerchantId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paySynchronyPaymentSettings, x => x.MerchantPassword, model.MerchantPassword_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paySynchronyPaymentSettings, x => x.Integration, model.Integration_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paySynchronyPaymentSettings, x => x.TokenNumber, model.TokenNumber_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paySynchronyPaymentSettings, x => x.WhitelistDomain, model.WhitelistDomain_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paySynchronyPaymentSettings, x => x.IsDebugMode, model.IsDebugMode_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpPost]
        public async Task<IActionResult> PaymentPostInfo()
        {
            var model = new AuthenticationTokenResponse();
            try
            {
                model = await FindStatusCallAsync();

                //session save
                HttpContext.Session.Set("syfPaymentInfo", model);

                if (model.StatusMessage != "Account Authentication Success")
                {
                    HttpContext.Session.SetString("PaymentMethodError", model.StatusMessage);
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "Error",
                            html = await RenderPartialViewToStringAsync("~/Plugins/Payments.Synchrony/Views/ErrorMessagepopup.cshtml", null)
                        },
                        goto_section = "Error"
                    });
                }
            }
            catch (Exception ex)
            {
                await _logger.WarningAsync(ex.Message, ex, await _workContext.GetCurrentCustomerAsync());
                HttpContext.Session.SetString("PaymentMethodError", "An error occurred.");
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "Error",
                        html = await RenderPartialViewToStringAsync("~/Plugins/Payments.Synchrony/Views/ErrorMessagepopup.cshtml", null)
                    },
                    goto_section = "Error"
                });
            }

            TempData["SecondModalpopup"] = JsonSerializer.Serialize(model);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "Third-Modal-Method",
                    html = await RenderPartialViewToStringAsync("~/Plugins/Payments.Synchrony/Views/ThirdModalpopup.cshtml", model)
                },
                goto_section = "Third_Modal_Method"
            });
        }

        // uses custom view
        public async Task<IActionResult> Confirm()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCartAsync(
                await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart,
                (await _storeContext.GetCurrentStoreAsync()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()) &&
                !_orderSettings.AnonymousCheckoutAllowed)
            {
                return Challenge();
            }

            //model
            var model = await PrepareConfirmOrderModelAsync(cart);

            return View("~/Plugins/Payments.Synchrony/Views/Confirm.cshtml", model);
        }

        [HttpPost]
        public async Task<JsonResult> PaymentPostInfoStatus()
        {
            var cart = await _shoppingCartService.GetShoppingCartAsync(
                await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart,
                (await _storeContext.GetCurrentStoreAsync()).Id);

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var paySynchronyPaymentSettings = await _settingService.LoadSettingAsync<SynchronyPaymentSettings>(storeScope);

            try
            {
                var model = await FindStatusCallAsync();

                //session save
                HttpContext.Session.Set("syfPaymentInfo", model);

                if (model.StatusMessage != "Auth Approved")
                {
                    HttpContext.Session.SetString("PaymentMethodError", model.StatusMessage);
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "Error",
                            html = await RenderPartialViewToStringAsync("~/Plugins/Payments.Synchrony/Views/ErrorMessagepopup.cshtml", null)
                        },
                        goto_section = "Error"
                    });
                }
            }
            catch (Exception ex)
            {
                await _logger.WarningAsync(ex.Message, ex, await _workContext.GetCurrentCustomerAsync());
                HttpContext.Session.SetString("PaymentMethodError", "An error occurred.");
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "Error",
                        html = await RenderPartialViewToStringAsync("~/Plugins/Payments.Synchrony/Views/ErrorMessagepopup.cshtml", null)
                    },
                    goto_section = "Error"
                });
            }
            return Json(new { success = 1 });
        }

        [HttpPost]
        public async Task<IActionResult> SavePaymentInfo(IFormCollection form)
        {
            try
            {
                //validation
                var cart = await _shoppingCartService.GetShoppingCartAsync(
                    await _workContext.GetCurrentCustomerAsync(),
                    ShoppingCartType.ShoppingCart,
                    (await _storeContext.GetCurrentStoreAsync()).Id);

                if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()) &&
                    !_orderSettings.AnonymousCheckoutAllowed)
                {
                    throw new Exception("Anonymous checkout is not allowed");
                }

                var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(
                    await _workContext.GetCurrentCustomerAsync(),
                    NopCustomerDefaults.SelectedPaymentMethodAttribute,
                    (await _storeContext.GetCurrentStoreAsync()).Id);
                var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(
                    paymentMethodSystemName,
                    await _workContext.GetCurrentCustomerAsync(),
                    (await _storeContext.GetCurrentStoreAsync()).Id);
                if (paymentMethod == null)
                    throw new Exception("Payment method is not selected");

                var warnings = await paymentMethod.ValidatePaymentFormAsync(form);
                foreach (var warning in warnings)
                    ModelState.AddModelError("", warning);

                if (ModelState.IsValid)
                {
                    //get payment info
                    var paymentInfo = await paymentMethod.GetPaymentInfoAsync(form);
                    //set previous order GUID (if exists)
                    _paymentService.GenerateOrderGuid(paymentInfo);

                    //session save
                    HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);
                    if (paymentInfo != null)
                    {
                        return Json(new { status = true });
                    }
                    else
                    {
                        HttpContext.Session.SetString(
                            "PaymentMethodError",
                            await _localizationService.GetResourceAsync("Plugins.Payments.Synchrony.paymentInfonotFound")
                        );
                        return Json(new
                        {
                            update_section = new UpdateSectionJsonModel
                            {
                                name = "Error",
                                html = await this.RenderPartialViewToStringAsync(
                                    "~/Plugins/Payments.Synchrony/Views/ErrorMessagepopup.cshtml", null
                                )
                            },
                            goto_section = "Error",
                            status = false
                        });
                    }
                }

                //If we got this far, something failed, redisplay form
                var paymenInfoModel = await _checkoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "payment-info",
                        html = await this.RenderPartialViewToStringAsync("OpcPaymentInfo", paymenInfoModel)
                    }
                });
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentCustomerAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [NonAction]
        protected virtual async Task<CustomCheckoutConfirmModel> PrepareConfirmOrderModelAsync(IList<ShoppingCartItem> cart)
        {
            var model = new CustomCheckoutConfirmModel
            {
                TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage
            };
            //min order amount validation
            bool minOrderTotalAmountOk = await _orderProcessingService.ValidateMinOrderTotalAmountAsync(cart);
            if (!minOrderTotalAmountOk)
            {
                decimal minOrderTotalAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(
                    _orderSettings.MinOrderTotalAmount, await _workContext.GetWorkingCurrencyAsync()
                );
                model.MinOrderTotalWarning = string.Format(
                    await _localizationService.GetResourceAsync("Checkout.MinOrderTotalAmount"),
                    await _priceFormatter.FormatPriceAsync(minOrderTotalAmount, true, false)
                );
            }

            model.SynchronyAuthTokenResponse = GetSynchronyAuthTokenResponse();

            return model;
        }

        private AuthenticationTokenResponse GetSynchronyAuthTokenResponse()
        {
            var authResponseJson = TempData["SecondModalpopup"].ToString();
            TempData.Keep("SecondModalpopup");
            var authTokenResponse =
                JsonSerializer.Deserialize<AuthenticationTokenResponse>(authResponseJson);

            if (authTokenResponse == null || authTokenResponse.StatusMessage != "Account Authentication Success")
            {
                throw new Exception("Unable to retrieve AuthenticationTokenResponse");
            }

            var transPromo = HttpContext.Session.GetString("TransPromo").Replace("\"", "");
            var sessionOrderTotal = HttpContext.Session.GetString("OrderTotal");
            if (!string.IsNullOrEmpty(transPromo))
            {
                authTokenResponse.PromoCode = transPromo;
            }
            if (!string.IsNullOrEmpty(sessionOrderTotal))
            {
                authTokenResponse.TransactionAmount = sessionOrderTotal;
            }

            return authTokenResponse;
        }

        [HttpGet]
        public void ParseDbuyJsonCallBack(string callbackMessage)
        {
            if (callbackMessage != null)
            {
                var authResponse = JsonSerializer.Deserialize<AuthenticationTokenResponse>(callbackMessage);
            }
        }
        #endregion

        private bool IsValidAuthTokenResponse(string message)
        {
            return message == "Auth Approved" || message == "Account Authentication Success";
        }

        private async Task ValidateGiftCardAmountsAsync()
        {
            // grab every gift card that the customer applied to this order
            IList<GiftCard> appliedGiftCards = await _giftCardService.GetActiveGiftCardsAppliedByCustomerAsync(await _workContext.GetCurrentCustomerAsync());

            if (appliedGiftCards.Count > 1)
            {
                throw new Exception("Only one gift card may be applied to an order");
            }

            foreach (GiftCard nopGiftCard in appliedGiftCards)
            {
                // check isam to make sure each gift card has the right $$
                GiftCard isamGiftCard = _isamGiftCardService.GetGiftCardInfo(nopGiftCard.GiftCardCouponCode).GiftCard;

                decimal nopAmtLeft = nopGiftCard.Amount;
                List<GiftCardUsageHistory> nopGcHistory = (await _giftCardService.GetGiftCardUsageHistoryAsync(nopGiftCard)).ToList();

                foreach (var history in nopGcHistory)
                {
                    nopAmtLeft -= history.UsedValue;
                }

                if (isamGiftCard.Amount != nopAmtLeft)
                {
                    throw new Exception("A gift card has been used since it was placed on this order");

                }
            }
        }

        private async Task<AuthenticationTokenResponse> FindStatusCallAsync()
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var paySynchronyPaymentSettings = await _settingService.LoadSettingAsync<SynchronyPaymentSettings>(storeScope);

            var cart = await _shoppingCartService.GetShoppingCartAsync(
                await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart,
                (await _storeContext.GetCurrentStoreAsync()).Id);

            var orderTotalsModel = await _shoppingCartModelFactory.PrepareOrderTotalsModelAsync(cart, false);
            string authorizationRegionStatusURL = paySynchronyPaymentSettings.Integration
                ? SynchronyPaymentConstants.DemoInquiryEndpoint
                : SynchronyPaymentConstants.LiveInquiryEndpoint;
            var merchantId = paySynchronyPaymentSettings.MerchantId;
            var merchantPassword = paySynchronyPaymentSettings.MerchantPassword;
            var Integration = paySynchronyPaymentSettings.Integration;
            var token = HttpContext.Session.GetString("token").Replace("\"", "");
            var transactionAmount = Convert.ToDecimal(orderTotalsModel.OrderTotal.Replace("$", ""));
            var model = new AuthenticationTokenResponse();
            // take reference from below link - Answer 1  by Seema As
            // https://stackoverflow.com/questions/39190018/how-to-get-object-using-httpclient-with-response-ok-in-web-api
            
            var response = new HttpResponseMessage();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(authorizationRegionStatusURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0");
                ServicePointManager.Expect100Continue = true;

                var requestBody = new
                {
                    merchantNumber = merchantId,
                    password = merchantPassword,
                    userToken = token
                };
                string json = JsonSerializer.Serialize(requestBody);

                if (_synchronyPaymentSettings.IsDebugMode)
                {
                    await _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Debug, $"FindStatusCallAsync request - {requestBody.ToString()}");
                }

                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls13 |
                    SecurityProtocolType.Tls12 |
                    SecurityProtocolType.Tls11 |
                    SecurityProtocolType.Tls;
                response = await client.PostAsync(authorizationRegionStatusURL, content);
            }

            var authResponseJsonAsString = await response.Content.ReadAsStringAsync();
            if (_synchronyPaymentSettings.IsDebugMode)
            {
                await _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Debug, $"FindStatusCallAsync response - {authResponseJsonAsString}");
            }

            if (authResponseJsonAsString.Contains("Unauthorized"))
            {
                await _logger.ErrorAsync($"Payments.Synchrony: Failed authenticating - {authResponseJsonAsString}");
                throw new HttpRequestException("There was an error, please select another payment method.");
            }

            var authResponse = JsonSerializer.Deserialize<AuthenticationTokenResponse>(
                authResponseJsonAsString
            );
                
            var customer = await _workContext.GetCurrentCustomerAsync();
            var billingAddress = await _addressService.GetAddressByIdAsync(customer.BillingAddressId.Value);
            model.Integration = Integration;
            model.MerchantId = merchantId;
            model.clientToken = token;
            model.transactionId = authResponse.transactionId;
            model.responseCode = authResponse.responseCode;
            model.responseDesc = authResponse.responseDesc;
            model.ZipCode = billingAddress.ZipPostalCode;
            model.State = (await _stateProvinceService.GetStateProvinceByAddressAsync(
                billingAddress
            )).Abbreviation;
            model.FirstName = billingAddress.FirstName;
            model.City = billingAddress.City;
            model.Address1 = billingAddress.Address1;
            model.LastName = billingAddress.LastName;
            model.StatusCode = authResponse.StatusCode;
            model.AccountNumber = authResponse.AccountNumber;
            model.StatusMessage = authResponse.StatusMessage;
            model.TransactionAmount = transactionAmount.ToString();

            model.ClientTransactionID = authResponse.ClientTransactionID;
            model.TransactionDate = authResponse.TransactionDate;
            model.TransactionDescription = authResponse.TransactionDescription;
            model.AuthCode = authResponse.AuthCode;
            model.PromoCode = authResponse.PromoCode;
            model.PostbackId = authResponse.PostbackId;

            return model;
        }
    }
}
