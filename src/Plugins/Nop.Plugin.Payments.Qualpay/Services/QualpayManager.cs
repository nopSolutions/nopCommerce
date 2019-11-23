using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.Qualpay.Domain;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using qpPg.Api;
using qpPg.Model;
using qpPlatform.Api;
using qpPlatform.Model;

namespace Nop.Plugin.Payments.Qualpay.Services
{
    /// <summary>
    /// Represents Qualpay service manager
    /// </summary>
    public class QualpayManager
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IEmailSender _emailSender;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentService _paymentService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProductService _productService;
        private readonly IShippingPluginManager _shippingPluginManager;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITaxService _taxService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IWorkContext _workContext;
        private readonly QualpaySettings _qualpaySettings;

        private readonly IDictionary<string, qpPlatform.Client.IApiAccessor> _platformApiClients;
        private readonly IDictionary<string, qpPg.Client.IApiAccessor> _pgApiClients;

        #endregion

        #region Ctor

        public QualpayManager(CurrencySettings currencySettings,
            EmailAccountSettings emailAccountSettings,
            IActionContextAccessor actionContextAccessor,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IEmailAccountService emailAccountService,
            IEmailSender emailSender,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentService paymentService,
            IPriceCalculationService priceCalculationService,
            IProductService productService,
            IShippingPluginManager shippingPluginManager,
            IShoppingCartService shoppingCartService,
            ITaxService taxService,
            IUrlHelperFactory urlHelperFactory,
            IWorkContext workContext,
            QualpaySettings qualpaySettings)
        {
            _currencySettings = currencySettings;
            _emailAccountSettings = emailAccountSettings;
            _actionContextAccessor = actionContextAccessor;
            _checkoutAttributeParser = checkoutAttributeParser;
            _currencyService = currencyService;
            _customerService = customerService;
            _emailAccountService = emailAccountService;
            _emailSender = emailSender;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _orderTotalCalculationService = orderTotalCalculationService;
            _paymentService = paymentService;
            _priceCalculationService = priceCalculationService;
            _productService = productService;
            _shippingPluginManager = shippingPluginManager;
            _shoppingCartService = shoppingCartService;
            _taxService = taxService;
            _urlHelperFactory = urlHelperFactory;
            _workContext = workContext;
            _qualpaySettings = qualpaySettings;

            _platformApiClients = new Dictionary<string, qpPlatform.Client.IApiAccessor>();
            _pgApiClients = new Dictionary<string, qpPg.Client.IApiAccessor>();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Invoke function and get result
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="function">Function</param>
        /// <returns>Result; error message if exists</returns>
        private (TResult Result, string ErrorMessage) InvokeFunction<TResult>(Func<TResult> function)
        {
            try
            {
                //ensure that plugin is configured
                if (string.IsNullOrEmpty(_qualpaySettings.MerchantId) || !long.TryParse(_qualpaySettings.MerchantId, out var merchantId))
                    throw new NopException("Plugin not configured");

                //process request action
                return (function(), null);
            }
            catch (Exception exception)
            {
                //log errors
                var errorMessage = $"Qualpay error: {exception.Message}";
                _logger.Error(errorMessage, exception, _workContext.CurrentCustomer);

                return (default(TResult), errorMessage);
            }
        }

        /// <summary>
        /// Get base API URL
        /// </summary>
        /// <returns>URL</returns>
        private string GetBaseApiUrl()
        {
            return _qualpaySettings.UseSandbox ? "https://api-test.qualpay.com/" : "https://api.qualpay.com/";
        }

        /// <summary>
        /// Prepare platform API client
        /// </summary>
        /// <typeparam name="TApiClient">API client type</typeparam>
        /// <param name="clientCtor">Function to create client instance</param>
        /// <returns>API client</returns>
        private TApiClient CreatePlatformApiClient<TApiClient>(Func<qpPlatform.Client.Configuration, TApiClient> clientCtor)
            where TApiClient : class, qpPlatform.Client.IApiAccessor
        {
            var clientTypeName = typeof(TApiClient).Name;
            if (!_platformApiClients.ContainsKey(clientTypeName))
            {
                _platformApiClients.Add(clientTypeName, clientCtor(new qpPlatform.Client.Configuration
                {
                    BasePath = $"{GetBaseApiUrl()}platform/",
                    Password = _qualpaySettings.SecurityKey,
                    UserAgent = QualpayDefaults.UserAgent
                }));
            }

            return _platformApiClients[clientTypeName] as TApiClient;
        }

        /// <summary>
        /// Prepare payment gateway API client
        /// </summary>
        /// <typeparam name="TApiClient">API client type</typeparam>
        /// <param name="clientCtor">Function to create client instance</param>
        /// <returns>API client</returns>
        private TApiClient CreatePgApiClient<TApiClient>(Func<qpPg.Client.Configuration, TApiClient> clientCtor)
            where TApiClient : class, qpPg.Client.IApiAccessor
        {
            var clientTypeName = typeof(TApiClient).Name;
            if (!_pgApiClients.ContainsKey(clientTypeName))
            {
                _pgApiClients.Add(clientTypeName, clientCtor(new qpPg.Client.Configuration
                {
                    BasePath = $"{GetBaseApiUrl()}pg/",
                    Password = _qualpaySettings.SecurityKey,
                    UserAgent = QualpayDefaults.UserAgent
                }));
            }

            return _pgApiClients[clientTypeName] as TApiClient;
        }

        /// <summary>
        /// Validate whether webhook request is initiated by Qualpay and return received data details if is valid
        /// </summary>
        /// <typeparam name="TData">Data details type</typeparam>
        /// <param name="request">Request</param>
        /// <returns>Validation result; received data details; error message id exists</returns>
        private (bool Valid, WebhookEvent<TData> Data, string ErrorMessage) ValidateWebhook<TData>(HttpRequest request)
            where TData : class
        {
            var (webhookData, errorMessage) = InvokeFunction(() =>
            {
                //try to get request message
                var message = string.Empty;
                using (var streamReader = new StreamReader(request.Body, Encoding.UTF8))
                    message = streamReader.ReadToEnd();

                if (string.IsNullOrEmpty(message))
                    throw new NopException("Webhook request is empty");

                //ensure that request is signed using a signature header
                if (!request.Headers.TryGetValue(QualpayDefaults.WebhookSignatureHeaderName, out var signatures))
                    throw new NopException("Webhook request not signed by a signature header");

                //get encrypted string from the request message
                var encryptedString = string.Empty;
                using (var hashAlgorithm = new HMACSHA256(Encoding.UTF8.GetBytes(_qualpaySettings.WebhookSecretKey)))
                    encryptedString = Convert.ToBase64String(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(message)));

                //equal this encrypted string with received signatures
                if (!signatures.Any(signature => signature.Equals(encryptedString)))
                    throw new NopException("Webhook request isn't valid");

                //request is valid, so log received message
                _logger.Information($"Qualpay Webhook. Webhook request is received: {message}");

                //and try to get data details from webhook message
                var webhookEvent = JsonConvert.DeserializeObject<WebhookEvent<TData>>(message);
                if (webhookEvent?.Data is TData data)
                    return webhookEvent;

                return null;
            });

            if (webhookData == null && string.IsNullOrEmpty(errorMessage))
                return (true, webhookData, "Webhook data is empty");

            if (webhookData == null && !string.IsNullOrEmpty(errorMessage))
                return (false, webhookData, errorMessage);

            return (true, webhookData, errorMessage);
        }

