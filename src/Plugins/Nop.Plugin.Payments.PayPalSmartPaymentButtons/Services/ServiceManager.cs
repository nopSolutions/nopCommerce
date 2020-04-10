using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BraintreeHttp;
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
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
using PayPalHttpClient = PayPal.Core.PayPalHttpClient;

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
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProductService _productService;
        private readonly IShippingPluginManager _shippingPluginManager;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IWorkContext _workContext;
        private readonly CurrencySettings _currencySettings;
        private readonly PayPalSmartPaymentButtonsSettings _settings;

        private readonly PayPalHttpClient _client;

        #endregion

        #region Ctor

        public ServiceManager(IAddressService addresService,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICountryService countryService,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            ILogger logger,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPriceCalculationService priceCalculationService,
            IProductService productService,
            IShippingPluginManager shippingPluginManager,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            ITaxService taxService,
            IWorkContext workContext,
            CurrencySettings currencySettings,
            PayPalSmartPaymentButtonsSettings settings)
        {
            _addresService = addresService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _countryService = countryService;
            _currencyService = currencyService;
            _genericAttributeService = genericAttributeService;
            _logger = logger;
            _orderTotalCalculationService = orderTotalCalculationService;
            _priceCalculationService = priceCalculationService;
            _productService = productService;
            _shippingPluginManager = shippingPluginManager;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _taxService = taxService;
            _workContext = workContext;
            _currencySettings = currencySettings;
            _settings = settings;

            _client = _settings.UseSandbox
                ? new PayPalHttpClient(new PayPal.Core.SandboxEnvironment(_settings.ClientId, _settings.SecretKey))
                : new PayPalHttpClient(new PayPal.Core.LiveEnvironment(_settings.ClientId, _settings.SecretKey));
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether the plugin is configured
        /// </summary>
        /// <param name="settings">Settings; pass null to use default values</param>
        /// <returns>Result</returns>
        private bool IsConfigured(PayPalSmartPaymentButtonsSettings settings = null)
        {
            //client id and secret are required to request services
            return !string.IsNullOrEmpty((settings ?? _settings).ClientId) && !string.IsNullOrEmpty((settings ?? _settings).SecretKey);
        }

        /// <summary>
        /// Handle function and get result
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="function">Function</param>
        /// <param name="settings">Settings; pass null to use default values</param>
        /// <returns>Result; error message if exists</returns>
        private (TResult Result, string ErrorMessage) HandleFunction<TResult>(Func<TResult> function,
            PayPalSmartPaymentButtonsSettings settings = null)
        {
            try
            {
                //ensure that plugin is configured
                if (!IsConfigured(settings))
                    throw new NopException("Plugin not configured");

                //invoke function
                return (function(), default(string));
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
                        : !string.IsNullOrEmpty(details.Name)
                        ? details.Name
                        : message;
                }

                //log errors
                _logger.Error($"{Defaults.SystemName} error: {System.Environment.NewLine}{exception.Message}",
                    exception, _workContext.CurrentCustomer);

                return (default(TResult), message);
            }
        }

        /// <summary>
        /// Handle request and get response
        /// </summary>
        /// <typeparam name="TClient">Client type</typeparam>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResponseData">Response data type</typeparam>
        /// <param name="client">Client</param>
        /// <param name="request">Request</param>
        /// <param name="settings">Settings; pass null to use default values</param>
        /// <returns>Response; error message if exists</returns>
        private (TResponseData ResponseData, string ErrorMessage) HandleRequest<TClient, TRequest, TResponseData>(TClient client,
            TRequest request, PayPalSmartPaymentButtonsSettings settings = null) where TClient : HttpClient where TRequest : HttpRequest where TResponseData : class
        {
            return HandleFunction(() =>
            {
                //prepare common request params
                request.Headers.Add(HeaderNames.UserAgent, Defaults.UserAgent);
                request.Headers.Add("PayPal-Partner-Attribution-Id", Defaults.PartnerCode);
                request.Headers.Add("Prefer", "return=representation");
                //request.Headers.Add("PayPal-Client-Metadata-Id", );
                //request.Headers.Add("PayPal-Request-Id", );

                //execute request
                var response = client.Execute(request)
                    ?? throw new NopException("No response from the service.");

                //return the results if necessary
                if (typeof(TResponseData) == typeof(object))
                    return default(TResponseData);

                return response.Result?.Result<TResponseData>();
            }, settings);
        }

        /// <summary>
        /// Validate received webhook details
        /// </summary>
        /// <typeparam name="TResource">Webhook resource type</typeparam>
        /// <param name="webhookEvent">Webhook details</param>
        /// <param name="headers">Request headers</param>
        /// <param name="settings">Settings</param>
        /// <returns>Result</returns>
        private bool ValidateReceivedEvent<TResource>(Event<TResource> webhookEvent, Microsoft.AspNetCore.Http.IHeaderDictionary headers,
            PayPalSmartPaymentButtonsSettings settings) where TResource : class
        {
            //create a separate client to request services
            var client = settings.UseSandbox
                ? new PayPalHttpClient(new PayPal.Core.SandboxEnvironment(settings.ClientId, settings.SecretKey))
                : new PayPalHttpClient(new PayPal.Core.LiveEnvironment(settings.ClientId, settings.SecretKey));

            //verify webhook event data
            var verifyRequest = new WebhookVerifySignatureRequest<TResource>();
            verifyRequest.RequestBody(new VerifyWebhookSignature<TResource>
            {
                AuthAlgo = headers["PAYPAL-AUTH-ALGO"],
                CertUrl = headers["PAYPAL-CERT-URL"],
                TransmissionId = headers["PAYPAL-TRANSMISSION-ID"],
                TransmissionSig = headers["PAYPAL-TRANSMISSION-SIG"],
                TransmissionTime = headers["PAYPAL-TRANSMISSION-TIME"],
                WebhookId = settings.WebhookId,
                WebhookEvent = webhookEvent
            });
            var (response, errorMessage) = HandleRequest<PayPalHttpClient, WebhookVerifySignatureRequest<TResource>, VerifyWebhookSignatureResponse>(client, verifyRequest, settings);

            // This would be hard to always get it to success, as the result is dependent on time of webhook sent.
            // As long as we get a 200 response, we should be fine.
            //see details here https://github.com/paypal/PayPal-NET-SDK/commit/16e5bebfd4021d0888679e526cdf1f4f19527f3e#diff-ee79bcc68a8451d30522e1d9b2c5bc13R36
            return string.IsNullOrEmpty(errorMessage);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare URL to load the service js script
        /// </summary>
        /// <returns>URL</returns>
        public string GetScriptUrl()
        {
            var parameters = new Dictionary<string, string>
            {
                ["client-id"] = _settings.ClientId,
                ["currency"] = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)?.CurrencyCode,
                ["intent"] = _settings.PaymentType.ToString().ToLower(),
                ["commit"] = false.ToString().ToLower(),
                ["vault"] = false.ToString().ToLower(),
                ["debug"] = false.ToString().ToLower(),
                //["components"] = "buttons", //default value
                //["merchant-id"] = null, //not used
                //["integration-date"] = null, //YYYY-MM-DD
                //["buyer-country"] = null, //available in the sandbox only
                //["locale"] = null, //PayPal auto detects it
            };
            if (!string.IsNullOrEmpty(_settings.DisabledFunding))
                parameters["disable-funding"] = _settings.DisabledFunding;
            if (!string.IsNullOrEmpty(_settings.DisabledCards))
                parameters["disable-card"] = _settings.DisabledCards;

            return QueryHelpers.AddQueryString(Defaults.ServiceScriptUrl, parameters);
        }

        /// <summary>
        /// Create an order
        /// </summary>
        /// <param name="orderGuid">Order GUID</param>
        /// <returns>Created order; error message if exists</returns>
        public (Order order, string errorMessage) CreateOrder(Guid orderGuid)
        {
            var currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)?.CurrencyCode;
            if (string.IsNullOrEmpty(currency))
                throw new NopException("Primary store currency not found");

            var billingAddress = _addresService.GetAddressById(_workContext.CurrentCustomer.BillingAddressId ?? 0);
            if (billingAddress == null)
                throw new NopException("Billing address not set");

            var shippingAddress = _addresService.GetAddressById(_workContext.CurrentCustomer.ShippingAddressId ?? 0);

            var billStateProvince = _stateProvinceService.GetStateProvinceByAddress(billingAddress);
            var shipStateProvince = _stateProvinceService.GetStateProvinceByAddress(shippingAddress);

            //prepare order details
            var orderDetails = new OrderRequest { CheckoutPaymentIntent = _settings.PaymentType.ToString().ToUpper() };

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
            var shippingPlugins = _shippingPluginManager.LoadActivePlugins(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
            var shoppingCart = _shoppingCartService
                .GetShoppingCart(_workContext.CurrentCustomer, Core.Domain.Orders.ShoppingCartType.ShoppingCart).ToList();
            var taxTotal = Math.Round(_orderTotalCalculationService.GetTaxTotal(shoppingCart, shippingPlugins, false), 2);
            var shippingTotal = Math.Round(_orderTotalCalculationService.GetShoppingCartShippingTotal(shoppingCart, shippingPlugins) ?? decimal.Zero, 2);
            var orderTotal = Math.Round(_orderTotalCalculationService.GetShoppingCartTotal(shoppingCart, false, false) ?? decimal.Zero, 2);

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
                    UnitAmount = new PayPalCheckoutSdk.Orders.Money { CurrencyCode = currency, Value = itemPrice.ToString("F") }
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
                        UnitAmount = new PayPalCheckoutSdk.Orders.Money { CurrencyCode = currency, Value = roundedAttributePrice.ToString("F") }
                    });
                }
            }

            //set totals
            itemTotal = Math.Round(itemTotal, 2);
            var discountTotal = Math.Round(itemTotal + taxTotal + shippingTotal - orderTotal, 2);
            purchaseUnit.AmountWithBreakdown = new AmountWithBreakdown
            {
                CurrencyCode = currency,
                Value = orderTotal.ToString("F"),
                AmountBreakdown = new AmountBreakdown
                {
                    ItemTotal = new PayPalCheckoutSdk.Orders.Money { CurrencyCode = currency, Value = itemTotal.ToString("F") },
                    TaxTotal = new PayPalCheckoutSdk.Orders.Money { CurrencyCode = currency, Value = taxTotal.ToString("F") },
                    Shipping = new PayPalCheckoutSdk.Orders.Money { CurrencyCode = currency, Value = shippingTotal.ToString("F") },
                    Discount = new PayPalCheckoutSdk.Orders.Money { CurrencyCode = currency, Value = discountTotal.ToString("F") }
                }
            };

            orderDetails.PurchaseUnits = new List<PurchaseUnitRequest> { purchaseUnit };

            return HandleRequest<HttpClient, OrdersCreateRequest, Order>(_client, new OrdersCreateRequest { Body = orderDetails });
        }

        /// <summary>
        /// Authorize a previously created order
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <returns>Authorized order; error message if exists</returns>
        public (Order order, string errorMessage) Authorize(string orderId)
        {
            var request = new OrdersAuthorizeRequest(orderId);
            request.RequestBody(new AuthorizeRequest());
            return HandleRequest<PayPalHttpClient, OrdersAuthorizeRequest, Order>(_client, request);
        }

        /// <summary>
        /// Capture a previously created order
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <returns>Captured order; error message if exists</returns>
        public (Order order, string errorMessage) Capture(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());
            return HandleRequest<PayPalHttpClient, OrdersCaptureRequest, Order>(_client, request);
        }

        /// <summary>
        /// Capture an authorization
        /// </summary>
        /// <param name="authorizationId">Authorization id</param>
        /// <returns>Capture details; error message if exists</returns>
        public (PayPalCheckoutSdk.Payments.Capture capture, string errorMessage) CaptureAuthorization(string authorizationId)
        {
            var request = new AuthorizationsCaptureRequest(authorizationId);
            request.RequestBody(new CaptureRequest());
            return HandleRequest<PayPalHttpClient, AuthorizationsCaptureRequest, PayPalCheckoutSdk.Payments.Capture>(_client, request);
        }

        /// <summary>
        /// Void an authorization
        /// </summary>
        /// <param name="authorizationId">Authorization id</param>
        /// <returns>Error message if exists</returns>
        public string Void(string authorizationId)
        {
            var request = new AuthorizationsVoidRequest(authorizationId);
            var (response, errorMessage) = HandleRequest<PayPalHttpClient, AuthorizationsVoidRequest, object>(_client, request);
            return errorMessage;
        }

        /// <summary>
        /// Refund a captured payment
        /// </summary>
        /// <param name="captureId">Capture id</param>
        /// <param name="currency">Currency code</param>
        /// <param name="amount">Amount to refund</param>
        /// <returns>Refund details; error message if exists</returns>
        public (PayPalCheckoutSdk.Payments.Refund capture, string errorMessage) Refund(string captureId, string currency, decimal? amount = null)
        {
            var request = new CapturesRefundRequest(captureId);
            var refundRequest = new RefundRequest();
            if (amount.HasValue)
                refundRequest.Amount = new PayPalCheckoutSdk.Payments.Money { CurrencyCode = currency, Value = amount.Value.ToString("F") };
            request.RequestBody(refundRequest);
            return HandleRequest<PayPalHttpClient, CapturesRefundRequest, PayPalCheckoutSdk.Payments.Refund>(_client, request);
        }

        /// <summary>
        /// Check passed credentials 
        /// </summary>
        /// <param name="settings">Settings</param>
        /// <returns>Error message if exists</returns>
        public string CheckCredentials(PayPalSmartPaymentButtonsSettings settings)
        {
            //create a separate client to request services
            var client = settings.UseSandbox
                ? new PayPal.Core.PayPalHttpClient(new PayPal.Core.SandboxEnvironment(settings.ClientId, settings.SecretKey))
                : new PayPal.Core.PayPalHttpClient(new PayPal.Core.LiveEnvironment(settings.ClientId, settings.SecretKey));

            //use a random get request only to check credentials validity
            var (eventTypes, errorMessage) = HandleRequest<PayPal.Core.PayPalHttpClient, AvailableEventTypeListRequest, EventTypeList>
                (client, new AvailableEventTypeListRequest(), settings);
            return errorMessage;
        }

        /// <summary>
        /// Create webhook that receive events for the subscribed event types
        /// </summary>
        /// <param name="settings">Settings</param>
        /// <param name="url">Webhook listener URL</param>
        /// <returns>Webhook; error message if exists</returns>
        public (Webhook webhook, string errorMessage) CreateWebHook(PayPalSmartPaymentButtonsSettings settings, string url)
        {
            //create a separate client to request services
            var client = settings.UseSandbox
                ? new PayPal.Core.PayPalHttpClient(new PayPal.Core.SandboxEnvironment(settings.ClientId, settings.SecretKey))
                : new PayPal.Core.PayPalHttpClient(new PayPal.Core.LiveEnvironment(settings.ClientId, settings.SecretKey));

            //check whether webhook already exists
            var listRequest = new WebhookListRequest();
            var (webhooks, getError) = HandleRequest<PayPal.Core.PayPalHttpClient, WebhookListRequest, WebhookList>(client, listRequest, settings);

            if (!string.IsNullOrEmpty(settings.WebhookId))
            {
                var webhookById = webhooks?.Webhooks?.FirstOrDefault(webhook => webhook.Id.Equals(settings.WebhookId));
                if (webhookById != null)
                    return (webhookById, getError);
            }

            var webhookByUrl = webhooks?.Webhooks?.FirstOrDefault(webhook => webhook.Url.Equals(url, StringComparison.InvariantCultureIgnoreCase));
            if (webhookByUrl != null)
                return (webhookByUrl, getError);

            //or try to create the new one if doesn't exist
            var createRequest = new WebhookCreateRequest();
            createRequest.RequestBody(new Webhook
            {
                EventTypes = Defaults.WebhookEventNames.Select(name => new EventType { Name = name }).ToList(),
                Url = url
            });
            return HandleRequest<PayPalHttpClient, WebhookCreateRequest, Webhook>(client, createRequest, settings);
        }

        /// <summary>
        /// Delete webhook
        /// </summary>
        /// <param name="settings">Settings</param>
        public void DeleteWebhook(PayPalSmartPaymentButtonsSettings settings)
        {
            //create a separate client to request services
            var client = settings.UseSandbox
                ? new PayPal.Core.PayPalHttpClient(new PayPal.Core.SandboxEnvironment(settings.ClientId, settings.SecretKey))
                : new PayPal.Core.PayPalHttpClient(new PayPal.Core.LiveEnvironment(settings.ClientId, settings.SecretKey));

            //try to delete webhook
            var request = new WebhookDeleteRequest(settings.WebhookId);
            HandleRequest<PayPal.Core.PayPalHttpClient, WebhookDeleteRequest, object>(client, request, settings);
        }

        /// <summary>
        /// Get details from the webhook request
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <param name="settings">Settings</param>
        /// <returns>Details; raw request data</returns>
        public (object details, string rawRequestString) GetWebhookRequestDetails(Microsoft.AspNetCore.Http.HttpRequest request,
            PayPalSmartPaymentButtonsSettings settings)
        {
            var (result, errorMessage) = HandleFunction(() =>
            {
                var rawRequestString = string.Empty;
                using (var streamReader = new StreamReader(request.Body))
                    rawRequestString = streamReader.ReadToEnd();

                //try to get generic webhook details
                object details = null;
                var webhookEvent = JsonConvert.DeserializeObject<Event<Order>>(rawRequestString);

                //and now get specific webhook resource
                switch (webhookEvent.ResourceType?.ToLowerInvariant())
                {
                    //order event, so just log some info
                    case "checkout-order":
                        //first, validate request
                        var orderWebhook = JsonConvert.DeserializeObject<Event<Order>>(rawRequestString);
                        if (!ValidateReceivedEvent(orderWebhook, request.Headers, settings))
                            break;

                        //log info
                        var orderTotal = orderWebhook?.Resource?.PurchaseUnits?.FirstOrDefault()?.AmountWithBreakdown;
                        if (orderTotal != null)
                            _logger.Information($"An order for {orderTotal.Value} {orderTotal.CurrencyCode} has been approved by buyer");
                        break;

                    //payment authorization
                    case "authorization":
                        //first, validate request
                        var authorizationWebhook = JsonConvert.DeserializeObject<Event<ExtendedAuthorization>>(rawRequestString);
                        if (!ValidateReceivedEvent(authorizationWebhook, request.Headers, settings))
                            break;

                        //request is valid, so set details
                        details = authorizationWebhook?.Resource;
                        break;

                    //payment capture
                    case "capture":
                        //first, validate request
                        var captureWebhook = JsonConvert.DeserializeObject<Event<ExtendedCapture>>(rawRequestString);
                        if (!ValidateReceivedEvent(captureWebhook, request.Headers, settings))
                            break;

                        //request is valid, so set details
                        details = captureWebhook?.Resource;
                        break;

                    //payment refund
                    case "refund":
                        //first, validate request
                        var refundWebhook = JsonConvert.DeserializeObject<Event<PayPalCheckoutSdk.Payments.Refund>>(rawRequestString);
                        if (!ValidateReceivedEvent(refundWebhook, request.Headers, settings))
                            break;

                        //request is valid, so set details
                        details = refundWebhook?.Resource;
                        break;
                }

                return (details, rawRequestString);
            }, settings);

            return result;
        }

        #endregion
    }
}