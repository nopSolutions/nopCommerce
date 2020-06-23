using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Domain;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using PayPal.v1.Webhooks;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
using PayPalHttp;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Services
{
    /// <summary>
    /// Represents the plugin service manager
    /// </summary>
    public class ServiceManager
    {
        #region Fields

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
        private readonly ITaxService _taxService;
        private readonly IWorkContext _workContext;
        private readonly CurrencySettings _currencySettings;

        #endregion

        #region Ctor

        public ServiceManager(IAddressService addresService,
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
            ITaxService taxService,
            IWorkContext workContext,
            CurrencySettings currencySettings)
        {
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
            _taxService = taxService;
            _workContext = workContext;
            _currencySettings = currencySettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether the plugin is configured
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <returns>Result</returns>
        private bool IsConfigured(PayPalSmartPaymentButtonsSettings settings)
        {
            //client id and secret are required to request services
            return !string.IsNullOrEmpty(settings?.ClientId) &&
                (!string.IsNullOrEmpty(settings?.SecretKey) || settings.ClientId.Equals("sb", StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Handle function and get result
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="settings">Plugin settings</param>
        /// <param name="function">Function</param>
        /// <returns>Result; error message if exists</returns>
        private (TResult Result, string ErrorMessage) HandleFunction<TResult>(PayPalSmartPaymentButtonsSettings settings, Func<TResult> function)
        {
            try
            {
                //ensure that plugin is configured
                if (!IsConfigured(settings))
                    throw new NopException("Plugin not configured");

                //invoke function
                return (function(), default);
            }
            catch (Exception exception)
            {
                //get a short error message
                var message = exception.Message;
                var detailedException = exception;
                do
                {
                    detailedException = detailedException.InnerException;
                } while (detailedException?.InnerException != null);
                if (detailedException is HttpException httpException)
                {
                    var details = JsonConvert.DeserializeObject<ExceptionDetails>(httpException.Message);
                    message = !string.IsNullOrEmpty(details.Message)
                        ? details.Message
                        : (!string.IsNullOrEmpty(details.Name) ? details.Name : message);
                }

                //log errors
                var logMessage = $"{Defaults.SystemName} error: {System.Environment.NewLine}{message}";
                _logger.Error(logMessage, exception, _workContext.CurrentCustomer);

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
        /// <returns>Result</returns>
        private TResult HandleCheckoutRequest<TRequest, TResult>(PayPalSmartPaymentButtonsSettings settings, TRequest request)
            where TRequest : HttpRequest where TResult : class
        {
            //prepare common request params
            request.Headers.Add(HeaderNames.UserAgent, Defaults.UserAgent);
            request.Headers.Add("PayPal-Partner-Attribution-Id", Defaults.PartnerCode);
            request.Headers.Add("Prefer", "return=representation");

            //execute request
            var environment = settings.UseSandbox
                ? new SandboxEnvironment(settings.ClientId, settings.SecretKey) as PayPalEnvironment
                : new LiveEnvironment(settings.ClientId, settings.SecretKey) as PayPalEnvironment;
            var client = new PayPalHttpClient(environment);
            var response = client.Execute(request)
                ?? throw new NopException("No response from the service.");

            //return the results if necessary
            if (typeof(TResult) == typeof(object))
                return default;

            var result = response.Result?.Result<TResult>()
                ?? throw new NopException("No response from the service.");

            return result;
        }

        /// <summary>
        /// Handle request to core services and get result
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="settings">Plugin settings</param>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        private TResult HandleCoreRequest<TRequest, TResult>(PayPalSmartPaymentButtonsSettings settings, TRequest request)
            where TRequest : BraintreeHttp.HttpRequest where TResult : class
        {
            //prepare common request params
            request.Headers.Add(HeaderNames.UserAgent, Defaults.UserAgent);
            request.Headers.Add("PayPal-Partner-Attribution-Id", Defaults.PartnerCode);
            request.Headers.Add("Prefer", "return=representation");

            //execute request
            var environment = settings.UseSandbox
                ? new PayPal.Core.SandboxEnvironment(settings.ClientId, settings.SecretKey) as PayPal.Core.PayPalEnvironment
                : new PayPal.Core.LiveEnvironment(settings.ClientId, settings.SecretKey) as PayPal.Core.PayPalEnvironment;
            var client = new PayPal.Core.PayPalHttpClient(environment);
            var response = client.Execute(request)
                ?? throw new NopException("No response from the service.");

            //return the results if necessary
            if (typeof(TResult) == typeof(object))
                return default;

            var result = response.Result?.Result<TResult>()
                ?? throw new NopException("No response from the service.");

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare service script
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <returns>Script; error message if exists</returns>
        public (string Script, string ErrorMessage) GetScript(PayPalSmartPaymentButtonsSettings settings)
        {
            return HandleFunction(settings, () =>
            {
                var parameters = new Dictionary<string, string>
                {
                    ["client-id"] = settings.ClientId,
                    ["currency"] = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)?.CurrencyCode?.ToUpper(),
                    ["intent"] = settings.PaymentType.ToString().ToLower(),
                    ["commit"] = false.ToString().ToLower(),
                    ["vault"] = false.ToString().ToLower(),
                    ["debug"] = false.ToString().ToLower(),
                    //["components"] = "buttons", //default value
                    //["merchant-id"] = null, //not used
                    //["integration-date"] = null, //not used (YYYY-MM-DD format)
                    //["buyer-country"] = null, //available in the sandbox only
                    //["locale"] = null, //PayPal auto detects it
                };
                if (!string.IsNullOrEmpty(settings.DisabledFunding))
                    parameters["disable-funding"] = settings.DisabledFunding;
                if (!string.IsNullOrEmpty(settings.DisabledCards))
                    parameters["disable-card"] = settings.DisabledCards;
                var scriptUrl = QueryHelpers.AddQueryString(Defaults.ServiceScriptUrl, parameters);

                return $@"<script src=""{scriptUrl}"" data-partner-attribution-id=""{Defaults.PartnerCode}""></script>";
            });
        }

        /// <summary>
        /// Create an order
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="orderGuid">Order GUID</param>
        /// <returns>Created order; error message if exists</returns>
        public (Order Order, string ErrorMessage) CreateOrder(PayPalSmartPaymentButtonsSettings settings, Guid orderGuid)
        {
            return HandleFunction(settings, () =>
            {
                var currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)?.CurrencyCode;
                if (string.IsNullOrEmpty(currency))
                    throw new NopException("Primary store currency not set");

                var billingAddress = _addresService.GetAddressById(_workContext.CurrentCustomer.BillingAddressId ?? 0);
                if (billingAddress == null)
                    throw new NopException("Customer billing address not set");

                var shippingAddress = _addresService.GetAddressById(_workContext.CurrentCustomer.ShippingAddressId ?? 0);

                var billStateProvince = _stateProvinceService.GetStateProvinceByAddress(billingAddress);
                var shipStateProvince = _stateProvinceService.GetStateProvinceByAddress(shippingAddress);

                //prepare order details
                var orderDetails = new OrderRequest { CheckoutPaymentIntent = settings.PaymentType.ToString().ToUpper() };

                //prepare some common properties
                orderDetails.ApplicationContext = new ApplicationContext
                {
                    BrandName = CommonHelper.EnsureMaximumLength(_storeContext.CurrentStore.Name, 127),
                    LandingPage = LandingPageType.Billing.ToString().ToUpper(),
                    UserAction = UserActionType.Continue.ToString().ToUpper(),
                    ShippingPreference = (shippingAddress != null ? ShippingPreferenceType.Set_provided_address : ShippingPreferenceType.No_shipping)
                        .ToString().ToUpper()
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
                        CountryCode = _countryService.GetCountryById(billingAddress.CountryId ?? 0)?.TwoLetterIsoCode,
                        PostalCode = CommonHelper.EnsureMaximumLength(billingAddress.ZipPostalCode, 60)
                    }
                };
                if (!string.IsNullOrEmpty(billingAddress.PhoneNumber))
                {
                    var cleanPhone = CommonHelper.EnsureMaximumLength(CommonHelper.EnsureNumericOnly(billingAddress.PhoneNumber), 14);
                    orderDetails.Payer.PhoneWithType = new PhoneWithType { PhoneNumber = new Phone { NationalNumber = cleanPhone } };
                }

                //prepare purchase unit details
                var shoppingCart = _shoppingCartService
                    .GetShoppingCart(_workContext.CurrentCustomer, Core.Domain.Orders.ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id)
                    .ToList();
                var taxTotal = Math.Round(_orderTotalCalculationService.GetTaxTotal(shoppingCart, false), 2);
                var shippingTotal = Math.Round(_orderTotalCalculationService.GetShoppingCartShippingTotal(shoppingCart) ?? decimal.Zero, 2);
                var orderTotal = Math.Round(_orderTotalCalculationService.GetShoppingCartTotal(shoppingCart, usePaymentMethodAdditionalFee: false) ?? decimal.Zero, 2);

                var purchaseUnit = new PurchaseUnitRequest
                {
                    ReferenceId = CommonHelper.EnsureMaximumLength(orderGuid.ToString(), 256),
                    CustomId = CommonHelper.EnsureMaximumLength(orderGuid.ToString(), 127),
                    Description = CommonHelper.EnsureMaximumLength($"Purchase at '{_storeContext.CurrentStore.Name}'", 127),
                    SoftDescriptor = CommonHelper.EnsureMaximumLength(_storeContext.CurrentStore.Name, 22)
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
                            CountryCode = _countryService.GetCountryById(billingAddress.CountryId ?? 0)?.TwoLetterIsoCode,
                            PostalCode = CommonHelper.EnsureMaximumLength(shippingAddress.ZipPostalCode, 60)
                        }
                    };
                }

                //set order items
                var itemTotal = decimal.Zero;
                purchaseUnit.Items = shoppingCart.Select(item =>
                {
                    var product = _productService.GetProductById(item.ProductId);

                    var itemPrice = Math.Round(_taxService.GetProductPrice(product,
                        _shoppingCartService.GetUnitPrice(item), false, _workContext.CurrentCustomer, out _), 2);
                    itemTotal += itemPrice * item.Quantity;
                    return new Item
                    {
                        Name = CommonHelper.EnsureMaximumLength(product.Name, 127),
                        Description = CommonHelper.EnsureMaximumLength(product.ShortDescription, 127),
                        Sku = CommonHelper.EnsureMaximumLength(product.Sku, 127),
                        Quantity = item.Quantity.ToString(),
                        Category = (product.IsDownload ? ItemCategoryType.Digital_goods : ItemCategoryType.Physical_goods)
                            .ToString().ToUpper(),
                        UnitAmount = new PayPalCheckoutSdk.Orders.Money { CurrencyCode = currency, Value = itemPrice.ToString("0.00", CultureInfo.InvariantCulture) }
                    };
                }).ToList();

                //add checkout attributes as order items
                var checkoutAttributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(_genericAttributeService
                    .GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.CheckoutAttributes, _storeContext.CurrentStore.Id));

                foreach (var (attribute, values) in checkoutAttributeValues)
                {
                    foreach (var attributeValue in values)
                    {
                        var attributePrice = _taxService.GetCheckoutAttributePrice(attribute, attributeValue, false, _workContext.CurrentCustomer);
                        var roundedAttributePrice = Math.Round(attributePrice, 2);

                        itemTotal += roundedAttributePrice;
                        purchaseUnit.Items.Add(new Item
                        {
                            Name = CommonHelper.EnsureMaximumLength(attribute.Name, 127),
                            Description = CommonHelper.EnsureMaximumLength($"{attribute.Name} - {attributeValue.Name}", 127),
                            Quantity = 1.ToString(),
                            UnitAmount = new PayPalCheckoutSdk.Orders.Money { CurrencyCode = currency, Value = roundedAttributePrice.ToString("0.00", CultureInfo.InvariantCulture) }
                        });
                    }
                }

                //set totals
                itemTotal = Math.Round(itemTotal, 2);
                var discountTotal = Math.Round(itemTotal + taxTotal + shippingTotal - orderTotal, 2);
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
                return HandleCheckoutRequest<OrdersCreateRequest, Order>(settings, orderRequest);
            });
        }

        /// <summary>
        /// Authorize a previously created order
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="orderId">Order id</param>
        /// <returns>Authorized order; error message if exists</returns>
        public (Order Order, string ErrorMessage) Authorize(PayPalSmartPaymentButtonsSettings settings, string orderId)
        {
            return HandleFunction(settings, () =>
            {
                var request = new OrdersAuthorizeRequest(orderId).RequestBody(new AuthorizeRequest());
                return HandleCheckoutRequest<OrdersAuthorizeRequest, Order>(settings, request);
            });
        }

        /// <summary>
        /// Capture a previously created order
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="orderId">Order id</param>
        /// <returns>Captured order; error message if exists</returns>
        public (Order Order, string ErrorMessage) Capture(PayPalSmartPaymentButtonsSettings settings, string orderId)
        {
            return HandleFunction(settings, () =>
            {
                var request = new OrdersCaptureRequest(orderId).RequestBody(new OrderActionRequest());
                return HandleCheckoutRequest<OrdersCaptureRequest, Order>(settings, request);
            });
        }

        /// <summary>
        /// Capture an authorization
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="authorizationId">Authorization id</param>
        /// <returns>Capture details; error message if exists</returns>
        public (PayPalCheckoutSdk.Payments.Capture Capture, string ErrorMessage) CaptureAuthorization
            (PayPalSmartPaymentButtonsSettings settings, string authorizationId)
        {
            return HandleFunction(settings, () =>
            {
                var request = new AuthorizationsCaptureRequest(authorizationId).RequestBody(new CaptureRequest());
                return HandleCheckoutRequest<AuthorizationsCaptureRequest, PayPalCheckoutSdk.Payments.Capture>(settings, request);
            });
        }

        /// <summary>
        /// Void an authorization
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="authorizationId">Authorization id</param>
        /// <returns>Voided order; Error message if exists</returns>
        public (object Order, string ErrorMessage) Void(PayPalSmartPaymentButtonsSettings settings, string authorizationId)
        {
            return HandleFunction(settings, () =>
            {
                var request = new AuthorizationsVoidRequest(authorizationId);
                return HandleCheckoutRequest<AuthorizationsVoidRequest, object>(settings, request);
            });
        }

        /// <summary>
        /// Refund a captured payment
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="captureId">Capture id</param>
        /// <param name="currency">Currency code</param>
        /// <param name="amount">Amount to refund</param>
        /// <returns>Refund details; error message if exists</returns>
        public (PayPalCheckoutSdk.Payments.Refund refund, string errorMessage) Refund
            (PayPalSmartPaymentButtonsSettings settings, string captureId, string currency, decimal? amount = null)
        {
            return HandleFunction(settings, () =>
            {
                var refundRequest = new RefundRequest();
                if (amount.HasValue)
                    refundRequest.Amount = new PayPalCheckoutSdk.Payments.Money { CurrencyCode = currency, Value = amount.Value.ToString("0.00", CultureInfo.InvariantCulture) };
                var request = new CapturesRefundRequest(captureId).RequestBody(refundRequest);
                return HandleCheckoutRequest<CapturesRefundRequest, PayPalCheckoutSdk.Payments.Refund>(settings, request);
            });
        }

        /// <summary>
        /// Get access token
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <returns>Access token; error message if exists</returns>
        public (AccessToken AccessToken, string ErrorMessage) GetAccessToken(PayPalSmartPaymentButtonsSettings settings)
        {
            //try to get access token
            return HandleFunction(settings, () =>
            {
                var environment = settings.UseSandbox
                    ? new SandboxEnvironment(settings.ClientId, settings.SecretKey) as PayPalEnvironment
                    : new LiveEnvironment(settings.ClientId, settings.SecretKey) as PayPalEnvironment;
                var request = new AccessTokenRequest(environment);
                return HandleCheckoutRequest<AccessTokenRequest, AccessToken>(settings, request);
            });
        }

        /// <summary>
        /// Create webhook that receive events for the subscribed event types
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="url">Webhook listener URL</param>
        /// <returns>Webhook; error message if exists</returns>
        public (Webhook webhook, string errorMessage) CreateWebHook(PayPalSmartPaymentButtonsSettings settings, string url)
        {
            return HandleFunction(settings, () =>
            {
                //check whether webhook already exists
                var webhooks = HandleCoreRequest<WebhookListRequest, WebhookList>(settings, new WebhookListRequest());
                if (!string.IsNullOrEmpty(settings.WebhookId))
                {
                    var webhookById = webhooks?.Webhooks?.FirstOrDefault(webhook => webhook.Id.Equals(settings.WebhookId));
                    if (webhookById != null)
                        return webhookById;
                }
                var webhookByUrl = webhooks?.Webhooks?.FirstOrDefault(webhook => webhook.Url.Equals(url, StringComparison.InvariantCultureIgnoreCase));
                if (webhookByUrl != null)
                    return webhookByUrl;

                //or try to create the new one if doesn't exist
                var request = new WebhookCreateRequest().RequestBody(new Webhook
                {
                    EventTypes = Defaults.WebhookEventNames.Select(name => new EventType { Name = name }).ToList(),
                    Url = url
                });
                return HandleCoreRequest<WebhookCreateRequest, Webhook>(settings, request);
            });
        }

        /// <summary>
        /// Delete webhook
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        public void DeleteWebhook(PayPalSmartPaymentButtonsSettings settings)
        {
            HandleFunction(settings, () =>
            {
                var request = new WebhookDeleteRequest(settings.WebhookId);
                return HandleCoreRequest<WebhookDeleteRequest, object>(settings, request);
            });
        }

        /// <summary>
        /// Handle webhook request
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <param name="request">HTTP request</param>
        public void HandleWebhook(PayPalSmartPaymentButtonsSettings settings, Microsoft.AspNetCore.Http.HttpRequest request)
        {
            HandleFunction(settings, async () =>
            {
                //get request details
                var rawRequestString = string.Empty;
                using (var streamReader = new StreamReader(request.Body))
                    rawRequestString = await streamReader.ReadToEndAsync();

                //define a local function to validate the webhook event and get an appropriate resource
                object getWebhookResource<TResource>() where TResource : class
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
                        WebhookId = settings.WebhookId,
                        WebhookEvent = webhookEvent
                    });
                    var result = HandleCoreRequest<WebhookVerifySignatureRequest<TResource>, VerifyWebhookSignatureResponse>(settings, verifyRequest);

                    // This would be hard to always get it to success, as the result is dependent on time of webhook sent.
                    // As long as we get a 200 response, we should be fine.
                    //see details here https://github.com/paypal/PayPal-NET-SDK/commit/16e5bebfd4021d0888679e526cdf1f4f19527f3e#diff-ee79bcc68a8451d30522e1d9b2c5bc13R36
                    return result != null ? webhookEvent?.Resource : default;
                }

                //try to get webhook resource type
                var webhookResourceType = JsonConvert.DeserializeObject<Event<Order>>(rawRequestString).ResourceType?.ToLowerInvariant();

                //and now get specific webhook resource
                var webhookResource = webhookResourceType switch
                {
                    "checkout-order" => getWebhookResource<Order>(),
                    "authorization" => getWebhookResource<PayPalCheckoutSdk.Payments.Authorization>(),
                    "capture" => getWebhookResource<PayPalCheckoutSdk.Payments.Capture>(),
                    "refund" => getWebhookResource<PayPalCheckoutSdk.Payments.Refund>(),
                    _ => null
                };
                if (webhookResource == null)
                    return false;

                var orderReference = webhookResource is Order payPalOrder
                    ? payPalOrder.PurchaseUnits?.FirstOrDefault()?.CustomId
                    : JsonConvert.DeserializeObject<Event<ExtendedWebhookResource>>(rawRequestString).Resource?.CustomId;
                if (!Guid.TryParse(orderReference, out var orderGuid))
                    return false;

                var order = _orderService.GetOrderByGuid(orderGuid);
                if (order == null)
                    return false;

                _orderService.InsertOrderNote(new Core.Domain.Orders.OrderNote()
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
                                _orderService.UpdateOrder(order);
                                _orderProcessingService.MarkAsAuthorized(order);
                            }
                        }
                        break;

                    case "denied":
                    case "expired":
                    case "pending":
                        order.CaptureTransactionResult = authorization.Status;
                        order.OrderStatus = Core.Domain.Orders.OrderStatus.Pending;
                        _orderService.UpdateOrder(order);
                        _orderProcessingService.CheckOrderStatus(order);
                        break;

                    case "voided":
                        if (_orderProcessingService.CanVoidOffline(order))
                        {
                            order.AuthorizationTransactionId = authorization.Id;
                            order.AuthorizationTransactionResult = authorization.Status;
                            _orderService.UpdateOrder(order);
                            _orderProcessingService.VoidOffline(order);
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
                                _orderService.UpdateOrder(order);
                                _orderProcessingService.MarkOrderAsPaid(order);
                            }
                        }
                        break;

                    case "pending":
                    case "declined":
                        order.CaptureTransactionResult = $"{capture.Status}. {capture.StatusDetails?.Reason}";
                        order.OrderStatus = Core.Domain.Orders.OrderStatus.Pending;
                        _orderService.UpdateOrder(order);
                        _orderProcessingService.CheckOrderStatus(order);
                        break;

                    case "refunded":
                        if (_orderProcessingService.CanRefundOffline(order))
                            _orderProcessingService.RefundOffline(order);
                        break;
                }

                //refund actions
                var refund = webhookResource as PayPalCheckoutSdk.Payments.Refund;
                switch (refund?.Status?.ToLowerInvariant())
                {
                    case "completed":
                        var refundIds = _genericAttributeService.GetAttribute<List<string>>(order, Defaults.RefundIdAttributeName)
                            ?? new List<string>();
                        if (!refundIds.Contains(refund.Id))
                        {
                            if (decimal.TryParse(refund.Amount?.Value, out var refundedAmount))
                            {
                                if (_orderProcessingService.CanPartiallyRefundOffline(order, refundedAmount))
                                {
                                    _orderProcessingService.PartiallyRefundOffline(order, refundedAmount);
                                    refundIds.Add(refund.Id);
                                    _genericAttributeService.SaveAttribute(order, Defaults.RefundIdAttributeName, refundIds);
                                }
                            }
                        }
                        break;
                }

                return true;
            });
        }

        #endregion
    }
}