        /// <summary>
        /// Send email about new subscription/unsubscription
        /// </summary>
        /// <param name="email">From email address</param>
        /// <param name="subscribe">Whether to subscribe the specified email</param>
        private void SendEmail(string email, bool subscribe)
        {
            //try to get an email account
            var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId)
                ?? throw new NopException("Email account could not be loaded");

            var subject = subscribe ? "New subscription" : "New unsubscription";
            var body = subscribe
                ? "nopCommerce user just left the email to receive an information about special offers from Qualpay."
                : "nopCommerce user has canceled subscription to receive Qualpay news.";

            //send email
            _emailSender.SendEmail(emailAccount: emailAccount,
                subject: subject, body: body,
                fromAddress: email, fromName: QualpayDefaults.UserAgent,
                toAddress: QualpayDefaults.SubscriptionEmail, toName: null);
        }

        #region Requests preparation

        /// <summary>
        /// Get transaction line items
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="orderTotal">Order total</param>
        /// <returns>List of transaction items; items tax amount</returns>
        private (IList<Domain.LineItem> Items, decimal TaxAmount) GetItems(Core.Domain.Customers.Customer customer, int storeId, decimal orderTotal)
        {
            var items = new List<Domain.LineItem>();

            //get current shopping cart            
            var shoppingCart = _shoppingCartService.GetShoppingCart(customer, ShoppingCartType.ShoppingCart, storeId);

            //get tax amount
            var shippingRateComputationMethods = _shippingPluginManager.LoadActivePlugins(customer, storeId);
            var taxAmount = _orderTotalCalculationService.GetTaxTotal(shoppingCart, shippingRateComputationMethods);

            //define function to create item
            Domain.LineItem createItem(decimal price, string description, string productCode, int quantity = 1)
            {
                return new Domain.LineItem
                {
                    CreditType = "D",
                    Description = CommonHelper.EnsureMaximumLength(description, 25),
                    MeasureUnit = "*",
                    ProductCode = CommonHelper.EnsureMaximumLength(productCode, 12),
                    Quantity = quantity,
                    UnitPrice = Convert.ToDouble(price)
                };
            }

            //create transaction items from shopping cart items
            items.AddRange(shoppingCart.Where(shoppingCartItem => shoppingCartItem.Product != null).Select(shoppingCartItem =>
            {
                //item price
                var price = _taxService.GetProductPrice(shoppingCartItem.Product,
                    _priceCalculationService.GetUnitPrice(shoppingCartItem), false, shoppingCartItem.Customer, out _);

                return createItem(price, shoppingCartItem.Product.Name,
                    _productService.FormatSku(shoppingCartItem.Product, shoppingCartItem.AttributesXml),
                    shoppingCartItem.Quantity);
            }));

            //create transaction items from checkout attributes
            var checkoutAttributesXml = _genericAttributeService
                .GetAttribute<string>(customer, NopCustomerDefaults.CheckoutAttributes, storeId);

            if (!string.IsNullOrEmpty(checkoutAttributesXml))
            {
                var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttributesXml);
                items.AddRange(attributeValues.Where(attributeValue => attributeValue.CheckoutAttribute != null).Select(attributeValue =>
                {
                    return createItem(_taxService.GetCheckoutAttributePrice(attributeValue, false, customer),
                        $"{attributeValue.CheckoutAttribute.Name} ({attributeValue.Name})", "checkout");
                }));
            }

            //create transaction item for payment method additional fee
            var paymentAdditionalFee = _paymentService.GetAdditionalHandlingFee(shoppingCart, QualpayDefaults.SystemName);
            var paymentPrice = _taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, false, customer);
            if (paymentPrice > decimal.Zero)
                items.Add(createItem(paymentPrice, $"Payment ({QualpayDefaults.SystemName})", "payment"));

            //create transaction item for shipping rate
            if (_shoppingCartService.ShoppingCartRequiresShipping(shoppingCart))
            {
                var shippingPrice = _orderTotalCalculationService
                    .GetShoppingCartShippingTotal(shoppingCart, false, shippingRateComputationMethods);
                if (shippingPrice.HasValue && shippingPrice.Value > decimal.Zero)
                    items.Add(createItem(shippingPrice.Value, "Shipping rate", "shipping"));
            }

            //create transaction item for all discounts
            var amountDifference = orderTotal - Convert.ToDecimal(items.Sum(lineItem => lineItem.UnitPrice * lineItem.Quantity).Value) - taxAmount;
            if (amountDifference < decimal.Zero)
                items.Add(createItem(amountDifference, "Discount amount", "discounts"));

            return (items, taxAmount);
        }

        /// <summary>
        /// Get request parameters to create a customer in Vault
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Request parameters to create customer</returns>
        private AddCustomerRequest CreateCustomerRequest(Core.Domain.Customers.Customer customer)
        {
            return new AddCustomerRequest
            (
                customerId: customer.Id.ToString(),
                customerEmail: customer.Email,
                customerFirstName: _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute),
                customerLastName: _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute),
                customerFirmName: _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute),
                customerPhone: _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute),
                shippingAddresses: customer.ShippingAddress == null ? null : new List<AddShippingAddressRequest>
                {
                    new AddShippingAddressRequest
                    (
                        primary: true,
                        shippingFirstName: customer.ShippingAddress.FirstName,
                        shippingLastName: customer.ShippingAddress.LastName,
                        shippingAddr1: customer.ShippingAddress?.Address1,
                        shippingAddr2: customer.ShippingAddress.Address2,
                        shippingCity: customer.ShippingAddress?.City,
                        shippingState: customer.ShippingAddress?.StateProvince?.Abbreviation,
                        shippingCountryCode: customer.ShippingAddress?.Country?.ThreeLetterIsoCode,
                        shippingZip: customer.ShippingAddress?.ZipPostalCode,
                        shippingFirmName: customer.ShippingAddress?.Company
                    )
                }
            );
        }

        /// <summary>
        /// Get frequency, subscription cycle interval and start date of recurring payment 
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Frequency type, cycle interval, start date</returns>
        private (AddSubscriptionRequest.PlanFrequencyEnum? frequency, int? interval, DateTime startDate) GetSubscriptionParameters(ProcessPaymentRequest processPaymentRequest)
        {
            switch (processPaymentRequest.RecurringCyclePeriod)
            {
                case RecurringProductCyclePeriod.Days:
                    if (processPaymentRequest.RecurringCycleLength % 30 == 0 || processPaymentRequest.RecurringCycleLength % 31 == 0)
                    {
                        return (AddSubscriptionRequest.PlanFrequencyEnum.NUMBER_3, processPaymentRequest.RecurringCycleLength / 30,
                            DateTime.UtcNow.AddDays(processPaymentRequest.RecurringCycleLength));
                    }

                    if (processPaymentRequest.RecurringCycleLength < 7)
                        throw new NopException("Qualpay Payment Gateway error: Recurring Billing supports payments with the minimum frequency of once a week");

                    return (AddSubscriptionRequest.PlanFrequencyEnum.NUMBER_0, processPaymentRequest.RecurringCycleLength / 7,
                        DateTime.UtcNow.AddDays(processPaymentRequest.RecurringCycleLength));

                case RecurringProductCyclePeriod.Weeks:
                    return (AddSubscriptionRequest.PlanFrequencyEnum.NUMBER_0, processPaymentRequest.RecurringCycleLength,
                        DateTime.UtcNow.AddDays(processPaymentRequest.RecurringCycleLength * 7));

                case RecurringProductCyclePeriod.Months:
                    if (processPaymentRequest.RecurringCycleLength == 12)
                        return (AddSubscriptionRequest.PlanFrequencyEnum.NUMBER_6, null, DateTime.UtcNow.AddYears(1));

                    return (AddSubscriptionRequest.PlanFrequencyEnum.NUMBER_3, processPaymentRequest.RecurringCycleLength,
                        DateTime.UtcNow.AddMonths(processPaymentRequest.RecurringCycleLength));

                case RecurringProductCyclePeriod.Years:
                    if (processPaymentRequest.RecurringCycleLength == 1)
                        return (AddSubscriptionRequest.PlanFrequencyEnum.NUMBER_6, null, DateTime.UtcNow.AddYears(1));

                    return (AddSubscriptionRequest.PlanFrequencyEnum.NUMBER_3, processPaymentRequest.RecurringCycleLength * 12,
                        DateTime.UtcNow.AddYears(processPaymentRequest.RecurringCycleLength));
            }

            return (null, null, DateTime.UtcNow);
        }

        /// <summary>
        /// Create request parameters to create subscription for recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Request parameters</returns>
        private AddSubscriptionRequest CreateSubscriptionRequest(ProcessPaymentRequest processPaymentRequest)
        {
            //whether Recurring Billing is enabled
            if (!_qualpaySettings.UseRecurringBilling)
                throw new NopException("Recurring payments are not available");

            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId)
                ?? throw new NopException("Customer cannot be loaded");

            //Recurring Billing is available only for registered customers
            if (customer.IsGuest())
                throw new NopException("Recurring payments are available only for registered customers");

            //Qualpay Payment Gateway supports only USD currency
            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (!primaryStoreCurrency.CurrencyCode.Equals("USD", StringComparison.InvariantCultureIgnoreCase))
                throw new NopException("USD is not primary store currency");

            //ensure that customer exists in Vault (recurring billing is available only for stored customers)
            var vaultCustomer = GetCustomer(customer.Id) ?? CreateCustomer(customer)
                ?? throw new NopException("Qualpay Payment Gateway error: Failed to create recurring payment.");

            var (frequency, interval, startDate) = GetSubscriptionParameters(processPaymentRequest);
            var subscriptionRequest = new AddSubscriptionRequest
            (
                profileId: _qualpaySettings.ProfileId,
                amtTran: Convert.ToDouble(Math.Round(processPaymentRequest.OrderTotal, 2)),
                tranCurrency: primaryStoreCurrency.CurrencyCode,
                customerId: customer.Id.ToString(),
                planDesc: processPaymentRequest.OrderGuid.ToString(),
                planDuration: processPaymentRequest.RecurringTotalCycles - 1,
                amtSetup: Convert.ToDouble(Math.Round(processPaymentRequest.OrderTotal, 2)),
                planFrequency: frequency,
                interval: interval,
                dateStart: startDate.ToString("yyyy-MM-dd")
            );
            
            //whether the customer has chosen a previously saved card
            var selectedCard = GetPreviouslySavedBillingCard(processPaymentRequest, customer);
            if (selectedCard != null)
            {
                //ensure that the selected card is default card
                if (!selectedCard.Primary ?? true)
                {
                    var updated = UpdateCustomerCard(customer.Id, new UpdateBillingCardRequest
                    (
                        cardId: selectedCard.CardId,
                        primary: true,
                        billingAddr1: customer.BillingAddress?.Address1,
                        billingZip: customer.BillingAddress?.ZipPostalCode
                    ));
                    if (!updated)
                        throw new NopException("Qualpay Payment Gateway error: Failed to pay by the selected card.");
                }

                return subscriptionRequest;
            }

            //get card identifier
            var tokenizedCardId = GetTokenizedCardId(processPaymentRequest, customer);
            if (string.IsNullOrEmpty(tokenizedCardId))
                throw new NopException("Qualpay Payment Gateway error: Failed to pay by the selected card.");

            //add tokenized billing card to customer
            var created = CreateCustomerCard(customer.Id, new AddBillingCardRequest
            (
                verify: true,
                primary: true,
                cardId: tokenizedCardId,
                billingAddr1: customer.BillingAddress?.Address1,
                billingZip: customer.BillingAddress?.ZipPostalCode
            ));
            if (!created)
                throw new NopException("Qualpay Payment Gateway error: Failed to pay by the selected card.");

            return subscriptionRequest;
        }

        /// <summary>
        /// Create request parameters to authorize or sale
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Request parameters</returns>
        private PGApiTransactionRequest CreateTransactionRequest(ProcessPaymentRequest processPaymentRequest)
        {
            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId)
                ?? throw new NopException("Customer cannot be loaded");

            //Qualpay Payment Gateway supports only USD currency
            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (!primaryStoreCurrency.CurrencyCode.Equals("USD", StringComparison.InvariantCultureIgnoreCase))
                throw new NopException("USD is not a primary store currency");

            var (items, taxAmount) = GetItems(customer, processPaymentRequest.StoreId, processPaymentRequest.OrderTotal);
            var transactionRequest = new PGApiTransactionRequest
            (
                merchantId: long.Parse(_qualpaySettings.MerchantId),
                developerId: $"{QualpayDefaults.DeveloperId}-{NopVersion.CurrentVersion}",
                purchaseId: CommonHelper.EnsureMaximumLength(processPaymentRequest.OrderGuid.ToString(), 25),
                amtTran: Convert.ToDouble(Math.Round(processPaymentRequest.OrderTotal, 2)),
                tranCurrency: QualpayDefaults.UsdNumericIsoCode,
                emailReceipt: !string.IsNullOrEmpty(customer.BillingAddress?.Email),
                customerEmail: customer.BillingAddress?.Email,
                lineItems: JsonConvert.SerializeObject(items),
                amtTax: Convert.ToDouble(Math.Round(taxAmount, 2))
            );

            //whether the customer has chosen a previously saved card
            var selectedCard = GetPreviouslySavedBillingCard(processPaymentRequest, customer);
            if (selectedCard != null)
            {
                //card exists, set it to the request parameters
                transactionRequest.CardId = selectedCard.CardId;
                transactionRequest.CustomerId = customer.Id.ToString();

                return transactionRequest;
            }

            //set card identifier to the request parameters
            transactionRequest.CardId = GetTokenizedCardId(processPaymentRequest, customer);

            //whether the customer has chosen to save card details for the future using
            var saveCardKey = _localizationService.GetResource("Plugins.Payments.Qualpay.Customer.Card.Save");
            if (!processPaymentRequest.CustomValues.ContainsKey(saveCardKey))
                return transactionRequest;

            //remove the value from payment custom values, since it is no longer needed
            processPaymentRequest.CustomValues.Remove(saveCardKey);

            //check whether customer is already exists in the Vault and try to create new one if does not exist
            var vaultCustomer = GetCustomer(customer.Id) ?? CreateCustomer(customer);

            if (vaultCustomer == null || string.IsNullOrEmpty(transactionRequest.CardId))
                return transactionRequest;

            //customer exists, thus add tokenized billing card to customer
            CreateCustomerCard(customer.Id, new AddBillingCardRequest
            (
                verify: true,
                cardId: transactionRequest.CardId,
                billingAddr1: customer.BillingAddress?.Address1,
                billingZip: customer.BillingAddress?.ZipPostalCode
            ));

            return transactionRequest;
        }

        /// <summary>
        /// Get selected by customer previously saved billing card
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <returns>Billing card</returns>
        private BillingCard GetPreviouslySavedBillingCard(ProcessPaymentRequest processPaymentRequest, Core.Domain.Customers.Customer customer)
        {
            var cardIdKey = _localizationService.GetResource("Plugins.Payments.Qualpay.Customer.Card");
            if (!processPaymentRequest.CustomValues.TryGetValue(cardIdKey, out var cardId))
                return null;

            //remove the value from payment custom values, since it is no longer needed
            processPaymentRequest.CustomValues.Remove(cardIdKey);

            //ensure that customer exists in Vault and has this card
            var selectedCard = GetCustomerCards(customer.Id)
                .FirstOrDefault(card => card?.CardId?.Equals(cardId.ToString()) ?? false)
                ?? throw new NopException("Qualpay Payment Gateway error: Failed to pay by the selected card.");

            return selectedCard;
        }

        /// <summary>
        /// Get tokenized card identifier
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <returns>Card identifier</returns>
        private string GetTokenizedCardId(ProcessPaymentRequest processPaymentRequest, Core.Domain.Customers.Customer customer)
        {
            var cardId = string.Empty;

            if (_qualpaySettings.UseEmbeddedFields)
            {
                //tokenized card identifier has already been received from Qualpay Embedded Fields 
                var tokenizedCardIdKey = _localizationService.GetResource("Plugins.Payments.Qualpay.Customer.Card.Token");
                if (processPaymentRequest.CustomValues.TryGetValue(tokenizedCardIdKey, out var tokenizedCardId))
                    cardId = tokenizedCardId.ToString();

                //remove the value from payment custom values, since it is no longer needed
                processPaymentRequest.CustomValues.Remove(tokenizedCardIdKey);
            }
            else
            {
                //or try to tokenize card data now
                cardId = TokenizeCard(new PGApiTokenizeRequest
                (
                    merchantId: long.Parse(_qualpaySettings.MerchantId),
                    developerId: $"{QualpayDefaults.DeveloperId}-{NopVersion.CurrentVersion}",
                    singleUse: true,
                    cardholderName: processPaymentRequest.CreditCardName,
                    cardNumber: processPaymentRequest.CreditCardNumber,
                    cvv2: processPaymentRequest.CreditCardCvv2,
                    expDate: $"{processPaymentRequest.CreditCardExpireMonth:D2}{processPaymentRequest.CreditCardExpireYear.ToString().Substring(2)}",
                    avsAddress: CommonHelper.EnsureMaximumLength(customer.BillingAddress?.Address1, 20),
                    avsZip: customer.BillingAddress?.ZipPostalCode
                ));
            }

            return cardId;
        }

        /// <summary>
        /// Tokenize card data
        /// </summary>
        /// <param name="request">Request parameters to tokenize card</param>
        /// <returns>Card identifier</returns>
        private string TokenizeCard(PGApiTokenizeRequest request)
        {
            return InvokeFunction(() =>
            {
                //get response
                var apiClient = CreatePgApiClient(config => new PaymentGatewayApi(config));
                var response = apiClient.Tokenize(request)
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Rcode != "000")
                    throw new NopException($"Error code - {response.Rcode}. {response.Rmsg}");
                
                //return result
                return response.CardId;
            }).Result;
        }

        /// <summary>
        /// Create customer billing card in Qualpay Customer Vault
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="request">Request parameters to create card</param>
        /// <returns>True if customer card successfully created in the Vault, otherwise false</returns>
        private bool CreateCustomerCard(int customerId, AddBillingCardRequest request)
        {
            return InvokeFunction(() =>
            {
                //get response
                var apiClient = CreatePlatformApiClient(config => new CustomerVaultApi(config));
                var response = apiClient.AddBillingCard(customerId.ToString(), request)
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Code != 0)
                    throw new NopException($"Error code - {response.Code}. {response.Message}");

                //return result
                return true;
            }).Result;
        }

        /// <summary>
        /// Update customer billing card in Qualpay Customer Vault
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="request">Request parameters to update card</param>
        /// <returns>True if customer card successfully updated in the Vault, otherwise false</returns>
        private bool UpdateCustomerCard(int customerId, UpdateBillingCardRequest request)
        {
            return InvokeFunction(() =>
            {
                //get response
                var apiClient = CreatePlatformApiClient(config => new CustomerVaultApi(config));
                var response = apiClient.UpdateBillingCard(customerId.ToString(), request)
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Code != 0)
                    throw new NopException($"Error code - {response.Code}. {response.Message}");

                //return result
                return true;
            }).Result;
        }

        /// <summary>
        /// Get a webhook by the identifier
        /// </summary>
        /// <param name="webhookId">Webhook identifier</param>
        /// <returns>Webhook</returns>
        private Webhook GetWebhook(long? webhookId)
        {
            return InvokeFunction(() =>
            {
                //get response
                var apiClient = CreatePlatformApiClient(config => new WebhooksApi(config));
                var response = apiClient.GetWebhook(webhookId)
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Code != 0)
                    throw new NopException($"Error code - {response.Code}. {response.Message}");
                
                //return result
                return response.Data;
            }).Result;
        }

        #endregion

        #endregion

        #region Methods

        #region Platform

        /// <summary>
        /// Get a customer from Qualpay Customer Vault by the passed identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Customer Vault</returns>
        public CustomerVault GetCustomer(int customerId)
        {
            return InvokeFunction(() =>
            {
                //get response
                var apiClient = CreatePlatformApiClient(config => new CustomerVaultApi(config));
                var response = apiClient.GetCustomer(customerId.ToString())
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Code != 0)
                    throw new NopException($"Error code - {response.Code}. {response.Message}");
                
                //return result
                return response.Data;
            }).Result;
        }

        /// <summary>
        /// Create new customer in Qualpay Customer Vault
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Customer Vault</returns>
        public CustomerVault CreateCustomer(Core.Domain.Customers.Customer customer)
        {
            return InvokeFunction(() =>
            {
                //get response
                var apiClient = CreatePlatformApiClient(config => new CustomerVaultApi(config));
                var response = apiClient.AddCustomer(CreateCustomerRequest(customer))
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Code != 0)
                    throw new NopException($"Error code - {response.Code}. {response.Message}");
                
                //return result
                return response.Data;
            }).Result;
        }

        /// <summary>
        /// Get customer billing cards from Qualpay Customer Vault
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>List of customer billing cards</returns>
        public IList<BillingCard> GetCustomerCards(int customerId)
        {
            return InvokeFunction(() =>
            {
                //get response
                var apiClient = CreatePlatformApiClient(config => new CustomerVaultApi(config));
                var response = apiClient.GetBillingCards(customerId.ToString())
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Code != 0)
                    throw new NopException($"Error code - {response.Code}. {response.Message}");
                
                //return result
                return response.Data?.BillingCards;
            }).Result ?? new List<BillingCard>();
        }

        /// <summary>
        /// Delete customer billing card from Qualpay Customer Vault
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="cardId">Card identifier</param>
        /// <returns>True if customer card successfully deleted from the Vault, otherwise false</returns>
        public bool DeleteCustomerCard(int customerId, string cardId)
        {
            return InvokeFunction(() =>
            {
                //get response
                var apiClient = CreatePlatformApiClient(config => new CustomerVaultApi(config));
                var response = apiClient.DeleteBillingCard(customerId.ToString(), new DeleteBillingCardRequest (cardId: cardId))
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Code != 0)
                    throw new NopException($"Error code - {response.Code}. {response.Message}");

                //return result
                return true;
            }).Result;
        }

        /// <summary>
        /// Get transient key from Qualpay Embedded Fields
        /// </summary>
        /// <returns>Embedded key</returns>
        public EmbeddedKey GetTransientKey()
        {
            return InvokeFunction(() =>
            {
                //get response
                var apiClient = CreatePlatformApiClient(config => new EmbeddedFieldsApi(config));
                var response = apiClient.GetEmbeddedTransientKey()
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Code != 0)
                    throw new NopException($"Error code - {response.Code}. {response.Message}");
                
                //return result
                return response.Data;
            }).Result;
        }

        /// <summary>
        /// Create webhook
        /// </summary>
        /// <param name="webhookId">Current webhook identifier if exists</param>
        /// <returns>Webhook</returns>
        public Webhook CreateWebhook(string webhookId = null)
        {
            return InvokeFunction(() =>
            {
                //check current webhook status if exists
                if (!string.IsNullOrEmpty(webhookId) && long.TryParse(webhookId, out var id))
                {
                    var webhook = GetWebhook(id);
                    if (webhook.Status == Webhook.StatusEnum.ACTIVE)
                        return webhook;
                }

                //prepare request
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
                var request = new Webhook
                (
                    emailAddress: new List<string> { _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId)?.Email },
                    events: new List<string>
                    {
                        QualpayDefaults.SubscriptionPaymentFailureWebhookEvent,
                        QualpayDefaults.SubscriptionPaymentSuccessWebhookEvent,
                        QualpayDefaults.ValidateUrlWebhookEvent
                    },
                    label: QualpayDefaults.WebhookLabel,
                    notificationUrl: urlHelper.RouteUrl(QualpayDefaults.WebhookRouteName, null, Uri.UriSchemeHttps)
                )
                {
                    Status = Webhook.StatusEnum.ACTIVE
                };

                //get response
                var apiClient = CreatePlatformApiClient(config => new WebhooksApi(config));
                var response = apiClient.AddWebhook(request)
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Code != 0)
                    throw new NopException($"Error code - {response.Code}. {response.Message}");
                
                //return result
                return response.Data;
            }).Result;
        }

        /// <summary>
        /// Get subscription transactions
        /// </summary>
        /// <param name="subscriptionId">Subscription identifier</param>
        /// <returns>Collection of transactions</returns>
        public IEnumerable<Transaction> GetSubscriptionTransactions(long? subscriptionId)
        {
            return InvokeFunction(() =>
            {
                //get response
                var apiClient = CreatePlatformApiClient(config => new RecurringBillingApi(config));
                var response = apiClient.GetSubscriptionTransactions(subscriptionId)
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Code != 0)
                    throw new NopException($"Error code - {response.Code}. {response.Message}");
                
                //return result
                return response.Data;
            }).Result ?? new List<Transaction>();
        }

        /// <summary>
        /// Create subscription
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Subscription; error message if exists</returns>
        public (Subscription Subscription, string ErrorMessage) CreateSubscription(ProcessPaymentRequest processPaymentRequest)
        {
            return InvokeFunction(() =>
            {
                //prepare request
                var request = CreateSubscriptionRequest(processPaymentRequest);

                //get response
                var apiClient = CreatePlatformApiClient(config => new RecurringBillingApi(config));
                var response = apiClient.AddSubscription(request)
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Code != 0)
                    throw new NopException($"Error code - {response.Code}. {response.Message}");
                
                //return result
                return response.Data;
            });
        }

        /// <summary>
        /// Cancel subscription
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="subscriptionId">Subscription identifier</param>
        /// <returns>Subscription; error message if exists</returns>
        public (Subscription Subscription, string ErrorMessage) CancelSubscription(string customerId, string subscriptionId)
        {
            return InvokeFunction(() =>
            {
                //get response
                var apiClient = CreatePlatformApiClient(config => new RecurringBillingApi(config));
                var response = apiClient.CancelSubscription(long.TryParse(subscriptionId, out var id) ? (long?)id : null,
                    new CancelSubscriptionRequest (customerId: customerId))
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Code != 0)
                    throw new NopException($"Error code - {response.Code}. {response.Message}");
                
                //return result
                return response.Data;
            });
        }

        #endregion

        #region Payment Gateway

        /// <summary>
        /// Authorize a transaction
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Response; error message if exists</returns>
        public (PGApiTransactionResponse Response, string errorMessage) Authorize(ProcessPaymentRequest processPaymentRequest)
        {
            return InvokeFunction(() =>
            {
                //create request
                var request = CreateTransactionRequest(processPaymentRequest);
                
                //get response
                var apiClient = CreatePgApiClient(config => new PaymentGatewayApi(config));
                var response = apiClient.Authorization(request)
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Rcode != "000")
                    throw new NopException($"Error code - {response.Rcode}. {response.Rmsg}");

                //return result
                return response;
            });
        }

        /// <summary>
        /// Sale
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Response; error message if exists</returns>
        public (PGApiTransactionResponse Response, string errorMessage) Sale(ProcessPaymentRequest processPaymentRequest)
        {
            return InvokeFunction(() =>
            {
                //create request
                var request = CreateTransactionRequest(processPaymentRequest);
                
                //get response
                var apiClient = CreatePgApiClient(config => new PaymentGatewayApi(config));
                var response = apiClient.Sale(request)
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Rcode != "000")
                    throw new NopException($"Error code - {response.Rcode}. {response.Rmsg}");

                //return result
                return response;
            });
        }

        /// <summary>
        /// Capture an authorized transaction
        /// </summary>
        /// <param name="transactionId">Transaction identifier</param>
        /// <param name="amount">Amount to capture</param>
        /// <returns>Response; error message if exists</returns>
        public (PGApiCaptureResponse Response, string errorMessage) CaptureTransaction(string transactionId, decimal amount)
        {
            return InvokeFunction(() =>
            {
                //prepare request
                var request = new PGApiCaptureRequest
                (
                    amtTran: Convert.ToDouble(amount),
                    merchantId: long.Parse(_qualpaySettings.MerchantId),
                    developerId: $"{QualpayDefaults.DeveloperId}-{NopVersion.CurrentVersion}"
                );

                //get response
                var apiClient = CreatePgApiClient(config => new PaymentGatewayApi(config));
                var response = apiClient.Capture(transactionId, request)
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Rcode != "000")
                    throw new NopException($"Error code - {response.Rcode}. {response.Rmsg}");

                //return result
                return response;
            });
        }

        /// <summary>
        /// Void an authorized transaction
        /// </summary>
        /// <param name="transactionId">Transaction identifier</param>
        /// <returns>Response; error message if exists</returns>
        public (PGApiVoidResponse Response, string errorMessage) VoidTransaction(string transactionId)
        {
            return InvokeFunction(() =>
            {
                //prepare request
                var request = new PGApiVoidRequest
                (
                    merchantId: long.Parse(_qualpaySettings.MerchantId),
                    developerId: $"{QualpayDefaults.DeveloperId}-{NopVersion.CurrentVersion}"
                );

                //get response
                var apiClient = CreatePgApiClient(config => new PaymentGatewayApi(config));
                var response = apiClient.CallVoid(transactionId, request)
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Rcode != "000")
                    throw new NopException($"Error code - {response.Rcode}. {response.Rmsg}");

                //return result
                return response;
            });
        }

        /// <summary>
        /// Refund a charged transaction
        /// </summary>
        /// <param name="transactionId">Transaction identifier</param>
        /// <param name="amount">Amount to capture</param>
        /// <returns>Response; error message if exists</returns>
        public (PGApiRefundResponse Response, string errorMessage) Refund(string transactionId, decimal amount)
        {
            return InvokeFunction(() =>
            {
                //prepare request
                var request = new PGApiRefundRequest
                (
                    amtTran: Convert.ToDouble(amount),
                    merchantId: long.Parse(_qualpaySettings.MerchantId),
                    developerId: $"{QualpayDefaults.DeveloperId}-{NopVersion.CurrentVersion}"
                );

                //get response
                var apiClient = CreatePgApiClient(config => new PaymentGatewayApi(config));
                var response = apiClient.Refund(transactionId, request)
                    ?? throw new NopException("No response from service");

                //whether request is succeeded
                if (response.Rcode != "000")
                    throw new NopException($"Error code - {response.Rcode}. {response.Rmsg}");

                //return result
                return response;
            });
        }

        #endregion

        /// <summary>
        /// Validate webhook request and return received event if is valid
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Validation result; received event details</returns>
        public (bool Valid, WebhookEvent<Subscription> webhookEvent) GetSubscriptionFromWebhookRequest(HttpRequest request)
        {
            var result = ValidateWebhook<Subscription>(request);
            return (result.Valid, result.Data);
        }

        /// <summary>
        /// Subscribe to Qualpay news
        /// </summary>
        /// <param name="email">Email address</param>
        /// <returns>True if successfully subscribed/unsubscribed, otherwise false; error message if exists</returns>
        public (bool Result, string ErrorMessage) SubscribeForQualpayNews(string email)
        {
            return InvokeFunction(() =>
            {
                //unsubscribe previous email
                if (!string.IsNullOrEmpty(_qualpaySettings.MerchantEmail))
                    SendEmail(_qualpaySettings.MerchantEmail, false);

                //subscribe new email
                if (!string.IsNullOrEmpty(email))
                    SendEmail(email, true);

                return true;
            });
        }

        #endregion
    }
}