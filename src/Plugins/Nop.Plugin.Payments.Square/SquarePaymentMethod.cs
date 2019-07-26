using System;
using System.Collections.Generic;
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
using SquareModel = Square.Connect.Model;

namespace Nop.Plugin.Payments.Square
{
    /// <summary>
    /// Represents Square payment method
    /// </summary>
    public class SquarePaymentMethod : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IPaymentService _paymentService;
        private readonly IPageHeadBuilder _pageHeadBuilder;
        private readonly ISettingService _settingService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IWebHelper _webHelper;
        private readonly SquarePaymentManager _squarePaymentManager;
        private readonly SquarePaymentSettings _squarePaymentSettings;

        #endregion

        #region Ctor

        public SquarePaymentMethod(CurrencySettings currencySettings,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IPaymentService paymentService,
            IPageHeadBuilder pageHeadBuilder,
            ISettingService settingService,
            IScheduleTaskService scheduleTaskService,
            IWebHelper webHelper,
            SquarePaymentManager squarePaymentManager,
            SquarePaymentSettings squarePaymentSettings)
        {
            _currencySettings = currencySettings;
            _currencyService = currencyService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _paymentService = paymentService;
            _pageHeadBuilder = pageHeadBuilder;
            _settingService = settingService;
            _scheduleTaskService = scheduleTaskService;
            _webHelper = webHelper;
            _squarePaymentManager = squarePaymentManager;
            _squarePaymentSettings = squarePaymentSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a payment status by tender card details status
        /// </summary>
        /// <param name="status">Tender card details status</param>
        /// <returns>Payment status</returns>
        private PaymentStatus GetPaymentStatus(SquareModel.TenderCardDetails.StatusEnum? status)
        {
            switch (status)
            {
                case SquareModel.TenderCardDetails.StatusEnum.AUTHORIZED:
                    return PaymentStatus.Authorized;

                case SquareModel.TenderCardDetails.StatusEnum.CAPTURED:
                    return PaymentStatus.Paid;

                case SquareModel.TenderCardDetails.StatusEnum.FAILED:
                    return PaymentStatus.Pending;

                case SquareModel.TenderCardDetails.StatusEnum.VOIDED:
                    return PaymentStatus.Voided;

                default:
                    return PaymentStatus.Pending;
            }
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
            var chargeRequest = CreateChargeRequest(paymentRequest, isRecurringPayment);

            //charge transaction
            var (transaction, error) = _squarePaymentManager.Charge(chargeRequest);
            if (transaction == null)
                throw new NopException(error);

            //get transaction tender
            var tender = transaction.Tenders?.FirstOrDefault();
            if (tender == null)
                throw new NopException("There are no tenders (methods of payment) used to pay in the transaction");

            //get transaction details
            var transactionStatus = tender.CardDetails?.Status;
            var transactionResult = $"Transaction was processed by using {transaction.Product}. Status is {transactionStatus}";

            //return result
            var result = new ProcessPaymentResult
            {
                NewPaymentStatus = GetPaymentStatus(transactionStatus)
            };

            if (_squarePaymentSettings.TransactionMode == TransactionMode.Authorize)
            {
                result.AuthorizationTransactionId = transaction.Id;
                result.AuthorizationTransactionResult = transactionResult;
            }

            if (_squarePaymentSettings.TransactionMode == TransactionMode.Charge)
            {
                result.CaptureTransactionId = transaction.Id;
                result.CaptureTransactionResult = transactionResult;
            }

            return result;
        }

        /// <summary>
        /// Create request parameters to charge transaction
        /// </summary>
        /// <param name="paymentRequest">Payment request parameters</param>
        /// <param name="isRecurringPayment">Whether it is a recurring payment</param>
        /// <returns>Charge request parameters</returns>
        private ExtendedChargeRequest CreateChargeRequest(ProcessPaymentRequest paymentRequest, bool isRecurringPayment)
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
            if (!Enum.TryParse(currency.CurrencyCode, out SquareModel.Money.CurrencyEnum moneyCurrency))
                throw new NopException($"The {currency.CurrencyCode} currency is not supported by the Square");

            //check customer's billing address, shipping address and email, 
            Func<Address, SquareModel.Address> createAddress = (address) => address == null ? null : new SquareModel.Address
            (
                AddressLine1: address.Address1,
                AddressLine2: address.Address2,
                AdministrativeDistrictLevel1: address.StateProvince?.Abbreviation,
                AdministrativeDistrictLevel2: address.County,
                Country: Enum.TryParse(address.Country?.TwoLetterIsoCode, out SquareModel.Address.CountryEnum countryCode)
                    ? (SquareModel.Address.CountryEnum?)countryCode : null,
                FirstName: address.FirstName,
                LastName: address.LastName,
                Locality: address.City,
                PostalCode: address.ZipPostalCode
            );
            var billingAddress = createAddress(customer.BillingAddress);
            var shippingAddress = billingAddress == null ? createAddress(customer.ShippingAddress) : null;
            var email = customer.BillingAddress != null ? customer.BillingAddress.Email : customer.ShippingAddress?.Email;

            //the transaction is ineligible for chargeback protection if they are not provided
            if ((billingAddress == null && shippingAddress == null) || string.IsNullOrEmpty(email))
                _logger.Warning("Square payment warning: Address or email is not provided, so the transaction is ineligible for chargeback protection", customer: customer);

            //the amount of money, in the smallest denomination of the currency indicated by currency. For example, when currency is USD, amount is in cents;
            //most currencies consist of 100 units of smaller denomination, so we multiply the total by 100
            var orderTotal = (int)(paymentRequest.OrderTotal * 100);
            var amountMoney = new SquareModel.Money(Amount: orderTotal, Currency: moneyCurrency);

            //create common charge request parameters
            var chargeRequest = new ExtendedChargeRequest
            (
                amountMoney: amountMoney,
                billingAddress: billingAddress,
                buyerEmailAddress: email,
                delayCapture: _squarePaymentSettings.TransactionMode == TransactionMode.Authorize,
                idempotencyKey: Guid.NewGuid().ToString(),
                integrationId: !string.IsNullOrEmpty(SquarePaymentDefaults.IntegrationId) ? SquarePaymentDefaults.IntegrationId : null,
                note: string.Format(SquarePaymentDefaults.PaymentNote, paymentRequest.OrderGuid),
                referenceId: paymentRequest.OrderGuid.ToString(),
                shippingAddress: shippingAddress
            );

            //try to get previously stored card details
            var storedCardKey = _localizationService.GetResource("Plugins.Payments.Square.Fields.StoredCard.Key");
            if (paymentRequest.CustomValues.TryGetValue(storedCardKey, out var storedCardId) && !storedCardId.ToString().Equals(Guid.Empty.ToString()))
            {
                //check whether customer exists
                var customerId = _genericAttributeService.GetAttribute<string>(customer, SquarePaymentDefaults.CustomerIdAttribute);
                var squareCustomer = _squarePaymentManager.GetCustomer(customerId);
                if (squareCustomer == null)
                    throw new NopException("Failed to retrieve customer");

                //set 'card on file' to charge
                chargeRequest.CustomerId = squareCustomer.Id;
                chargeRequest.CustomerCardId = storedCardId.ToString();
                return chargeRequest;
            }

            //or try to get the card nonce
            var cardNonceKey = _localizationService.GetResource("Plugins.Payments.Square.Fields.CardNonce.Key");
            if (!paymentRequest.CustomValues.TryGetValue(cardNonceKey, out var cardNonce) || string.IsNullOrEmpty(cardNonce?.ToString()))
                throw new NopException("Failed to get the card nonce");

            //remove the card nonce from payment custom values, since it is no longer needed
            paymentRequest.CustomValues.Remove(cardNonceKey);

            //whether to save card details for the future purchasing
            var saveCardKey = _localizationService.GetResource("Plugins.Payments.Square.Fields.SaveCard.Key");
            if (paymentRequest.CustomValues.TryGetValue(saveCardKey, out var saveCardValue) && saveCardValue is bool saveCard && saveCard && !customer.IsGuest())
            {
                //remove the value from payment custom values, since it is no longer needed
                paymentRequest.CustomValues.Remove(saveCardKey);

                try
                {
                    //check whether customer exists
                    var customerId = _genericAttributeService.GetAttribute<string>(customer, SquarePaymentDefaults.CustomerIdAttribute);
                    var squareCustomer = _squarePaymentManager.GetCustomer(customerId);

                    if (squareCustomer == null)
                    {
                        //try to create the new one, if not exists
                        var customerRequest = new SquareModel.CreateCustomerRequest
                        (
                            EmailAddress: customer.Email,
                            Nickname: customer.Username,
                            GivenName: _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute),
                            FamilyName: _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute),
                            PhoneNumber: _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute),
                            CompanyName: _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute),
                            ReferenceId: customer.CustomerGuid.ToString()
                        );
                        squareCustomer = _squarePaymentManager.CreateCustomer(customerRequest);
                        if (squareCustomer == null)
                            throw new NopException("Failed to create customer. Error details in the log");

