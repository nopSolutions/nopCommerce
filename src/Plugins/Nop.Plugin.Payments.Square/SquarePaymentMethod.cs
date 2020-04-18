using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Tasks;
using Nop.Plugin.Payments.Square.Domain;
using Nop.Plugin.Payments.Square.Extensions;
using Nop.Plugin.Payments.Square.Models;
using Nop.Plugin.Payments.Square.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Tasks;
using Nop.Web.Framework.UI;
using SquareModel = Square.Models;

namespace Nop.Plugin.Payments.Square
{
    /// <summary>
    /// Represents Square payment method
    /// </summary>
    public class SquarePaymentMethod : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IPaymentService _paymentService;
        private readonly IPageHeadBuilder _pageHeadBuilder;
        private readonly ISettingService _settingService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IWebHelper _webHelper;
        private readonly SquarePaymentManager _squarePaymentManager;
        private readonly SquarePaymentSettings _squarePaymentSettings;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public SquarePaymentMethod(CurrencySettings currencySettings,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IPaymentService paymentService,
            IPageHeadBuilder pageHeadBuilder,
            ISettingService settingService,
            IScheduleTaskService scheduleTaskService,
            IStateProvinceService stateProvinceService,
            IWebHelper webHelper,
            SquarePaymentManager squarePaymentManager,
            SquarePaymentSettings squarePaymentSettings,
            IStoreContext storeContext)
        {
            _currencySettings = currencySettings;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _paymentService = paymentService;
            _pageHeadBuilder = pageHeadBuilder;
            _settingService = settingService;
            _scheduleTaskService = scheduleTaskService;
            _stateProvinceService = stateProvinceService;
            _webHelper = webHelper;
            _squarePaymentManager = squarePaymentManager;
            _squarePaymentSettings = squarePaymentSettings;
            _storeContext = storeContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check supported currency 
        /// </summary>
        /// <param name="currency">Currency</param>
        /// <returns>True - value must be correspond to ISO 4217, else - false</returns>
        private bool CheckSupportCurrency(Currency currency)
        {
            foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                var regionInfo = new RegionInfo(culture.Name);
                if (currency.CurrencyCode.Equals(regionInfo.ISOCurrencySymbol, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a payment status by tender card details status
        /// </summary>
        /// <param name="status">Tender card details status</param>
        /// <returns>Payment status</returns>
        private PaymentStatus GetPaymentStatus(string status)
        {
            return status switch
            {
                SquarePaymentDefaults.PAYMENT_APPROVED_STATUS => PaymentStatus.Authorized,
                SquarePaymentDefaults.PAYMENT_COMPLETED_STATUS => PaymentStatus.Paid,
                SquarePaymentDefaults.PAYMENT_FAILED_STATUS => PaymentStatus.Pending,
                SquarePaymentDefaults.PAYMENT_CANCELED_STATUS => PaymentStatus.Voided,
                _ => PaymentStatus.Pending,
            };
        }

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="paymentRequest">Payment info required for an order processing</param>
        /// <param name="isRecurringPayment">Whether it is a recurring payment</param>
        /// <returns>Process payment result</returns>
        private ProcessPaymentResult ProcessPayment(ProcessPaymentRequest paymentRequest, bool isRecurringPayment)
        {
            //create charge request
            var squarePaymentRequest = CreatePaymentRequest(paymentRequest, isRecurringPayment);

            //charge transaction for current store
            var storeId = _storeContext.CurrentStore.Id;
            var (payment, error) = _squarePaymentManager.CreatePayment(squarePaymentRequest, storeId);
            if (payment == null)
                throw new NopException(error);

            //get transaction details
            var paymentStatus = payment.Status;
            var paymentResult = $"Payment was processed. Status is {paymentStatus}";

            //return result
            var result = new ProcessPaymentResult
            {
                NewPaymentStatus = GetPaymentStatus(paymentStatus)
            };

            if (_squarePaymentSettings.TransactionMode == TransactionMode.Authorize)
            {
                result.AuthorizationTransactionId = payment.Id;
                result.AuthorizationTransactionResult = paymentResult;
            }

            if (_squarePaymentSettings.TransactionMode == TransactionMode.Charge)
            {
                result.CaptureTransactionId = payment.Id;
                result.CaptureTransactionResult = paymentResult;
            }

            return result;
        }

        /// <summary>
        /// Create request parameters to charge transaction
        /// </summary>
        /// <param name="paymentRequest">Payment request parameters</param>
        /// <param name="isRecurringPayment">Whether it is a recurring payment</param>
        /// <returns>Charge request parameters</returns>
        private ExtendedCreatePaymentRequest CreatePaymentRequest(ProcessPaymentRequest paymentRequest, bool isRecurringPayment)
        {
            //get customer
            var customer = _customerService.GetCustomerById(paymentRequest.CustomerId);
            if (customer == null)
                throw new NopException("Customer cannot be loaded");

            //get the primary store currency
            var currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (currency == null)
                throw new NopException("Primary store currency cannot be loaded");

            //whether the currency is supported by the Square
            if (!CheckSupportCurrency(currency))
                throw new NopException($"The {currency.CurrencyCode} currency is not supported by the Square");

            var storeId = _storeContext.CurrentStore.Id;

            //check customer's billing address, shipping address and email, 
            SquareModel.Address createAddress(Address address)
            {
                if (address == null)
                    return null;

                var country = _countryService.GetCountryByAddress(address);

                return new SquareModel.Address
                (
                    addressLine1: address.Address1,
                    addressLine2: address.Address2,
                    administrativeDistrictLevel1: _stateProvinceService.GetStateProvinceByAddress(address)?.Abbreviation,
                    administrativeDistrictLevel2: address.County,
                    country: string.Equals(country?.TwoLetterIsoCode, new RegionInfo(country?.TwoLetterIsoCode).TwoLetterISORegionName, StringComparison.InvariantCultureIgnoreCase)
                        ? country?.TwoLetterIsoCode : null,
                    firstName: address.FirstName,
                    lastName: address.LastName,
                    locality: address.City,
                    postalCode: address.ZipPostalCode
                );
            }

            var customerBillingAddress = _customerService.GetCustomerBillingAddress(customer);
            var customerShippingAddress = _customerService.GetCustomerShippingAddress(customer);

            var billingAddress = createAddress(customerBillingAddress);
            var shippingAddress = billingAddress == null ? createAddress(customerShippingAddress) : null;
            var email = customerBillingAddress != null ? customerBillingAddress.Email : customerShippingAddress?.Email;

            //the transaction is ineligible for chargeback protection if they are not provided
            if ((billingAddress == null && shippingAddress == null) || string.IsNullOrEmpty(email))
                _logger.Warning("Square payment warning: Address or email is not provided, so the transaction is ineligible for chargeback protection", customer: customer);

            //the amount of money, in the smallest denomination of the currency indicated by currency. For example, when currency is USD, amount is in cents;
            //most currencies consist of 100 units of smaller denomination, so we multiply the total by 100
            var orderTotal = (int)(paymentRequest.OrderTotal * 100);
            var amountMoney = new SquareModel.Money(orderTotal, currency.CurrencyCode);

            //try to get the verification token if exists
            var tokenKey = _localizationService.GetResource("Plugins.Payments.Square.Fields.Token.Key");
            if ((!paymentRequest.CustomValues.TryGetValue(tokenKey, out var token) || string.IsNullOrEmpty(token?.ToString())) && _squarePaymentSettings.Use3ds)
                throw new NopException("Failed to get the verification token");

            //remove the verification token from payment custom values, since it's no longer needed
            paymentRequest.CustomValues.Remove(tokenKey);

            var location = _squarePaymentManager.GetSelectedActiveLocation(storeId);
            if (location == null)
                throw new NopException("Location is a required parameter for payment requests");

            var paymentRequestBuilder = new SquareModel.CreatePaymentRequest.Builder
                (
                    //Payment source, regardless of whether it is a card on file or a nonce.
                    //this parameter will be initialized below
                    sourceId: null,
                    idempotencyKey: Guid.NewGuid().ToString(),
                    amountMoney: amountMoney
                )
                .Autocomplete(_squarePaymentSettings.TransactionMode == TransactionMode.Charge)
                .BillingAddress(billingAddress)
                .ShippingAddress(shippingAddress)
                .BuyerEmailAddress(email)
                .Note(string.Format(SquarePaymentDefaults.PaymentNote, paymentRequest.OrderGuid))
                .ReferenceId(paymentRequest.OrderGuid.ToString())
                .VerificationToken(token?.ToString())
                .LocationId(location.Id);

            var integrationId = !_squarePaymentSettings.UseSandbox && !string.IsNullOrEmpty(SquarePaymentDefaults.IntegrationId)
                ? SquarePaymentDefaults.IntegrationId
                : null;

            //try to get previously stored card details
            var storedCardKey = _localizationService.GetResource("Plugins.Payments.Square.Fields.StoredCard.Key");
            if (paymentRequest.CustomValues.TryGetValue(storedCardKey, out var storedCardId) && !storedCardId.ToString().Equals(Guid.Empty.ToString()))
            {
                //check whether customer exists for current store
                var customerId = _genericAttributeService.GetAttribute<string>(customer, SquarePaymentDefaults.CustomerIdAttribute);
                var squareCustomer = _squarePaymentManager.GetCustomer(customerId, storeId);
                if (squareCustomer == null)
                    throw new NopException("Failed to retrieve customer");

                //set 'card on file'
                return paymentRequestBuilder
                    .CustomerId(squareCustomer.Id)
                    .SourceId(storedCardId.ToString())
                    .Build()
                    .ToExtendedRequest(integrationId);
            }

            //or try to get the card nonce
            var cardNonceKey = _localizationService.GetResource("Plugins.Payments.Square.Fields.CardNonce.Key");
            if (!paymentRequest.CustomValues.TryGetValue(cardNonceKey, out var cardNonce) || string.IsNullOrEmpty(cardNonce?.ToString()))
                throw new NopException("Failed to get the card nonce");

            //remove the card nonce from payment custom values, since it's no longer needed
            paymentRequest.CustomValues.Remove(cardNonceKey);

            //whether to save card details for the future purchasing
            var saveCardKey = _localizationService.GetResource("Plugins.Payments.Square.Fields.SaveCard.Key");
            if (paymentRequest.CustomValues.TryGetValue(saveCardKey, out var saveCardValue) && saveCardValue is bool saveCard && saveCard && !_customerService.IsGuest(customer))
            {
                //remove the value from payment custom values, since it is no longer needed
                paymentRequest.CustomValues.Remove(saveCardKey);

                try
                {
                    //check whether customer exists for current store
                    var customerId = _genericAttributeService.GetAttribute<string>(customer, SquarePaymentDefaults.CustomerIdAttribute);
                    var squareCustomer = _squarePaymentManager.GetCustomer(customerId, storeId);

                    if (squareCustomer == null)
                    {
                        //try to create the new one for current store, if not exists
                        var customerRequestBuilder = new SquareModel.CreateCustomerRequest.Builder()
                            .EmailAddress(customer.Email)
                            .Nickname(customer.Username)
                            .GivenName(_genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute))
                            .FamilyName(_genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute))
                            .PhoneNumber(_genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute))
                            .CompanyName(_genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute))
                            .ReferenceId(customer.CustomerGuid.ToString());

                        squareCustomer = _squarePaymentManager.CreateCustomer(customerRequestBuilder.Build(), storeId);
                        if (squareCustomer == null)
                            throw new NopException("Failed to create customer. Error details in the log");

                        //save customer identifier as generic attribute
                        _genericAttributeService.SaveAttribute(customer, SquarePaymentDefaults.CustomerIdAttribute, squareCustomer.Id);
                    }

                    //create request parameters to create the new card
                    var cardRequestBuilder = new SquareModel.CreateCustomerCardRequest.Builder(cardNonce.ToString())
                        .VerificationToken(token?.ToString());

                    var cardBillingAddress = billingAddress ?? shippingAddress;

                    //set postal code
                    var postalCodeKey = _localizationService.GetResource("Plugins.Payments.Square.Fields.PostalCode.Key");
                    if (paymentRequest.CustomValues.TryGetValue(postalCodeKey, out var postalCode) && !string.IsNullOrEmpty(postalCode.ToString()))
                    {
                        //remove the value from payment custom values, since it is no longer needed
                        paymentRequest.CustomValues.Remove(postalCodeKey);

                        cardBillingAddress ??= new SquareModel.Address();
                        cardBillingAddress = cardBillingAddress
                            .ToBuilder()
                            .PostalCode(postalCode.ToString())
                            .Build();
                    }

                    cardRequestBuilder.BillingAddress(cardBillingAddress);

                    //try to create card for current store
                    var card = _squarePaymentManager.CreateCustomerCard(squareCustomer.Id, cardRequestBuilder.Build(), storeId);
                    if (card == null)
                        throw new NopException("Failed to create card. Error details in the log");

                    //save card identifier to payment custom values for further purchasing
                    if (isRecurringPayment)
                        paymentRequest.CustomValues.Add(storedCardKey, card.Id);

                    //set 'card on file'
                    return paymentRequestBuilder
                        .CustomerId(squareCustomer.Id)
                        .SourceId(card.Id)
                        .Build()
                        .ToExtendedRequest(integrationId);
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

            //set 'card nonce'
            return paymentRequestBuilder
                .SourceId(cardNonce.ToString())
                .Build()
                .ToExtendedRequest(integrationId);
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
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return _paymentService.CalculateAdditionalFee(cart,
                _squarePaymentSettings.AdditionalFee, _squarePaymentSettings.AdditionalFeePercentage);
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

            //capture transaction for current store
            var storeId = _storeContext.CurrentStore.Id;
            var transactionId = capturePaymentRequest.Order.AuthorizationTransactionId;
            var (successfullyCompleted, error) = _squarePaymentManager.CompletePayment(transactionId, storeId);
            if (!successfullyCompleted)
                throw new NopException(error);

            //successfully captured
            return new CapturePaymentResult
            {
                NewPaymentStatus = PaymentStatus.Paid,
                CaptureTransactionId = transactionId
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

            //get the primary store currency
            var currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (currency == null)
                throw new NopException("Primary store currency cannot be loaded");

            //whether the currency is supported by the Square
            if (!CheckSupportCurrency(currency))
                throw new NopException($"The {currency.CurrencyCode} currency is not supported by the Square");

            //the amount of money in the smallest denomination of the currency indicated by currency. For example, when currency is USD, amount is in cents;
            //most currencies consist of 100 units of smaller denomination, so we multiply the total by 100
            var orderTotal = (int)(refundPaymentRequest.AmountToRefund * 100);
            var amountMoney = new SquareModel.Money(orderTotal, currency.CurrencyCode);

            //first try to get the transaction for current store
            var storeId = _storeContext.CurrentStore.Id;
            var transactionId = refundPaymentRequest.Order.CaptureTransactionId;

            var paymentRefundRequest = new SquareModel.RefundPaymentRequest
                (
                    idempotencyKey: Guid.NewGuid().ToString(),
                    amountMoney: amountMoney,
                    paymentId: transactionId
                );

            var (paymentRefund, paymentRefundError) = _squarePaymentManager.RefundPayment(paymentRefundRequest, storeId);
            if (paymentRefund == null)
                throw new NopException(paymentRefundError);

            //if refund status is 'pending', try to refund once more with the same request parameters for current store
            if (paymentRefund.Status == SquarePaymentDefaults.REFUND_STATUS_PENDING)
            {
                (paymentRefund, paymentRefundError) = _squarePaymentManager.RefundPayment(paymentRefundRequest, storeId);
                if (paymentRefund == null)
                    throw new NopException(paymentRefundError);
            }

            //check whether refund is completed
            if (paymentRefund.Status != SquarePaymentDefaults.REFUND_STATUS_COMPLETED)
            {
                //change error notification to warning one (for the pending status)
                if (paymentRefund.Status == SquarePaymentDefaults.REFUND_STATUS_PENDING)
                    _pageHeadBuilder.AddCssFileParts(ResourceLocation.Head, @"~/Plugins/Payments.Square/Content/styles.css", null);

                return new RefundPaymentResult { Errors = new[] { $"Refund is {paymentRefund.Status}" }.ToList() };
            }

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

            //void transaction for current store
            var storeId = _storeContext.CurrentStore.Id;
            var transactionId = voidPaymentRequest.Order.AuthorizationTransactionId;
            var (successfullyCanceled, error) = _squarePaymentManager.CancelPayment(transactionId, storeId);
            if (!successfullyCanceled)
                throw new NopException(error);

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
            return false;
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            //try to get errors
            if (form.TryGetValue(nameof(PaymentInfoModel.Errors), out var errorsString) && !StringValues.IsNullOrEmpty(errorsString))
                return errorsString.ToString().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return new List<string>();
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            var paymentRequest = new ProcessPaymentRequest();

            //pass custom values to payment processor
            if (form.TryGetValue(nameof(PaymentInfoModel.Token), out var token) && !StringValues.IsNullOrEmpty(token))
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Square.Fields.Token.Key"), token.ToString());

            if (form.TryGetValue(nameof(PaymentInfoModel.CardNonce), out var cardNonce) && !StringValues.IsNullOrEmpty(cardNonce))
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Square.Fields.CardNonce.Key"), cardNonce.ToString());

            if (form.TryGetValue(nameof(PaymentInfoModel.StoredCardId), out var storedCardId) && !StringValues.IsNullOrEmpty(storedCardId) && !storedCardId.Equals(Guid.Empty.ToString()))
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Square.Fields.StoredCard.Key"), storedCardId.ToString());

            if (form.TryGetValue(nameof(PaymentInfoModel.SaveCard), out var saveCardValue) && !StringValues.IsNullOrEmpty(saveCardValue) && bool.TryParse(saveCardValue[0], out var saveCard) && saveCard)
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Square.Fields.SaveCard.Key"), saveCard);

            if (form.TryGetValue(nameof(PaymentInfoModel.PostalCode), out var postalCode) && !StringValues.IsNullOrEmpty(postalCode))
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Square.Fields.PostalCode.Key"), postalCode.ToString());

            return paymentRequest;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentSquare/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return SquarePaymentDefaults.VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new SquarePaymentSettings
            {
                LocationId = "0",
                TransactionMode = TransactionMode.Charge,
                UseSandbox = true
            });

            //install renew access token schedule task
            if (_scheduleTaskService.GetTaskByType(SquarePaymentDefaults.RenewAccessTokenTask) == null)
            {
                _scheduleTaskService.InsertTask(new ScheduleTask
                {
                    Enabled = true,
                    Seconds = SquarePaymentDefaults.AccessTokenRenewalPeriodRecommended * 24 * 60 * 60,
                    Name = SquarePaymentDefaults.RenewAccessTokenTaskName,
                    Type = SquarePaymentDefaults.RenewAccessTokenTask,
                });
            }

            //locales
            _localizationService.AddPluginLocaleResource(new Dictionary<string, string>
            {
                ["Enums.Nop.Plugin.Payments.Square.Domain.TransactionMode.Authorize"] = "Authorize only",
                ["Enums.Nop.Plugin.Payments.Square.Domain.TransactionMode.Charge"] = "Charge (authorize and capture)",
                ["Plugins.Payments.Square.AccessTokenRenewalPeriod.Error"] = "Token renewal limit to {0} days max, but it is recommended that you specify {1} days for the period",
                ["Plugins.Payments.Square.Fields.AccessToken"] = "Access token",
                ["Plugins.Payments.Square.Fields.AccessToken.Hint"] = "Get the automatically renewed OAuth access token by pressing button 'Obtain access token'.",
                ["Plugins.Payments.Square.Fields.AdditionalFee"] = "Additional fee",
                ["Plugins.Payments.Square.Fields.AdditionalFee.Hint"] = "Enter additional fee to charge your customers.",
                ["Plugins.Payments.Square.Fields.AdditionalFeePercentage"] = "Additional fee. Use percentage",
                ["Plugins.Payments.Square.Fields.AdditionalFeePercentage.Hint"] = "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.",
                ["Plugins.Payments.Square.Fields.ApplicationId"] = "Application ID",
                ["Plugins.Payments.Square.Fields.ApplicationId.Hint"] = "Enter your application ID, available from the application dashboard.",
                ["Plugins.Payments.Square.Fields.ApplicationSecret"] = "Application secret",
                ["Plugins.Payments.Square.Fields.ApplicationSecret.Hint"] = "Enter your application secret, available from the application dashboard.",
                ["Plugins.Payments.Square.Fields.CardNonce.Key"] = "Pay using card nonce",
                ["Plugins.Payments.Square.Fields.Location"] = "Business location",
                ["Plugins.Payments.Square.Fields.Location.Hint"] = "Choose your business location. Location is a required parameter for payment requests.",
                ["Plugins.Payments.Square.Fields.Location.NotExist"] = "No locations",
                ["Plugins.Payments.Square.Fields.Location.Select"] = "Select location",
                ["Plugins.Payments.Square.Fields.PostalCode"] = "Postal code",
                ["Plugins.Payments.Square.Fields.PostalCode.Key"] = "Postal code",
                ["Plugins.Payments.Square.Fields.SandboxAccessToken"] = "Sandbox access token",
                ["Plugins.Payments.Square.Fields.SandboxAccessToken.Hint"] = "Enter your sandbox access token, available from the application dashboard.",
                ["Plugins.Payments.Square.Fields.SandboxApplicationId"] = "Sandbox application ID",
                ["Plugins.Payments.Square.Fields.SandboxApplicationId.Hint"] = "Enter your sandbox application ID, available from the application dashboard.",
                ["Plugins.Payments.Square.Fields.SaveCard"] = "Save the card data for future purchasing",
                ["Plugins.Payments.Square.Fields.SaveCard.Key"] = "Save card details",
                ["Plugins.Payments.Square.Fields.StoredCard"] = "Use a previously saved card",
                ["Plugins.Payments.Square.Fields.StoredCard.Key"] = "Pay using stored card token",
                ["Plugins.Payments.Square.Fields.StoredCard.Mask"] = "*{0}",
                ["Plugins.Payments.Square.Fields.StoredCard.SelectCard"] = "Select a card",
                ["Plugins.Payments.Square.Fields.Token.Key"] = "Verification token",
                ["Plugins.Payments.Square.Fields.TransactionMode"] = "Transaction mode",
                ["Plugins.Payments.Square.Fields.TransactionMode.Hint"] = "Choose the transaction mode.",
                ["Plugins.Payments.Square.Fields.Use3ds"] = "Use 3D-Secure",
                ["Plugins.Payments.Square.Fields.Use3ds.Hint"] = "Determine whether to use 3D-Secure feature. Used for Strong customer authentication (SCA). SCA is generally friction-free for the buyer, but a card-issuing bank may require additional authentication for some payments. In those cases, the buyer must verify their identiy with the bank using an additional secure dialog.",
                ["Plugins.Payments.Square.Fields.UseSandbox"] = "Use sandbox",
                ["Plugins.Payments.Square.Fields.UseSandbox.Hint"] = "Determine whether to use sandbox credentials.",
                ["Plugins.Payments.Square.Instructions"] = @"
                    <div style=""margin: 0 0 10px;"">
                        <em><b>Warning: Square sandbox data has been changed. For more information visit our <a href=""https://docs.nopcommerce.com/user-guide/configuring/settingup/payments/methods/square.html"" target=""_blank"">documentation</a>.</em></b><br />
                        <br />
                        For plugin configuration, follow these steps:<br />
                        <br />
                        1. You will need a Square Merchant account. If you don't already have one, you can sign up here: <a href=""http://squ.re/nopcommerce"" target=""_blank"">https://squareup.com/signup/</a><br />
                        2. Sign in to 'Square Merchant Dashboard'. Go to 'Account & Settings' &#8594; 'Locations' tab and create new location.<br />
                        <em>   Important: Your merchant account must have at least one location with enabled credit card processing. Please refer to the Square customer support if you have any questions about how to set this up.</em><br />
                        3. Sign in to your 'Square Developer Dashboard' at <a href=""http://squ.re/nopcommerce1"" target=""_blank"">https://connect.squareup.com/apps</a>; use the same login credentials as your merchant account.<br />
                        4. Click on 'Create Your First Application' and fill in the 'Application Name'. This name is for you to recognize the application in the developer portal and is not used by the plugin. Click 'Create Application' at the bottom of the page.<br />
                        5. Now you are on the details page of the previously created application. On the 'Credentials' tab click on the 'Change Version' button and choose '2019-09-25'.<br />
                        6. Make sure you uncheck 'Use sandbox' below.<br />
                        7. In the 'Square Developer Dashboard' go to the details page of the your previously created application:
                           <ul>
                              <li>On the 'Credentials' tab make sure the 'Application mode' setting value is 'Production'</li>
                              <li>On the 'Credentials' tab copy the 'Application ID' and paste it into 'Application ID' below</li>
                              <li>Go to 'OAuth' tab. Click 'Show' on the 'Application Secret' field. Copy the 'Application Secret' and paste it into 'Application Secret' below</li>
                              <li>Copy this URL: <em>{0}</em>. On the 'OAuth' tab paste this URL into 'Redirect URL'. Click 'Save'</li>
                           </ul>
                        8. Click 'Save' below to save the plugin configuration.<br />
                        9. Click 'Obtain access token' below; the Access token field should populate.<br />
                        <em>Note: If for whatever reason you would like to disable an access to your accounts, simply 'Revoke access tokens' below.</em><br />
                        10. Choose the previously created location. 'Location' is a required parameter for payment requests.<br />
                        11. Fill in the remaining fields and click 'Save' to complete the configuration.<br />
                        <br />
                        <em>Note: The payment form must be generated only on a webpage that uses HTTPS.</em><br />
                    </div>",
                ["Plugins.Payments.Square.ObtainAccessToken"] = "Obtain access token",
                ["Plugins.Payments.Square.ObtainAccessToken.Error"] = "An error occurred while obtaining an access token",
                ["Plugins.Payments.Square.ObtainAccessToken.Success"] = "The access token was successfully obtained",
                ["Plugins.Payments.Square.PaymentMethodDescription"] = "Pay by credit card using Square",
                ["Plugins.Payments.Square.RenewAccessToken.Error"] = "Square payment error. An error occurred while renewing an access token",
                ["Plugins.Payments.Square.RenewAccessToken.Success"] = "Square payment info. The access token was successfully renewed",
                ["Plugins.Payments.Square.RevokeAccessTokens"] = "Revoke access tokens",
                ["Plugins.Payments.Square.RevokeAccessTokens.Error"] = "An error occurred while revoking access tokens",
                ["Plugins.Payments.Square.RevokeAccessTokens.Success"] = "All access tokens were successfully revoked"
            });

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<SquarePaymentSettings>();

            //remove scheduled task
            var task = _scheduleTaskService.GetTaskByType(SquarePaymentDefaults.RenewAccessTokenTask);
            if (task != null)
                _scheduleTaskService.DeleteTask(task);

            //locales
            _localizationService.DeletePluginLocaleResources("Enums.Nop.Plugin.Payments.Square");
            _localizationService.DeletePluginLocaleResources("Plugins.Payments.Square");

            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture => true;

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund => true;

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund => true;

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid => true;

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.Manual;

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo => false;

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public string PaymentMethodDescription => _localizationService.GetResource("Plugins.Payments.Square.PaymentMethodDescription");

        #endregion
    }
}