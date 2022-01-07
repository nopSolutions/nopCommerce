using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Payments.PayPalCommerce.Domain.Onboarding;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework.Infrastructure;
using PayPal.v1.Webhooks;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
using PayPalHttp;

namespace Nop.Plugin.Payments.PayPalCommerce.Services
{
    /// <summary>
    /// Represents the plugin service manager
    /// </summary>
    public class ServiceManager
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IAddressService _addresService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly OnboardingHttpClient _onboardingHttpClient;

        #endregion

        #region Ctor

        public ServiceManager(CurrencySettings currencySettings,
            IActionContextAccessor actionContextAccessor,
            IAddressService addresService,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICountryService countryService,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            ILogger logger,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IProductService productService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreService storeService,
            ITaxService taxService,
            IUrlHelperFactory urlHelperFactory,
            IWebHelper webHelper,
            IWorkContext workContext,
            OnboardingHttpClient onboardingHttpClient)
        {
            _currencySettings = currencySettings;
            _actionContextAccessor = actionContextAccessor;
            _addresService = addresService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _countryService = countryService;
            _currencyService = currencyService;
            _genericAttributeService = genericAttributeService;
            _logger = logger;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _storeService = storeService;
            _taxService = taxService;
            _urlHelperFactory = urlHelperFactory;
            _webHelper = webHelper;
            _workContext = workContext;
            _onboardingHttpClient = onboardingHttpClient;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Handle function and get result
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="function">Function</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result; error message if exists
        /// </returns>
        private async Task<(TResult Result, string Error)> HandleFunctionAsync<TResult>(Func<Task<TResult>> function)
        {
            try
            {
                //invoke function
                return (await function(), default);
            }
            catch (Exception exception)
            {
                //get a short error message
                var message = exception.Message;
                if (exception is HttpException httpException)
                {
                    //get error details if exist
                    var details = JsonConvert.DeserializeObject<ExceptionDetails>(httpException.Message);
                    message = details.Message?.Trim('.') ?? details.Name ?? message;
                    if (details?.Details?.Any() ?? false)
                    {
                        message += details.Details.Aggregate(":", (text, issue) => $"{text} " +
                            $"{(issue.Description ?? issue.Issue).Trim('.')}{(!string.IsNullOrEmpty(issue.Field) ? $"({issue.Field})" : null)},").Trim(',');
                    }
                }

                //log errors
                var logMessage = $"{PayPalCommerceDefaults.SystemName} error: {System.Environment.NewLine}{message}";
                await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());

                return (default, message);
            }
        }

        /// <summary>
        /// Handle request to checkout services and get result
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="settings">Plugin settings</param>
        /// <param name="request">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        private static async Task<TResult> HandleCheckoutRequestAsync<TRequest, TResult>(PayPalCommerceSettings settings, TRequest request)
            where TRequest : HttpRequest where TResult : class
        {
            //prepare common request params
            request.Headers.Add(HeaderNames.UserAgent, PayPalCommerceDefaults.UserAgent);
            request.Headers.Add("PayPal-Partner-Attribution-Id", PayPalCommerceDefaults.PartnerCode);
            request.Headers.Add("Prefer", "return=representation");

            //execute request
            var client = new PayPalHttpClient(settings.UseSandbox
                ? new SandboxEnvironment(settings.ClientId, settings.SecretKey)
                : new LiveEnvironment(settings.ClientId, settings.SecretKey));
            client.SetConnectTimeout(TimeSpan.FromSeconds(settings.RequestTimeout ?? 10));
            var response = await client.Execute(request)
                ?? throw new NopException("No response from the service");

            //return the results if necessary
            if (typeof(TResult) == typeof(object))
                return default;

            var result = response.Result<TResult>()
                ?? throw new NopException("No response from the service");

            return result;
        }