                        //save customer identifier as generic attribute
                        _genericAttributeService.SaveAttribute(customer, SquarePaymentDefaults.CustomerIdAttribute, squareCustomer.Id);
                    }

                    //create request parameters to create the new card
                    var cardRequest = new SquareModel.CreateCustomerCardRequest
                    (
                        BillingAddress: chargeRequest.BillingAddress ?? chargeRequest.ShippingAddress,
                        CardNonce: cardNonce.ToString()
                    );

                    //set postal code
                    var postalCodeKey = _localizationService.GetResource("Plugins.Payments.Square.Fields.PostalCode.Key");
                    if (paymentRequest.CustomValues.TryGetValue(postalCodeKey, out var postalCode) && !string.IsNullOrEmpty(postalCode.ToString()))
                    {
                        //remove the value from payment custom values, since it is no longer needed
                        paymentRequest.CustomValues.Remove(postalCodeKey);

                        cardRequest.BillingAddress = cardRequest.BillingAddress ?? new SquareModel.Address();
                        cardRequest.BillingAddress.PostalCode = postalCode.ToString();
                    }

                    //try to create card
                    var card = _squarePaymentManager.CreateCustomerCard(squareCustomer.Id, cardRequest);
                    if (card == null)
                        throw new NopException("Failed to create card. Error details in the log");

                    //save card identifier to payment custom values for further purchasing
                    if (isRecurringPayment)
                        paymentRequest.CustomValues.Add(storedCardKey, card.Id);

                    //set 'card on file' to charge
                    chargeRequest.CustomerId = squareCustomer.Id;
                    chargeRequest.CustomerCardId = card.Id;
                    return chargeRequest;
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

            //set 'card nonce' to charge
            chargeRequest.CardNonce = cardNonce.ToString();
            return chargeRequest;
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

            //capture transaction
            var transactionId = capturePaymentRequest.Order.AuthorizationTransactionId;
            var (successfullyCaptured, error) = _squarePaymentManager.CaptureTransaction(transactionId);
            if (!successfullyCaptured)
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
            if (!Enum.TryParse(currency.CurrencyCode, out SquareModel.Money.CurrencyEnum moneyCurrency))
                throw new NopException($"The {currency.CurrencyCode} currency is not supported by the service");

            //the amount of money in the smallest denomination of the currency indicated by currency. For example, when currency is USD, amount is in cents;
            //most currencies consist of 100 units of smaller denomination, so we multiply the total by 100
            var orderTotal = (int)(refundPaymentRequest.AmountToRefund * 100);
            var amountMoney = new SquareModel.Money(Amount: orderTotal, Currency: moneyCurrency);

            //first try to get the transaction
            var transactionId = refundPaymentRequest.Order.CaptureTransactionId;
            var (transaction, transactionError) = _squarePaymentManager.GetTransaction(transactionId);
            if (transaction == null)
                throw new NopException(transactionError);

            //get tender
            var tender = transaction.Tenders?.FirstOrDefault();
            if (tender == null)
                throw new NopException("There are no tenders (methods of payment) used to pay in the transaction");

            //create refund of the transaction
            var refundRequest = new SquareModel.CreateRefundRequest
            (
                AmountMoney: amountMoney,
                IdempotencyKey: Guid.NewGuid().ToString(),
                TenderId: tender.Id
            );
            var (createdRefund, refundError) = _squarePaymentManager.CreateRefund(transactionId, refundRequest);
            if (createdRefund == null)
                throw new NopException(refundError);

            //if refund status is 'pending', try to refund once more with the same request parameters
            if (createdRefund.Status == SquareModel.Refund.StatusEnum.PENDING)
            {
                (createdRefund, refundError) = _squarePaymentManager.CreateRefund(transactionId, refundRequest);
                if (createdRefund == null)
                    throw new NopException(refundError);
            }

            //check whether refund is approved
            if (createdRefund.Status != SquareModel.Refund.StatusEnum.APPROVED)
            {
                //change error notification to warning one (for the pending status)
                if (createdRefund.Status == SquareModel.Refund.StatusEnum.PENDING)
                    _pageHeadBuilder.AddCssFileParts(ResourceLocation.Head, @"~/Plugins/Payments.Square/Content/styles.css", null);

                return new RefundPaymentResult { Errors = new[] { $"Refund is {createdRefund.Status}" }.ToList() };
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

            //void transaction
            var transactionId = voidPaymentRequest.Order.AuthorizationTransactionId;
            var (successfullyVoided, error) = _squarePaymentManager.VoidTransaction(transactionId);
            if (!successfullyVoided)
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
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Payments.Square.Domain.TransactionMode.Authorize", "Authorize only");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Payments.Square.Domain.TransactionMode.Charge", "Charge (authorize and capture)");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.AccessTokenRenewalPeriod.Error", "Token renewal limit to {0} days max, but it is recommended that you specify {1} days for the period");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.AccessToken", "Access token");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.AccessToken.Hint", "Get the automatically renewed OAuth access token by pressing button 'Obtain access token'.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFee", "Additional fee");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationId", "Application ID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationId.Hint", "Enter your application ID, available from the application dashboard.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationSecret", "Application secret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationSecret.Hint", "Enter your application secret, available from the application dashboard.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.CardNonce.Key", "Pay using card nonce");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.Location", "Business location");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.Location.Hint", "Choose your business location. Location is a required parameter for payment requests.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.Location.NotExist", "No locations");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.Location.Select", "Select location");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.PostalCode", "Postal code");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.PostalCode.Key", "Postal code");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxAccessToken", "Sandbox access token");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxAccessToken.Hint", "Enter your sandbox access token, available from the application dashboard.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxApplicationId", "Sandbox application ID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxApplicationId.Hint", "Enter your sandbox application ID, available from the application dashboard.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.SaveCard", "Save the card data for future purchasing");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.SaveCard.Key", "Save card details");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard", "Use a previously saved card");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard.Key", "Pay using stored card token");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard.Mask", "*{0}");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard.SelectCard", "Select a card");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.TransactionMode", "Transaction mode");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.TransactionMode.Hint", "Choose the transaction mode.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.UseSandbox", "Use sandbox");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.UseSandbox.Hint", "Determine whether to use sandbox credentials.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Instructions", @"
                <p>
                    For plugin configuration follow these steps:<br />
                    <br />
                    1. You will need a Square Merchant account. If you don't already have one, you can sign up here: <a href=""http://squ.re/nopcommerce"" target=""_blank"">https://squareup.com/signup/</a><br />
                    <em>Important: Your merchant account must have at least one location with enabled credit card processing. Please refer to the Square customer support if you have any questions about how to set this up.</em><br />
                    2. Sign in to your Square Developer Portal at <a href=""http://squ.re/nopcommerce1"" target=""_blank"">https://connect.squareup.com/apps</a>; use the same sign in credentials as your merchant account.<br />
                    3. Click on '+New Application' and fill in the Application Name. This name is for you to recognize the application in the developer portal and is not used by the extension. Click 'Create Application' at the bottom of the page.<br />
                    4. In the Square Developer admin go to 'Credentials' tab. Copy the Application ID and paste it into Application ID below.<br />
                    5. In the Square Developer admin go to 'OAuth' tab. Click 'Show Secret'. Copy the Application Secret and paste it into Application Secret below. Click 'Save' on this page.<br />
                    6. Copy this URL: <em>{0}</em>. Go to the Square Developer admin, go to 'OAuth' tab, and paste this URL into Redirect URL. Click 'Save'.<br />
                    7. On this page click 'Obtain access token' below; the Access token field should populate. Click 'Save' below.<br />
                    8. Choose the business location. Location is a required parameter for payment requests.<br />
                    9. Fill in the remaining fields and save to complete the configuration.<br />
                    <br />
                    <em>Note: The payment form must be generated only on a webpage that uses HTTPS.</em><br />
                </p>");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.ObtainAccessToken", "Obtain access token");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.ObtainAccessToken.Error", "An error occurred while obtaining an access token");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.ObtainAccessToken.Success", "The access token was successfully obtained");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.PaymentMethodDescription", "Pay by credit card using Square");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.RenewAccessToken.Error", "Square payment error. An error occurred while renewing an access token");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.RenewAccessToken.Success", "Square payment info. The access token was successfully renewed");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.RevokeAccessTokens", "Revoke access tokens");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.RevokeAccessTokens.Error", "An error occurred while revoking access tokens");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.RevokeAccessTokens.Success", "All access tokens were successfully revoked");

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
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Payments.Square.Domain.TransactionMode.Authorize");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Payments.Square.Domain.TransactionMode.Charge");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.AccessTokenRenewalPeriod.Error");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.AccessToken");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.AccessToken.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFee");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFee.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFeePercentage");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFeePercentage.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationId");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationSecret.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.CardNonce.Key");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.Location");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.Location.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.Location.NotExist");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.Location.Select");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.PostalCode");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.PostalCode.Key");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxAccessToken");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxAccessToken.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxApplicationId");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxApplicationId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.SaveCard");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.SaveCard.Key");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard.Key");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard.Mask");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard.SelectCard");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.TransactionMode");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.TransactionMode.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.UseSandbox");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.UseSandbox.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.Instructions");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.ObtainAccessToken");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.ObtainAccessToken.Error");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.ObtainAccessToken.Success");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.PaymentMethodDescription");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.RenewAccessToken.Error");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.RenewAccessToken.Success");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.RevokeAccessTokens");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.RevokeAccessTokens.Error");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Square.RevokeAccessTokens.Success");

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