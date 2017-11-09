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
using Nop.Core.Plugins;
using Nop.Plugin.Payments.Square.Domain;
using Nop.Plugin.Payments.Square.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
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
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
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
            IOrderTotalCalculationService orderTotalCalculationService,
            IPageHeadBuilder pageHeadBuilder,
            ISettingService settingService,
            IScheduleTaskService scheduleTaskService,
            IWebHelper webHelper,
            SquarePaymentManager squarePaymentManager,
            SquarePaymentSettings squarePaymentSettings)
        {
            this._currencySettings = currencySettings;
            this._currencyService = currencyService;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._logger = logger;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._pageHeadBuilder = pageHeadBuilder;
            this._settingService = settingService;
            this._scheduleTaskService = scheduleTaskService;
            this._webHelper = webHelper;
            this._squarePaymentManager = squarePaymentManager;
            this._squarePaymentSettings = squarePaymentSettings;
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
                AmountMoney: amountMoney,
                BillingAddress: billingAddress,
                BuyerEmailAddress: email,
                DelayCapture: _squarePaymentSettings.TransactionMode == TransactionMode.Authorize,
                IdempotencyKey: Guid.NewGuid().ToString(),
                IntegrationId: !string.IsNullOrEmpty(SquarePaymentDefaults.IntegrationId) ? SquarePaymentDefaults.IntegrationId : null,
                Note: string.Format(SquarePaymentDefaults.PaymentNote, paymentRequest.OrderGuid),
                ReferenceId: paymentRequest.OrderGuid.ToString(),
                ShippingAddress: shippingAddress
            );

            //try to get previously stored card details
            var storedCardKey = _localizationService.GetResource("Plugins.Payments.Square.Fields.StoredCard.Key");
            if (paymentRequest.CustomValues.TryGetValue(storedCardKey, out object storedCardId) && !storedCardId.ToString().Equals("0"))
            {
                //check whether customer exists
                var customerId = customer.GetAttribute<string>(SquarePaymentDefaults.CustomerIdAttribute);
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
            if (!paymentRequest.CustomValues.TryGetValue(cardNonceKey, out object cardNonce) || string.IsNullOrEmpty(cardNonce?.ToString()))
                throw new NopException("Failed to get the card nonce");

            //remove the card nonce from payment custom values, since it is no longer needed
            paymentRequest.CustomValues.Remove(cardNonceKey);

            //whether to save card details for the future purchasing
            var saveCardKey = _localizationService.GetResource("Plugins.Payments.Square.Fields.SaveCard.Key");
            if (paymentRequest.CustomValues.TryGetValue(saveCardKey, out object saveCardValue) && saveCardValue is bool saveCard && saveCard && !customer.IsGuest())
            {
                //remove the value from payment custom values, since it is no longer needed
                paymentRequest.CustomValues.Remove(saveCardKey);

                try
                {
                    //check whether customer exists
                    var customerId = customer.GetAttribute<string>(SquarePaymentDefaults.CustomerIdAttribute);
                    var squareCustomer = _squarePaymentManager.GetCustomer(customerId);

                    if (squareCustomer == null)
                    {
                        //try to create the new one, if not exists
                        var customerRequest = new SquareModel.CreateCustomerRequest
                        (
                            EmailAddress: customer.Email,
                            Nickname: customer.Username,
                            GivenName: customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName),
                            FamilyName: customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName),
                            PhoneNumber: customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                            CompanyName: customer.GetAttribute<string>(SystemCustomerAttributeNames.Company),
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
                    if (paymentRequest.CustomValues.TryGetValue(postalCodeKey, out object postalCode) && !string.IsNullOrEmpty(postalCode.ToString()))
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
            var result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
                _squarePaymentSettings.AdditionalFee, _squarePaymentSettings.AdditionalFeePercentage);

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
            //try to get errors
            if (form.TryGetValue("Errors", out StringValues errorsString) && !StringValues.IsNullOrEmpty(errorsString))
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
            if (form.TryGetValue("CardNonce", out StringValues cardNonce) && !StringValues.IsNullOrEmpty(cardNonce))
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Square.Fields.CardNonce.Key"), cardNonce.ToString());

            if (form.TryGetValue("StoredCardId", out StringValues storedCardId) && !StringValues.IsNullOrEmpty(storedCardId) && !storedCardId.Equals("0"))
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Square.Fields.StoredCard.Key"), storedCardId.ToString());

            if (form.TryGetValue("SaveCard", out StringValues saveCardValue) && !StringValues.IsNullOrEmpty(saveCardValue) && bool.TryParse(saveCardValue[0], out bool saveCard) && saveCard)
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Square.Fields.SaveCard.Key"), saveCard);

            if (form.TryGetValue("PostalCode", out StringValues postalCode) && !StringValues.IsNullOrEmpty(postalCode))
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
        /// Gets a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <param name="viewComponentName">View component name</param>
        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "PaymentSquare";
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
            this.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Payments.Square.Domain.TransactionMode.Authorize", "Authorize only");
            this.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Payments.Square.Domain.TransactionMode.Charge", "Charge (authorize and capture)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.AccessTokenRenewalPeriod.Error", "Token renewal limit to {0} days max, but it is recommended that you specify {1} days for the period");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.AccessToken", "Access token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.AccessToken.Hint", "Get the automatically renewed OAuth access token by pressing button 'Obtain access token'.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationId", "Application ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationId.Hint", "Enter your application ID, available from the application dashboard.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationSecret", "Application secret");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationSecret.Hint", "Enter your application secret, available from the application dashboard.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.CardNonce.Key", "Pay using card nonce");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.Location", "Business location");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.Location.Hint", "Choose your business location. Location is a required parameter for payment requests.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.Location.NotExist", "No locations");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.PostalCode", "Postal code");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.PostalCode.Key", "Postal code");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxAccessToken", "Sandbox access token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxAccessToken.Hint", "Enter your sandbox access token, available from the application dashboard.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxApplicationId", "Sandbox application ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxApplicationId.Hint", "Enter your sandbox application ID, available from the application dashboard.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.SaveCard", "Save the card data for future purchasing");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.SaveCard.Key", "Save card details");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard", "Use a previously saved card");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard.Key", "Pay using stored card token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard.Mask", "*{0}");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard.SelectCard", "Select a card");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.TransactionMode", "Transaction mode");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.TransactionMode.Hint", "Choose the transaction mode.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.UseSandbox", "Use sandbox");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Fields.UseSandbox.Hint", "Determine whether to use sandbox credentials.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.Instructions", @"
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
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.ObtainAccessToken", "Obtain access token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.ObtainAccessToken.Error", "An error occurred while obtaining an access token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.ObtainAccessToken.Success", "The access token was successfully obtained");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.PaymentMethodDescription", "Pay by credit card using Square");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.RenewAccessToken.Error", "Square payment error. An error occurred while renewing an access token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.RenewAccessToken.Success", "Square payment info. The access token was successfully renewed");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.RevokeAccessTokens", "Revoke access tokens");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.RevokeAccessTokens.Error", "An error occurred while revoking access tokens");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Square.RevokeAccessTokens.Success", "All access tokens were successfully revoked");
            
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
            this.DeletePluginLocaleResource("Enums.Nop.Plugin.Payments.Square.Domain.TransactionMode.Authorize");
            this.DeletePluginLocaleResource("Enums.Nop.Plugin.Payments.Square.Domain.TransactionMode.Charge");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.AccessTokenRenewalPeriod.Error");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.AccessToken");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.AccessToken.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationId");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationId.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationSecret");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.ApplicationSecret.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.CardNonce.Key");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.Location");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.Location.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.Location.NotExist");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.PostalCode");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.PostalCode.Key");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxAccessToken");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxAccessToken.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxApplicationId");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.SandboxApplicationId.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.SaveCard");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.SaveCard.Key");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard.Key");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard.Mask");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.StoredCard.SelectCard");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.TransactionMode");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.TransactionMode.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.UseSandbox");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Fields.UseSandbox.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.Instructions");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.ObtainAccessToken");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.ObtainAccessToken.Error");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.ObtainAccessToken.Success");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.PaymentMethodDescription");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.RenewAccessToken.Error");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.RenewAccessToken.Success");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.RevokeAccessTokens");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.RevokeAccessTokens.Error");
            this.DeletePluginLocaleResource("Plugins.Payments.Square.RevokeAccessTokens.Success");

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
        public PaymentMethodType PaymentMethodType
        {
            get { return PaymentMethodType.Standard; }
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
            get { return _localizationService.GetResource("Plugins.Payments.Square.PaymentMethodDescription"); }
        }

        #endregion
    }
}