        /// <summary>
        /// Handle request to core services and get result
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="settings">Plugin settings</param>
        /// <param name="request">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        private static async Task<TResult> HandleCoreRequestAsync<TRequest, TResult>(PayPalCommerceSettings settings, TRequest request)
            where TRequest : BraintreeHttp.HttpRequest where TResult : class
        {
            //prepare common request params
            request.Headers.Add(HeaderNames.UserAgent, PayPalCommerceDefaults.UserAgent);
            request.Headers.Add("PayPal-Partner-Attribution-Id", PayPalCommerceDefaults.PartnerCode);
            request.Headers.Add("Prefer", "return=representation");

            //execute request
            var client = new PayPal.Core.PayPalHttpClient(settings.UseSandbox
                ? new PayPal.Core.SandboxEnvironment(settings.ClientId, settings.SecretKey)
                : new PayPal.Core.LiveEnvironment(settings.ClientId, settings.SecretKey));
            client.SetConnectTimeout(TimeSpan.FromSeconds(settings.RequestTimeout ?? 10));
            var response = await client.Execute(request)
                ?? throw new NopException("No response from the service");

            //return the results if necessary
            if (typeof(TResult) == typeof(object))
                return default;

            var result = response.Result<TResult>()
                ?? throw new NopException("No response from the service");

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check whether the plugin is configured
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <returns>Result</returns>
        public static bool IsConfigured(PayPalCommerceSettings settings)
        {
            //client id and secret are required to request services
            return !string.IsNullOrEmpty(settings?.ClientId) && !string.IsNullOrEmpty(settings?.SecretKey);
        }

        /// <summary>
        /// Get access token
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the access token; error message if exists
        /// </returns>
        public async Task<(AccessToken AccessToken, string Error)> GetAccessTokenAsync(PayPalCommerceSettings settings)
        {
            //try to get access token
            return await HandleFunctionAsync(async () =>
            {
                var request = new AccessTokenRequest(settings.UseSandbox
                    ? new SandboxEnvironment(settings.ClientId, settings.SecretKey)
                    : new LiveEnvironment(settings.ClientId, settings.SecretKey));

                return await HandleCheckoutRequestAsync<AccessTokenRequest, AccessToken>(settings, request);
            });
        }

        /// <summary>
        /// Prepare service script
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="widgetZone">Widget zone name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the script; error message if exists
        /// </returns>
        public async Task<(string Script, string Error)> GetScriptAsync(PayPalCommerceSettings settings, string widgetZone)
        {
            return await HandleFunctionAsync(async () =>
            {
                //ensure that plugin is configured
                if (!IsConfigured(settings))
                    throw new NopException("Plugin not configured");

                var components = new List<string>() { "buttons" };

                var parameters = new Dictionary<string, string>
                {
                    ["client-id"] = settings.ClientId,
                    ["currency"] = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode?.ToUpperInvariant(),
                    ["intent"] = settings.PaymentType.ToString().ToLowerInvariant(),
                    ["commit"] = (settings.PaymentType == Domain.PaymentType.Capture).ToString().ToLowerInvariant(),
                    ["vault"] = false.ToString().ToLowerInvariant(),
                    ["debug"] = false.ToString().ToLowerInvariant(),
                    ["components"] = "",
                    //["buyer-country"] = null, //available in the sandbox only
                    //["locale"] = null, //PayPal auto detects this
                };
                if (!string.IsNullOrEmpty(settings.DisabledFunding))
                    parameters["disable-funding"] = settings.DisabledFunding;
                if (!string.IsNullOrEmpty(settings.EnabledFunding))
                    parameters["enable-funding"] = settings.EnabledFunding;
                if (widgetZone.Equals(PublicWidgetZones.OrderSummaryContentBefore) || widgetZone.Equals(PublicWidgetZones.ProductDetailsTop))
                    components.Add("funding-eligibility");
                if (settings.DisplayPayLaterMessages)
                    components.Add("messages");
                parameters["components"] = string.Join(",", components);
                var scriptUrl = QueryHelpers.AddQueryString(PayPalCommerceDefaults.ServiceScriptUrl, parameters);

                var pageType = widgetZone.Equals(PublicWidgetZones.OrderSummaryContentBefore)
                    ? "cart"
                    : (widgetZone.Equals(PublicWidgetZones.ProductDetailsTop)
                    ? "product-details"
                    : "checkout");

                return $@"<script src=""{scriptUrl}"" data-partner-attribution-id=""{PayPalCommerceDefaults.PartnerCode}"" data-page-type=""{pageType}""></script>";
            });
        }

        #region Payments

        /// <summary>
        /// Create an order
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="orderGuid">Order GUID</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the created order; error message if exists
        /// </returns>
        public async Task<(Order Order, string Error)> CreateOrderAsync(PayPalCommerceSettings settings, Guid orderGuid)
        {
            return await HandleFunctionAsync(async () =>
            {
                //ensure that plugin is configured
                if (!IsConfigured(settings))
                    throw new NopException("Plugin not configured");

                var customer = await _workContext.GetCurrentCustomerAsync();
                var store = await _storeContext.GetCurrentStoreAsync();

                var currency = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
                if (string.IsNullOrEmpty(currency))
                    throw new NopException("Primary store currency not set");

                var billingAddress = await _addresService.GetAddressByIdAsync(customer.BillingAddressId ?? 0)
                    ?? throw new NopException("Customer billing address not set");

                var shoppingCart = (await _shoppingCartService
                    .GetShoppingCartAsync(customer, Core.Domain.Orders.ShoppingCartType.ShoppingCart, store.Id))
                    .ToList();

                var shippingAddress = await _addresService.GetAddressByIdAsync(customer.ShippingAddressId ?? 0);
                if (!await _shoppingCartService.ShoppingCartRequiresShippingAsync(shoppingCart))
                    shippingAddress = null;

                var billStateProvince = await _stateProvinceService.GetStateProvinceByAddressAsync(billingAddress);
                var shipStateProvince = await _stateProvinceService.GetStateProvinceByAddressAsync(shippingAddress);

                //prepare order details
                var orderDetails = new OrderRequest { CheckoutPaymentIntent = settings.PaymentType.ToString().ToUpperInvariant() };

                //prepare some common properties
                orderDetails.ApplicationContext = new ApplicationContext
                {
                    BrandName = CommonHelper.EnsureMaximumLength(store.Name, 127),
                    LandingPage = LandingPageType.Billing.ToString().ToUpperInvariant(),
                    UserAction = settings.PaymentType == Domain.PaymentType.Authorize
                        ? UserActionType.Continue.ToString().ToUpperInvariant()
                        : UserActionType.Pay_now.ToString().ToUpperInvariant(),
                    ShippingPreference = (shippingAddress != null ? ShippingPreferenceType.Set_provided_address : ShippingPreferenceType.No_shipping)
                        .ToString().ToUpperInvariant()
                };

                //prepare customer billing details
                orderDetails.Payer = new Payer
                {
                    Name = new Name
                    {
                        GivenName = CommonHelper.EnsureMaximumLength(billingAddress.FirstName, 140),
                        Surname = CommonHelper.EnsureMaximumLength(billingAddress.LastName, 140)
                    },
                    Email = CommonHelper.EnsureMaximumLength(billingAddress.Email, 254),
                    AddressPortable = new AddressPortable
                    {
                        AddressLine1 = CommonHelper.EnsureMaximumLength(billingAddress.Address1, 300),
                        AddressLine2 = CommonHelper.EnsureMaximumLength(billingAddress.Address2, 300),
                        AdminArea2 = CommonHelper.EnsureMaximumLength(billingAddress.City, 120),
                        AdminArea1 = CommonHelper.EnsureMaximumLength(billStateProvince?.Abbreviation, 300),
                        CountryCode = (await _countryService.GetCountryByIdAsync(billingAddress.CountryId ?? 0))?.TwoLetterIsoCode,
                        PostalCode = CommonHelper.EnsureMaximumLength(billingAddress.ZipPostalCode, 60)
                    }
                };
                if (!string.IsNullOrEmpty(billingAddress.PhoneNumber))
                {
                    var cleanPhone = CommonHelper.EnsureMaximumLength(CommonHelper.EnsureNumericOnly(billingAddress.PhoneNumber), 14);
                    orderDetails.Payer.PhoneWithType = new PhoneWithType { PhoneNumber = new Phone { NationalNumber = cleanPhone } };
                }

                //prepare purchase unit details
                var taxTotal = Math.Round((await _orderTotalCalculationService.GetTaxTotalAsync(shoppingCart, false)).taxTotal, 2);
                var shippingTotal = Math.Round(await _orderTotalCalculationService.GetShoppingCartShippingTotalAsync(shoppingCart) ?? decimal.Zero, 2);
                var (shoppingCartTotal, _, _, _, _, _) = await _orderTotalCalculationService
                    .GetShoppingCartTotalAsync(shoppingCart, usePaymentMethodAdditionalFee: false);
                var orderTotal = Math.Round(shoppingCartTotal ?? decimal.Zero, 2);

                var purchaseUnit = new PurchaseUnitRequest
                {
                    ReferenceId = CommonHelper.EnsureMaximumLength(orderGuid.ToString(), 256),
                    CustomId = CommonHelper.EnsureMaximumLength(orderGuid.ToString(), 127),
                    Description = CommonHelper.EnsureMaximumLength($"Purchase at '{store.Name}'", 127),
                    SoftDescriptor = CommonHelper.EnsureMaximumLength(store.Name, 22)
                };

                //prepare shipping address details
                if (shippingAddress != null)
                {
                    purchaseUnit.ShippingDetail = new ShippingDetail
                    {
                        Name = new Name { FullName = CommonHelper.EnsureMaximumLength($"{shippingAddress.FirstName} {shippingAddress.LastName}", 300) },
                        AddressPortable = new AddressPortable
                        {
                            AddressLine1 = CommonHelper.EnsureMaximumLength(shippingAddress.Address1, 300),
                            AddressLine2 = CommonHelper.EnsureMaximumLength(shippingAddress.Address2, 300),
                            AdminArea2 = CommonHelper.EnsureMaximumLength(shippingAddress.City, 120),
                            AdminArea1 = CommonHelper.EnsureMaximumLength(shipStateProvince?.Abbreviation, 300),
                            CountryCode = (await _countryService.GetCountryByIdAsync(billingAddress.CountryId ?? 0))?.TwoLetterIsoCode,
                            PostalCode = CommonHelper.EnsureMaximumLength(shippingAddress.ZipPostalCode, 60)
                        }
                    };
                }

                //set order items
                purchaseUnit.Items = await shoppingCart.SelectAwait(async item =>
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);

                    var (unitPrice, _, _) = await _shoppingCartService.GetUnitPriceAsync(item, true);
                    var (itemPrice, _) = await _taxService.GetProductPriceAsync(product, unitPrice, false, customer);
                    return new Item
                    {
                        Name = CommonHelper.EnsureMaximumLength(product.Name, 127),
                        Description = CommonHelper.EnsureMaximumLength(product.ShortDescription, 127),
                        Sku = CommonHelper.EnsureMaximumLength(product.Sku, 127),
                        Quantity = item.Quantity.ToString(),
                        Category = (product.IsDownload ? ItemCategoryType.Digital_goods : ItemCategoryType.Physical_goods)
                            .ToString().ToUpperInvariant(),
                        UnitAmount = new PayPalCheckoutSdk.Orders.Money { CurrencyCode = currency, Value = itemPrice.ToString("0.00", CultureInfo.InvariantCulture) }
                    };
                }).ToListAsync();

                //add checkout attributes as order items
                var checkoutAttributes = await _genericAttributeService
                    .GetAttributeAsync<string>(customer, NopCustomerDefaults.CheckoutAttributes, store.Id);
                var checkoutAttributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttributes);
                await foreach (var (attribute, values) in checkoutAttributeValues)
                {
                    await foreach (var attributeValue in values)
                    {
                        var (attributePrice, _) = await _taxService.GetCheckoutAttributePriceAsync(attribute, attributeValue, false, customer);
                        purchaseUnit.Items.Add(new Item
                        {
                            Name = CommonHelper.EnsureMaximumLength(attribute.Name, 127),
                            Description = CommonHelper.EnsureMaximumLength($"{attribute.Name} - {attributeValue.Name}", 127),
                            Quantity = 1.ToString(),
                            UnitAmount = new PayPalCheckoutSdk.Orders.Money { CurrencyCode = currency, Value = attributePrice.ToString("0.00", CultureInfo.InvariantCulture) }
                        });
                    }
                }

                //set totals
                //there may be a problem with a mismatch of amounts since ItemTotal should equal sum of (unit amount * quantity) across all items
                //but PayPal forcibly rounds all amounts to two decimal, so the more items, the higher the chance of rounding errors
                //we obviously cannot change the order total, so slightly adjust other totals to match all requirements
                var itemTotal = Math.Round(purchaseUnit.Items.Sum(item =>
                    decimal.Parse(item.UnitAmount.Value, NumberStyles.Any, CultureInfo.InvariantCulture) * int.Parse(item.Quantity)), 2);
                var discountTotal = Math.Round(itemTotal + taxTotal + shippingTotal - orderTotal, 2);
                if (discountTotal < decimal.Zero || discountTotal < settings.MinDiscountAmount)
                {
                    taxTotal -= discountTotal;
                    discountTotal = decimal.Zero;
                }
                purchaseUnit.AmountWithBreakdown = new AmountWithBreakdown
                {
                    CurrencyCode = currency,
                    Value = orderTotal.ToString("0.00", CultureInfo.InvariantCulture),
                    AmountBreakdown = new AmountBreakdown
                    {
                        ItemTotal = new PayPalCheckoutSdk.Orders.Money { CurrencyCode = currency, Value = itemTotal.ToString("0.00", CultureInfo.InvariantCulture) },
                        TaxTotal = new PayPalCheckoutSdk.Orders.Money { CurrencyCode = currency, Value = taxTotal.ToString("0.00", CultureInfo.InvariantCulture) },
                        Shipping = new PayPalCheckoutSdk.Orders.Money { CurrencyCode = currency, Value = shippingTotal.ToString("0.00", CultureInfo.InvariantCulture) },
                        Discount = new PayPalCheckoutSdk.Orders.Money { CurrencyCode = currency, Value = discountTotal.ToString("0.00", CultureInfo.InvariantCulture) }
                    }
                };

                orderDetails.PurchaseUnits = new List<PurchaseUnitRequest> { purchaseUnit };

                var orderRequest = new OrdersCreateRequest().RequestBody(orderDetails);
                return await HandleCheckoutRequestAsync<OrdersCreateRequest, Order>(settings, orderRequest);
            });
        }

        /// <summary>
        /// Authorize a previously created order
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="orderId">Order id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the authorized order; error message if exists
        /// </returns>
        public async Task<(Order Order, string Error)> AuthorizeAsync(PayPalCommerceSettings settings, string orderId)
        {
            return await HandleFunctionAsync(async () =>
            {
                //ensure that plugin is configured
                if (!IsConfigured(settings))
                    throw new NopException("Plugin not configured");

                var request = new OrdersAuthorizeRequest(orderId).RequestBody(new AuthorizeRequest());

                return await HandleCheckoutRequestAsync<OrdersAuthorizeRequest, Order>(settings, request);
            });
        }

        /// <summary>
        /// Capture a previously created order
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="orderId">Order id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the captured order; error message if exists
        /// </returns>
        public async Task<(Order Order, string Error)> CaptureAsync(PayPalCommerceSettings settings, string orderId)
        {
            return await HandleFunctionAsync(async () =>
            {
                //ensure that plugin is configured
                if (!IsConfigured(settings))
                    throw new NopException("Plugin not configured");

                var request = new OrdersCaptureRequest(orderId).RequestBody(new OrderActionRequest());

                return await HandleCheckoutRequestAsync<OrdersCaptureRequest, Order>(settings, request);
            });
        }

        /// <summary>
        /// Capture an authorization
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="authorizationId">Authorization id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the capture details; error message if exists
        /// </returns>
        public async Task<(PayPalCheckoutSdk.Payments.Capture Capture, string Error)> CaptureAuthorizationAsync
            (PayPalCommerceSettings settings, string authorizationId)
        {
            return await HandleFunctionAsync(async () =>
            {
                //ensure that plugin is configured
                if (!IsConfigured(settings))
                    throw new NopException("Plugin not configured");

                var request = new AuthorizationsCaptureRequest(authorizationId).RequestBody(new CaptureRequest());

                return await HandleCheckoutRequestAsync<AuthorizationsCaptureRequest, PayPalCheckoutSdk.Payments.Capture>(settings, request);
            });
        }

        /// <summary>
        /// Void an authorization
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="authorizationId">Authorization id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the voided order; Error message if exists
        /// </returns>
        public async Task<(object Order, string Error)> VoidAsync(PayPalCommerceSettings settings, string authorizationId)
        {
            return await HandleFunctionAsync(async () =>
            {
                //ensure that plugin is configured
                if (!IsConfigured(settings))
                    throw new NopException("Plugin not configured");

                var request = new AuthorizationsVoidRequest(authorizationId);

                return await HandleCheckoutRequestAsync<AuthorizationsVoidRequest, object>(settings, request);
            });
        }

        /// <summary>
        /// Refund a captured payment
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="captureId">Capture id</param>
        /// <param name="currency">Currency code</param>
        /// <param name="amount">Amount to refund</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the refund details; error message if exists
        /// </returns>
        public async Task<(PayPalCheckoutSdk.Payments.Refund Refund, string Error)> RefundAsync
            (PayPalCommerceSettings settings, string captureId, string currency, decimal? amount = null)
        {
            return await HandleFunctionAsync(async () =>
            {
                //ensure that plugin is configured
                if (!IsConfigured(settings))
                    throw new NopException("Plugin not configured");

                var refundRequest = new RefundRequest();
                if (amount.HasValue)
                {
                    refundRequest.Amount = new PayPalCheckoutSdk.Payments.Money
                    {
                        CurrencyCode = currency,
                        Value = amount.Value.ToString("0.00", CultureInfo.InvariantCulture)
                    };
                }
                var request = new CapturesRefundRequest(captureId).RequestBody(refundRequest);

                return await HandleCheckoutRequestAsync<CapturesRefundRequest, PayPalCheckoutSdk.Payments.Refund>(settings, request);
            });
        }

        #endregion

        #region Webhooks

        /// <summary>
        /// Get webhook by the URL
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="webhookUrl">Webhook URL</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the webhook; error message if exists
        /// </returns>
        public async Task<(Webhook Webhook, string Error)> GetWebhookAsync(PayPalCommerceSettings settings, string webhookUrl)
        {
            return await HandleFunctionAsync(async () =>
            {
                //ensure that plugin is configured
                if (!IsConfigured(settings))
                    throw new NopException("Plugin not configured");

                var webhookList = await HandleCoreRequestAsync<WebhookListRequest, WebhookList>(settings, new WebhookListRequest());
                var webhookByUrl = webhookList?.Webhooks
                    ?.FirstOrDefault(webhook => webhook.Url?.Equals(webhookUrl, StringComparison.InvariantCultureIgnoreCase) ?? false);

                return webhookByUrl;
            });
        }

        /// <summary>
        /// Create webhook that receive events for the subscribed event types
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="storeId">Store id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the webhook; error message if exists
        /// </returns>
        public async Task<(Webhook Webhook, string Error)> CreateWebhookAsync(PayPalCommerceSettings settings, int storeId)
        {
            return await HandleFunctionAsync(async () =>
            {
                //ensure that plugin is configured
                if (!IsConfigured(settings))
                    throw new NopException("Plugin not configured");

                //prepare webhook URL
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
                var store = storeId > 0
                    ? await _storeService.GetStoreByIdAsync(storeId)
                    : await _storeContext.GetCurrentStoreAsync();
                var webhookUrl = $"{store.Url.TrimEnd('/')}{urlHelper.RouteUrl(PayPalCommerceDefaults.WebhookRouteName)}".ToLowerInvariant();

                //check whether the webhook already exists
                var (webhook, _) = await GetWebhookAsync(settings, webhookUrl);
                if (webhook is not null)
                    return webhook;

                //or try to create the new one if doesn't exist
                var request = new WebhookCreateRequest().RequestBody(new Webhook
                {
                    EventTypes = PayPalCommerceDefaults.WebhookEventNames.Select(name => new EventType { Name = name }).ToList(),
                    Url = webhookUrl
                });

                return await HandleCoreRequestAsync<WebhookCreateRequest, Webhook>(settings, request);
            });
        }

        /// <summary>
        /// Delete webhook
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DeleteWebhookAsync(PayPalCommerceSettings settings)
        {
            await HandleFunctionAsync(async () =>
            {
                //ensure that plugin is configured
                if (!IsConfigured(settings))
                    throw new NopException("Plugin not configured");

                var (webhook, _) = await GetWebhookAsync(settings, settings.WebhookUrl);
                if (webhook is null)
                    return null;

                return await HandleCoreRequestAsync<WebhookDeleteRequest, object>(settings, new WebhookDeleteRequest(webhook.Id));
            });
        }

        /// <summary>
        /// Handle webhook request
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="request">HTTP request</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleWebhookAsync(PayPalCommerceSettings settings, Microsoft.AspNetCore.Http.HttpRequest request)
        {
            await HandleFunctionAsync(async () =>
            {
                //ensure that plugin is configured
                if (!IsConfigured(settings))
                    throw new NopException("Plugin not configured");

                //get request details
                var rawRequestString = string.Empty;
                using (var streamReader = new StreamReader(request.Body))
                    rawRequestString = await streamReader.ReadToEndAsync();

                if (string.IsNullOrEmpty(settings.WebhookUrl))
                    throw new NopException("Webhook is not set");

                var (webhook, _) = await GetWebhookAsync(settings, settings.WebhookUrl);
                if (webhook is null)
                    throw new NopException($"No webhook configured for URL '{settings.WebhookUrl}'");

                //define a local function to validate the webhook event and get an appropriate resource
                async Task<object> getWebhookResource<TResource>() where TResource : class
                {
                    //verify webhook event data
                    var webhookEvent = JsonConvert.DeserializeObject<Event<TResource>>(rawRequestString);
                    var verifyRequest = new WebhookVerifySignatureRequest<TResource>().RequestBody(new VerifyWebhookSignature<TResource>
                    {
                        AuthAlgo = request.Headers["PAYPAL-AUTH-ALGO"],
                        CertUrl = request.Headers["PAYPAL-CERT-URL"],
                        TransmissionId = request.Headers["PAYPAL-TRANSMISSION-ID"],
                        TransmissionSig = request.Headers["PAYPAL-TRANSMISSION-SIG"],
                        TransmissionTime = request.Headers["PAYPAL-TRANSMISSION-TIME"],
                        WebhookId = webhook.Id,
                        WebhookEvent = webhookEvent
                    });
                    var result = await HandleCoreRequestAsync<WebhookVerifySignatureRequest<TResource>, VerifyWebhookSignatureResponse>(settings, verifyRequest);

                    // This would be hard to always get it to success, as the result is dependent on time of webhook sent.
                    // As long as we get a 200 response, we should be fine.
                    //see details here https://github.com/paypal/PayPal-NET-SDK/commit/16e5bebfd4021d0888679e526cdf1f4f19527f3e#diff-ee79bcc68a8451d30522e1d9b2c5bc13R36
                    return result != null ? webhookEvent?.Resource : default;
                }

                //try to get webhook resource type
                var webhookResourceType = JsonConvert.DeserializeObject<Event<object>>(rawRequestString).ResourceType?.ToLowerInvariant();

                //and now get specific webhook resource
                var webhookResource = webhookResourceType switch
                {
                    "checkout-order" => await getWebhookResource<Order>(),
                    "authorization" => await getWebhookResource<PayPalCheckoutSdk.Payments.Authorization>(),
                    "capture" => await getWebhookResource<PayPalCheckoutSdk.Payments.Capture>(),
                    "refund" => await getWebhookResource<PayPalCheckoutSdk.Payments.Refund>(),
                    _ => null
                };
                if (webhookResource is null)
                    throw new NopException($"Unknown webhook resource type '{webhookResourceType}'");

                var orderReference = webhookResource is Order payPalOrder
                    ? payPalOrder.PurchaseUnits?.FirstOrDefault()?.CustomId
                    : JsonConvert.DeserializeObject<Event<ExtendedWebhookResource>>(rawRequestString).Resource?.CustomId;
                if (!Guid.TryParse(orderReference, out var orderGuid))
                    throw new NopException($"Could not recognize an order reference '{orderReference}'");

                var order = await _orderService.GetOrderByGuidAsync(orderGuid)
                    ?? throw new NopException($"Could not find an order {orderGuid}");

                await _orderService.InsertOrderNoteAsync(new Core.Domain.Orders.OrderNote()
                {
                    OrderId = order.Id,
                    Note = $"Webhook details: {System.Environment.NewLine}{rawRequestString}",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                //authorization actions
                var authorization = webhookResource as PayPalCheckoutSdk.Payments.Authorization;
                switch (authorization?.Status?.ToLowerInvariant())
                {
                    case "created":
                        if (decimal.TryParse(authorization.Amount?.Value, out var authorizedAmount) && authorizedAmount == Math.Round(order.OrderTotal, 2))
                        {
                            //all is ok, so authorize order
                            if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
                            {
                                order.AuthorizationTransactionId = authorization.Id;
                                order.AuthorizationTransactionResult = authorization.Status;
                                await _orderService.UpdateOrderAsync(order);
                                await _orderProcessingService.MarkAsAuthorizedAsync(order);
                            }
                        }
                        break;

                    case "denied":
                    case "expired":
                    case "pending":
                        order.CaptureTransactionResult = authorization.Status;
                        order.OrderStatus = Core.Domain.Orders.OrderStatus.Pending;
                        await _orderService.UpdateOrderAsync(order);
                        await _orderProcessingService.CheckOrderStatusAsync(order);
                        break;

                    case "voided":
                        if (_orderProcessingService.CanVoidOffline(order))
                        {
                            order.AuthorizationTransactionId = authorization.Id;
                            order.AuthorizationTransactionResult = authorization.Status;
                            await _orderService.UpdateOrderAsync(order);
                            await _orderProcessingService.VoidOfflineAsync(order);
                        }
                        break;
                }

                //capture actions
                var capture = webhookResource as PayPalCheckoutSdk.Payments.Capture;
                switch (capture?.Status?.ToLowerInvariant())
                {
                    case "completed":
                        if (decimal.TryParse(capture.Amount?.Value, out var capturedAmount) && capturedAmount == Math.Round(order.OrderTotal, 2))
                        {
                            if (_orderProcessingService.CanMarkOrderAsPaid(order))
                            {
                                order.CaptureTransactionId = capture.Id;
                                order.CaptureTransactionResult = capture.Status;
                                await _orderService.UpdateOrderAsync(order);
                                await _orderProcessingService.MarkOrderAsPaidAsync(order);
                            }
                        }
                        break;

                    case "pending":
                    case "declined":
                        order.CaptureTransactionResult = $"{capture.Status}. {capture.StatusDetails?.Reason}";
                        order.OrderStatus = Core.Domain.Orders.OrderStatus.Pending;
                        await _orderService.UpdateOrderAsync(order);
                        await _orderProcessingService.CheckOrderStatusAsync(order);
                        break;

                    case "refunded":
                        if (_orderProcessingService.CanRefundOffline(order))
                            await _orderProcessingService.RefundOfflineAsync(order);
                        break;
                }

                //refund actions
                var refund = webhookResource as PayPalCheckoutSdk.Payments.Refund;
                switch (refund?.Status?.ToLowerInvariant())
                {
                    case "completed":
                        var refundIds = await _genericAttributeService.GetAttributeAsync<List<string>>(order, PayPalCommerceDefaults.RefundIdAttributeName)
                            ?? new List<string>();
                        if (!refundIds.Contains(refund.Id))
                        {
                            if (decimal.TryParse(refund.Amount?.Value, out var refundedAmount))
                            {
                                if (_orderProcessingService.CanPartiallyRefundOffline(order, refundedAmount))
                                {
                                    await _orderProcessingService.PartiallyRefundOfflineAsync(order, refundedAmount);
                                    refundIds.Add(refund.Id);
                                    await _genericAttributeService.SaveAttributeAsync(order, PayPalCommerceDefaults.RefundIdAttributeName, refundIds);
                                }
                            }
                        }
                        break;
                }

                //order actions
                payPalOrder = webhookResource as Order;
                switch (payPalOrder?.Status?.ToLowerInvariant())
                {
                    case "approved":
                        if (decimal.TryParse(payPalOrder.PurchaseUnits?.FirstOrDefault()?.AmountWithBreakdown?.Value, out var approvedAmount) && approvedAmount == Math.Round(order.OrderTotal, 2))
                        {
                            //all is ok, so authorize the approved order
                            if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
                            {
                                order.AuthorizationTransactionResult = payPalOrder.Status;
                                await _orderService.UpdateOrderAsync(order);
                                await _orderProcessingService.MarkAsAuthorizedAsync(order);
                            }
                        }
                        break;
                }

                return true;
            });
        }

        #endregion

        #region Onboarding

        /// <summary>
        /// Get details to sign up a merchant
        /// </summary>
        /// <param name="email">Merchant email</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the merchant details; error message if exists
        /// </returns>
        public async Task<(Merchant Merchant, string Error)> OnboardAsync(string email)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (string.IsNullOrEmpty(email))
                    throw new NopException("Email is not set");

                var language = await _workContext.GetWorkingLanguageAsync();
                var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var store = storeId > 0
                    ? await _storeService.GetStoreByIdAsync(storeId)
                    : await _storeContext.GetCurrentStoreAsync();
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
                var redirectUrl = urlHelper.RouteUrl(PayPalCommerceDefaults.ConfigurationRouteName, null, _webHelper.GetCurrentRequestProtocol());
                var request = new OnboardingRequest
                {
                    Email = email,
                    Culture = language.LanguageCulture,
                    StoreUrl = $"{store.Url.TrimEnd('/')}/",
                    RedirectUrl = redirectUrl
                };
                var merchant = await _onboardingHttpClient.RequestAsync<OnboardingRequest, Merchant>(request)
                    ?? throw new NopException($"Empty result");

                if (string.IsNullOrEmpty(merchant.SignUpUrl))
                    throw new NopException($"URL to sign up is empty");

                return merchant;
            });
        }

        /// <summary>
        /// Sign up a merchant with the passed authentication parameters
        /// </summary>
        /// <param name="merchantGuid">Merchant internal id</param>
        /// <param name="authCode">Authentication parameters</param>
        /// <param name="sharedId">Authentication parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the merchant details; error message if exists
        /// </returns>
        public async Task<(Merchant Merchant, string Error)> SignUpAsync(string merchantGuid, string authCode, string sharedId)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (string.IsNullOrEmpty(merchantGuid))
                    throw new NopException("Merchant internal id is not set");

                if (string.IsNullOrEmpty(sharedId) || string.IsNullOrEmpty(authCode))
                    throw new NopException("Authentication parameters are empty");

                var request = new SignUpRequest(merchantGuid)
                {
                    AuthCode = authCode,
                    SharedId = sharedId
                };
                var merchant = await _onboardingHttpClient.RequestAsync<SignUpRequest, Merchant>(request)
                    ?? throw new NopException($"Empty result");

                return merchant;
            });
        }

        /// <summary>
        /// Get the merchant details
        /// </summary>
        /// <param name="merchantGuid">Merchant internal id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the merchant details; error message if exists
        /// </returns>
        public async Task<(Merchant Merchant, string Error)> GetMerchantAsync(string merchantGuid)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (string.IsNullOrEmpty(merchantGuid))
                    throw new NopException("Merchant internal id is not set");

                var request = new MerchantRequest(merchantGuid);
                var merchant = await _onboardingHttpClient.RequestAsync<MerchantRequest, Merchant>(request)
                    ?? throw new NopException($"Empty result");

                return merchant;
            });
        }

        /// <summary>
        /// Revoke access
        /// </summary>
        /// <param name="merchantGuid">Merchant internal id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of request; error message if exists
        /// </returns>
        public async Task<(bool Result, string Error)> RevokeAccessAsync(string merchantGuid)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (string.IsNullOrEmpty(merchantGuid))
                    throw new NopException("Merchant internal id is not set");

                var request = new RevokeAccessRequest(merchantGuid);
                await _onboardingHttpClient.RequestAsync<RevokeAccessRequest, object>(request);

                return true;
            });
        }

        #endregion

        #endregion
    }
}