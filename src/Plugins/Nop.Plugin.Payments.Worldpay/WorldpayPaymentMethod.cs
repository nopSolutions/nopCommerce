using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.Worldpay.Domain;
using Nop.Plugin.Payments.Worldpay.Domain.Enums;
using Nop.Plugin.Payments.Worldpay.Domain.Models;
using Nop.Plugin.Payments.Worldpay.Domain.Requests;
using Nop.Plugin.Payments.Worldpay.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;

namespace Nop.Plugin.Payments.Worldpay
{
    /// <summary>
    /// Represents Worldpay payment method
    /// </summary>
    public class WorldpayPaymentMethod : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IPaymentService _paymentService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWebHelper _webHelper;
        private readonly WorldpayPaymentManager _worldpayPaymentManager;
        private readonly WorldpayPaymentSettings _worldpayPaymentSettings;

        #endregion

        #region Ctor

        public WorldpayPaymentMethod(ICurrencyService currencyService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IPaymentService paymentService,
            ISettingService settingService,
            IStoreService storeService,
            IWebHelper webHelper,
            WorldpayPaymentManager worldpayPaymentManager,
            WorldpayPaymentSettings worldpayPaymentSettings)
        {
            this._currencyService = currencyService;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._logger = logger;
            this._paymentService = paymentService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._webHelper = webHelper;
            this._worldpayPaymentManager = worldpayPaymentManager;
            this._worldpayPaymentSettings = worldpayPaymentSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Process a regular or recurring payment
        /// </summary>
        /// <param name="paymentRequest">Payment request parameters</param>
        /// <param name="isRecurringPayment">Whether it is a recurring payment</param>
        /// <returns>Process payment result</returns>
        private ProcessPaymentResult ProcessPayment(ProcessPaymentRequest paymentRequest, bool isRecurringPayment)
        {
            //create request parameters
            var request = CreateChargeRequest(paymentRequest, isRecurringPayment);

            //charge transaction
            var transaction = _worldpayPaymentSettings.TransactionMode == TransactionMode.Authorize
                ? _worldpayPaymentManager.Authorize(new AuthorizeRequest(request)) : _worldpayPaymentManager.Charge(request)
                ?? throw new NopException("An error occurred while processing. Error details in the log");

            //save card identifier to payment custom values for further purchasing
            if (isRecurringPayment && !string.IsNullOrEmpty(transaction.VaultData?.Token?.PaymentMethodId))
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Worldpay.Fields.StoredCard.Key"), transaction.VaultData.Token.PaymentMethodId);

            //return result
            var result = new ProcessPaymentResult
            {
                AvsResult = $"{transaction.AvsResult}. Code: {transaction.AvsCode}",
                Cvv2Result = $"{transaction.CvvResult}. Code: {transaction.CvvCode}",
                AuthorizationTransactionCode = transaction.AuthorizationCode
            };

            if (_worldpayPaymentSettings.TransactionMode == TransactionMode.Authorize)
            {
                result.AuthorizationTransactionId = transaction.TransactionId.ToString();
                result.AuthorizationTransactionResult = transaction.ResponseText;
                result.NewPaymentStatus = PaymentStatus.Authorized;
            }

            if (_worldpayPaymentSettings.TransactionMode == TransactionMode.Charge)
            {
                result.CaptureTransactionId = transaction.TransactionId.ToString();
                result.CaptureTransactionResult = transaction.ResponseText;
                result.NewPaymentStatus = PaymentStatus.Paid;
            }

            return result;
        }

        /// <summary>
        /// Create request parameters to charge transaction
        /// </summary>
        /// <param name="paymentRequest">Payment request parameters</param>
        /// <param name="isRecurringPayment">Whether it is a recurring payment</param>
        /// <returns>Charge request parameters</returns>
        private ChargeRequest CreateChargeRequest(ProcessPaymentRequest paymentRequest, bool isRecurringPayment)
        {
            //get customer
            var customer = _customerService.GetCustomerById(paymentRequest.CustomerId);
            if (customer == null)
                throw new NopException("Customer cannot be loaded");

            //whether USD is available
            var usdCurrency = _currencyService.GetCurrencyByCode("USD");
            if (usdCurrency == null)
                throw new NopException("USD currency cannot be loaded");

            //create common charge request parameters
            var request = new ChargeRequest
            {
                OrderId = CommonHelper.EnsureMaximumLength(paymentRequest.OrderGuid.ToString(), 25),
                TransactionDuplicateCheckType = TransactionDuplicateCheckType.NoCheck,
                ExtendedInformation = new ExtendedInformation
                {
                    InvoiceNumber = paymentRequest.OrderGuid.ToString(),
                    InvoiceDescription = $"Order from the '{_storeService.GetStoreById(paymentRequest.StoreId)?.Name}'"
                }
            };

            //set amount in USD
            var amount = _currencyService.ConvertFromPrimaryStoreCurrency(paymentRequest.OrderTotal, usdCurrency);
            request.Amount = Math.Round(amount, 2);

            //get current shopping cart
            var shoppingCart = customer.ShoppingCartItems
                .Where(shoppingCartItem => shoppingCartItem.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(paymentRequest.StoreId).ToList();

            //whether there are non-downloadable items in the shopping cart
            var nonDownloadable = shoppingCart.Any(item => !item.Product.IsDownload);
            request.ExtendedInformation.GoodsType = nonDownloadable ? GoodsType.Physical : GoodsType.Digital;

            //try to get previously stored card details
            var storedCardKey = _localizationService.GetResource("Plugins.Payments.Worldpay.Fields.StoredCard.Key");
            if (paymentRequest.CustomValues.TryGetValue(storedCardKey, out object storedCardId) && !storedCardId.ToString().Equals(Guid.Empty.ToString()))
            {
                //check whether customer exists in Vault
                var vaultCustomer = _worldpayPaymentManager.GetCustomer(_genericAttributeService.GetAttribute<string>(customer, WorldpayPaymentDefaults.CustomerIdAttribute))
                    ?? throw new NopException("Failed to retrieve customer");

                //use previously stored card to charge
                request.PaymentVaultToken = new VaultToken
                {
                    CustomerId = vaultCustomer.CustomerId,
                    PaymentMethodId = storedCardId.ToString()
                };

                return request;
            }

            //or try to get the card token
            var cardTokenKey = _localizationService.GetResource("Plugins.Payments.Worldpay.Fields.Token.Key");
            if (!paymentRequest.CustomValues.TryGetValue(cardTokenKey, out object token) || string.IsNullOrEmpty(token?.ToString()))
                throw new NopException("Failed to get the card token");

            //remove the card token from payment custom values, since it is no longer needed
            paymentRequest.CustomValues.Remove(cardTokenKey);

            //public key is required to pay with card token
            request.PaymentVaultToken = new VaultToken
            {
                PublicKey = _worldpayPaymentSettings.PublicKey
            };

            //whether to save card details for the future purchasing
            var saveCardKey = _localizationService.GetResource("Plugins.Payments.Worldpay.Fields.SaveCard.Key");
            if (paymentRequest.CustomValues.TryGetValue(saveCardKey, out object saveCardValue) && saveCardValue is bool saveCard && saveCard && !customer.IsGuest())
            {
                //remove the value from payment custom values, since it is no longer needed
                paymentRequest.CustomValues.Remove(saveCardKey);

                try
                {
                    //check whether customer exists and try to create the new one, if not exists
                    var vaultCustomer = _worldpayPaymentManager.GetCustomer(_genericAttributeService.GetAttribute<string>(customer, WorldpayPaymentDefaults.CustomerIdAttribute))
                        ?? _worldpayPaymentManager.CreateCustomer(new CreateCustomerRequest
                        {
                            CustomerId = customer.Id.ToString(),
                            CustomerDuplicateCheckType = CustomerDuplicateCheckType.Ignore,
                            EmailReceiptEnabled = !string.IsNullOrEmpty(customer.Email),
                            Email = customer.Email,
                            FirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute),
                            LastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute),
                            Company = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute),
                            Phone = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute),
                            BillingAddress = new Address
                            {
                                Line1 = customer.BillingAddress?.Address1,
                                City = customer.BillingAddress?.City,
                                State = customer.BillingAddress?.StateProvince?.Abbreviation,
                                Country = customer.BillingAddress?.Country?.TwoLetterIsoCode,
                                Zip = customer.BillingAddress?.ZipPostalCode,
                                Company = customer.BillingAddress?.Company,
                                Phone = customer.BillingAddress?.PhoneNumber
                            }
                        }) ?? throw new NopException("Failed to create customer. Error details in the log");

                    //save Vault customer identifier as a generic attribute
                    _genericAttributeService.SaveAttribute(customer, WorldpayPaymentDefaults.CustomerIdAttribute, vaultCustomer.CustomerId);

                    //add card to the Vault after charge
                    request.AddToVault = true;
                    request.PaymentVaultToken.CustomerId = vaultCustomer.CustomerId;
                }
                catch (Exception exception)
                {
                    _logger.Warning(exception.Message, exception, customer);
                    if (isRecurringPayment)
                        throw new NopException("For recurring payments you need to save the card details");
                }
            }
            else if (isRecurringPayment)
                throw new NopException("For recurring payments you need to save the card details");

            //use card token to charge
            request.PaymentVaultToken.PaymentMethodId = token.ToString();

            return request;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest == null)
                throw new ArgumentException(nameof(processPaymentRequest));

            return ProcessPayment(processPaymentRequest, false);
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //do nothing
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var result = _paymentService.CalculateAdditionalFee(cart,
                _worldpayPaymentSettings.AdditionalFee, _worldpayPaymentSettings.AdditionalFeePercentage);

            return result;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            if (capturePaymentRequest == null)
                throw new ArgumentException(nameof(capturePaymentRequest));

            //set amount in USD
            var amount = _currencyService.ConvertCurrency(capturePaymentRequest.Order.OrderTotal, capturePaymentRequest.Order.CurrencyRate);

            //whether there are non-downloadable order items
            var nonDownloadable = capturePaymentRequest.Order.OrderItems.Any(item => !item.Product?.IsDownload ?? true);

            //capture transaction
            var transaction = _worldpayPaymentManager.CaptureTransaction(new CaptureRequest
            {
                TransactionId = capturePaymentRequest.Order.AuthorizationTransactionId,
                Amount = Math.Round(amount, 2),
                ExtendedInformation = new ExtendedInformation
                {
                    GoodsType = nonDownloadable ? GoodsType.Physical : GoodsType.Digital,
                    InvoiceNumber = capturePaymentRequest.Order.CustomOrderNumber,
                    InvoiceDescription = $"Order from the '{_storeService.GetStoreById(capturePaymentRequest.Order.StoreId)?.Name}'"
                }
            }) ?? throw new NopException("An error occurred while processing. Error details in the log");

            //successfully captured
            return new CapturePaymentResult
            {
                NewPaymentStatus = PaymentStatus.Paid,
                CaptureTransactionId = transaction.TransactionId.ToString(),
                CaptureTransactionResult = transaction.ResponseText
            };
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            if (refundPaymentRequest == null)
                throw new ArgumentException(nameof(refundPaymentRequest));

            //whether USD is available
            var usdCurrency = _currencyService.GetCurrencyByCode("USD");
            if (usdCurrency == null)
                throw new NopException("USD currency cannot be loaded");

            //set amount in USD
            var amount = _currencyService.ConvertCurrency(refundPaymentRequest.AmountToRefund, refundPaymentRequest.Order.CurrencyRate);

            var transaction = _worldpayPaymentManager.Refund(new RefundRequest
            {
                TransactionId = refundPaymentRequest.Order.CaptureTransactionId,
                Amount = Math.Round(amount, 2),
                OrderId = CommonHelper.EnsureMaximumLength(Guid.NewGuid().ToString(), 25)
            }) ?? throw new NopException("An error occurred while processing. Error details in the log");

            //successfully refunded
            return new RefundPaymentResult
            {
                NewPaymentStatus = refundPaymentRequest.IsPartialRefund ? PaymentStatus.PartiallyRefunded : PaymentStatus.Refunded
            };
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            if (voidPaymentRequest == null)
                throw new ArgumentException(nameof(voidPaymentRequest));

            //void transaction
            var transaction = _worldpayPaymentManager.VoidTransaction(new VoidRequest
            {
                TransactionId = voidPaymentRequest.Order.AuthorizationTransactionId,
                OrderId = CommonHelper.EnsureMaximumLength(Guid.NewGuid().ToString(), 25),
                VoidType = VoidType.MerchantGenerated
            }) ?? throw new NopException("An error occurred while processing. Error details in the log");

            //successfully voided
            return new VoidPaymentResult
            {
                NewPaymentStatus = PaymentStatus.Voided
            };
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest == null)
                throw new ArgumentException(nameof(processPaymentRequest));

            return ProcessPayment(processPaymentRequest, true);
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            if (cancelPaymentRequest == null)
                throw new ArgumentException(nameof(cancelPaymentRequest));

            //always success
            return new CancelRecurringPaymentResult();
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //it's not a redirection payment method. So we always return false
            return false;
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentException(nameof(form));

            //try to get errors
            if (form.TryGetValue("Errors", out StringValues errorsString) && !StringValues.IsNullOrEmpty(errorsString))
                return new[] { errorsString.ToString() }.ToList();

            return new List<string>();
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentException(nameof(form));

            var paymentRequest = new ProcessPaymentRequest();

            //pass custom values to payment method
            if (form.TryGetValue("Token", out StringValues token) && !StringValues.IsNullOrEmpty(token))
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Worldpay.Fields.Token.Key"), token.ToString());

            if (form.TryGetValue("StoredCardId", out StringValues storedCardId) && !StringValues.IsNullOrEmpty(storedCardId) && !storedCardId.Equals(Guid.Empty.ToString()))
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Worldpay.Fields.StoredCard.Key"), storedCardId.ToString());

            if (form.TryGetValue("SaveCard", out StringValues saveCardValue) && !StringValues.IsNullOrEmpty(saveCardValue) && bool.TryParse(saveCardValue[0], out bool saveCard) && saveCard)
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Worldpay.Fields.SaveCard.Key"), saveCard);

            return paymentRequest;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentWorldpay/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return WorldpayPaymentDefaults.ViewComponentName;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new WorldpayPaymentSettings
            {
                UseSandbox = true,
                TransactionMode = TransactionMode.Charge
            });

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.Enums.CreditCardType.AMEX", "AMEX");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.Enums.CreditCardType.Discover", "DISCOVER");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.Enums.CreditCardType.MasterCard", "MasterCard");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.Enums.CreditCardType.MasterCardFleet", "MasterCard Fleet");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.Enums.CreditCardType.Visa", "VISA");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.Enums.CreditCardType.VisaFleet", "VISA Fleet");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.TransactionMode.Authorize", "Authorize only");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.TransactionMode.Charge", "Charge (authorize and capture)");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.AdditionalFee", "Additional fee");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.CardId", "Card ID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.CardType", "Card type");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.ExpirationDate", "Expiration date");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.MaskedNumber", "Masked number");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.PublicKey", "Public key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.PublicKey.Hint", "Specify the Public key. It will be sent to the email address that you signed up with during the sandbox sign-up process.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.SaveCard", "Save the card data for future purchasing");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.SaveCard.Key", "Save card details");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.SecureKey", "Secure key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.SecureKey.Hint", "Specify the Secure key. You can obtain the Secure Key by signing into the Virtual Terminal with the login credentials that you were emailed to you during the sign-up process. You will then need to navigate to Settings and click on the Key Management link.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.SecureNetId", "SecureNet ID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.SecureNetId.Hint", "Specify the SecureNet ID. You will get this in an email shortly after signing up for your account.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.StoredCard", "Use a previously saved card");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.StoredCard.Key", "Pay using stored card identifier");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.StoredCard.SelectCard", "Select a card");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.Token.Key", "Pay using card token");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.TransactionMode", "Transaction mode");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.TransactionMode.Hint", "Choose the transaction mode.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.UseSandbox", "Use sandbox");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.UseSandbox.Hint", "Determine whether to enable sandbox (testing environment).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.ValidateAddress", "Validate address");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.ValidateAddress.Hint", "Determine whether to validate customers' billing addresses on processing payments.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.WorldpayCustomerId", "Worldpay Vault customer ID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Fields.WorldpayCustomerId.Hint", "Displays Worldpay Vault customer ID. If the customer is not yet stored in the Worldpay Vault, specify the customer ID and click the button 'Store customer to the Worldpay Vault'");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.Instructions", @"
                <p>
                    For plugin configuration follow these steps:
                    <ul style=""list-style-type:none;"">
                        <li>
                            1. Sign up for a <a href=""https://www.worldpay.us/Partner/nopcommerce"" target=""_blank"">Merchant Account</a>
                        </li>
                        <li>
                            2. Upon merchant account approval an email will be sent to you with your Virtual Terminal credentials where you can obtain your SecureNet ID, Secure Key and Public Key
                        </li>
                        <li>
                            3. Steps to obtain each:
                            <ul style=""list-style-type:none;"">
                                <li>a. Sign into the Virtual Terminal with the login credentials that you received</li>
                                <li>b. The SecureNet ID will be found on the upper right hand side of your Virtual Terminal page</li>
                                <li>c. To obtain your Keys, navigate to the Settings tab and select the Key Management link</li>
                                <li>d. Select appropriate tab – SecureKey or Public Key</li>
                            </ul>
                        </li>
                        <li>
                            4. Fill in the remaining fields and save to complete the configuration
                        </li>
                    </ul>
                    <br /><em>Note: This plugin supports merchants domiciled in the United States only.</em>
                    <br />
                    <em>Note: The Worldpay US platform supports only USD currency; ensure that you have correctly configured the exchange rate from your primary store currency to the USD currency.</em>
                    <br />
                </p>");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.PaymentMethodDescription", "Pay by credit card using Worldpay");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.WorldpayCustomer", "Worldpay Vault");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.WorldpayCustomer.Create", "Store customer to the Worldpay Vault");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Worldpay.WorldpayCustomer.NotExists", "This customer is not yet stored in the Worldpay Vault");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<WorldpayPaymentSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.Enums.CreditCardType.AMEX");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.Enums.CreditCardType.Discover");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.Enums.CreditCardType.MasterCard");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.Enums.CreditCardType.MasterCardFleet");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.Enums.CreditCardType.Visa");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.Enums.CreditCardType.VisaFleet");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.TransactionMode.Authorize");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Payments.Worldpay.Domain.TransactionMode.Charge");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.AdditionalFee");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.AdditionalFee.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.AdditionalFeePercentage");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.AdditionalFeePercentage.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.CardId");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.CardType");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.ExpirationDate");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.MaskedNumber");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.PublicKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.PublicKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.SaveCard");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.SaveCard.Key");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.SecureKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.SecureKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.SecureNetId");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.SecureNetId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.StoredCard");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.StoredCard.Key");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.StoredCard.SelectCard");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.Token.Key");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.TransactionMode");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.TransactionMode.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.UseSandbox");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.UseSandbox.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.ValidateAddress");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.ValidateAddress.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.WorldpayCustomerId");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Fields.WorldpayCustomerId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.Instructions");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.PaymentMethodDescription");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.WorldpayCustomer");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.WorldpayCustomer.Create");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Worldpay.WorldpayCustomer.NotExists");

            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get { return RecurringPaymentType.Manual; }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public Nop.Services.Payments.PaymentMethodType PaymentMethodType
        {
            get { return Nop.Services.Payments.PaymentMethodType.Standard; }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public string PaymentMethodDescription
        {
            //return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
            //for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
            get { return _localizationService.GetResource("Plugins.Payments.Worldpay.PaymentMethodDescription"); }
        }

        #endregion
    }
}