using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Qualpay.Domain;
using Nop.Plugin.Payments.Qualpay.Models;
using Nop.Plugin.Payments.Qualpay.Services;
using Nop.Plugin.Payments.Qualpay.Validators;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.Qualpay
{
    /// <summary>
    /// Represents Qualpay payment gateway processor
    /// </summary>
    public class QualpayProcessor : BasePlugin, IPaymentMethod, IWidgetPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPaymentService _paymentService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly QualpayManager _qualpayManager;
        private readonly QualpaySettings _qualpaySettings;

        #endregion

        #region Ctor

        public QualpayProcessor(ILocalizationService localizationService,
            IPaymentService paymentService,
            ISettingService settingService,
            IWebHelper webHelper,
            QualpayManager qualpayManager,
            QualpaySettings qualpaySettings)
        {
            _localizationService = localizationService;
            _paymentService = paymentService;
            _settingService = settingService;
            _webHelper = webHelper;
            _qualpayManager = qualpayManager;
            _qualpaySettings = qualpaySettings;
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
            //get response
            var (response, error) =
                _qualpaySettings.PaymentTransactionType == TransactionType.Authorization ? _qualpayManager.Authorize(processPaymentRequest) :
                _qualpaySettings.PaymentTransactionType == TransactionType.Sale ? _qualpayManager.Sale(processPaymentRequest) :
                throw new ArgumentException("Transaction type is not supported", nameof(_qualpaySettings.PaymentTransactionType));

            if (!string.IsNullOrEmpty(error))
                return new ProcessPaymentResult { Errors = new[] { error } };

            //request succeeded
            var result = new ProcessPaymentResult
            {
                AvsResult = response.AuthAvsResult,
                Cvv2Result = response.AuthCvv2Result,
                AuthorizationTransactionCode = response.AuthCode
            };

            //set an authorization details
            if (_qualpaySettings.PaymentTransactionType == TransactionType.Authorization)
            {
                result.AuthorizationTransactionId = response.PgId;
                result.AuthorizationTransactionResult = response.Rmsg;
                result.NewPaymentStatus = PaymentStatus.Authorized;
            }

            //or set a capture details
            if (_qualpaySettings.PaymentTransactionType == TransactionType.Sale)
            {
                result.CaptureTransactionId = response.PgId;
                result.CaptureTransactionResult = response.Rmsg;
                result.NewPaymentStatus = PaymentStatus.Paid;
            }

            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //nothing
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return _paymentService.CalculateAdditionalFee(cart,
                _qualpaySettings.AdditionalFee, _qualpaySettings.AdditionalFeePercentage);
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            //capture full amount of the authorized transaction
            var (response, error) = _qualpayManager.CaptureTransaction(capturePaymentRequest.Order.AuthorizationTransactionId,
                Math.Round(capturePaymentRequest.Order.OrderTotal, 2));

            if (!string.IsNullOrEmpty(error))
                return new CapturePaymentResult { Errors = new[] { error } };

            //request succeeded
            return new CapturePaymentResult
            {
                CaptureTransactionId = response.PgId,
                CaptureTransactionResult = response.Rmsg,
                NewPaymentStatus = PaymentStatus.Paid
            };
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            //refund full or partial amount of the captured transaction
            var (response, error) = _qualpayManager.Refund(refundPaymentRequest.Order.CaptureTransactionId,
                Math.Round(refundPaymentRequest.AmountToRefund, 2));

            if (!string.IsNullOrEmpty(error))
                return new RefundPaymentResult { Errors = new[] { error } };

            //request succeeded
            return new RefundPaymentResult
            {
                NewPaymentStatus = refundPaymentRequest.IsPartialRefund
                    ? PaymentStatus.PartiallyRefunded
                    : PaymentStatus.Refunded
            };
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            //void full amount of the authorized transaction
            var (response, error) = _qualpayManager.VoidTransaction(voidPaymentRequest.Order.AuthorizationTransactionId);

            if (!string.IsNullOrEmpty(error))
                return new VoidPaymentResult { Errors = new[] { error } };

            //request succeeded
            return new VoidPaymentResult { NewPaymentStatus = PaymentStatus.Voided };
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            //create subscription for recurring billing
            var (subscription, error) = _qualpayManager.CreateSubscription(processPaymentRequest);

            if (!string.IsNullOrEmpty(error))
                return new ProcessPaymentResult { Errors = new[] { error } };

            //request succeeded
            return new ProcessPaymentResult
            {
                SubscriptionTransactionId = subscription.SubscriptionId.ToString(),
                AuthorizationTransactionCode = subscription.Response?.AuthCode,
                AuthorizationTransactionId = subscription.Response?.PgId,
                CaptureTransactionId = subscription.Response?.PgId,
                CaptureTransactionResult = subscription.Response?.Rmsg,
                AuthorizationTransactionResult = subscription.Response?.Rmsg,
                AvsResult = subscription.Response?.AvsResult,
                Cvv2Result = subscription.Response?.Cvv2Result,
                NewPaymentStatus = PaymentStatus.Paid
            };
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            //try to cancel recurring payment
            var (subscription, error) = _qualpayManager.CancelSubscription(cancelPaymentRequest.Order.CustomerId.ToString(),
                cancelPaymentRequest.Order.SubscriptionTransactionId);

            if (!string.IsNullOrEmpty(error))
                return new CancelRecurringPaymentResult { Errors = new[] { error } };

            return new CancelRecurringPaymentResult();
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            return true;
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            if (_qualpaySettings.UseEmbeddedFields)
            {
                //try to get errors from Qualpay card tokenization
                if (form.TryGetValue(nameof(PaymentInfoModel.Errors), out var errorsString) && !StringValues.IsNullOrEmpty(errorsString))
                    return errorsString.ToString().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            else
            {
                //validate payment info (custom validation)
                var validationResult = new PaymentInfoValidator(_localizationService).Validate(new PaymentInfoModel
                {
                    CardholderName = form[nameof(PaymentInfoModel.CardholderName)],
                    CardNumber = form[nameof(PaymentInfoModel.CardNumber)],
                    ExpireMonth = form[nameof(PaymentInfoModel.ExpireMonth)],
                    ExpireYear = form[nameof(PaymentInfoModel.ExpireYear)],
                    CardCode = form[nameof(PaymentInfoModel.CardCode)],
                    BillingCardId = form[nameof(PaymentInfoModel.BillingCardId)],
                    SaveCardDetails = form.TryGetValue(nameof(PaymentInfoModel.SaveCardDetails), out var saveCardDetails)
                        && bool.TryParse(saveCardDetails.FirstOrDefault(), out var saveCard)
                        && saveCard
                });
                if (!validationResult.IsValid)
                    return validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            }

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
                throw new ArgumentNullException(nameof(form));

            var paymentRequest = new ProcessPaymentRequest();

            //pass custom values to payment processor
            var cardId = form[nameof(PaymentInfoModel.BillingCardId)];
            if (!StringValues.IsNullOrEmpty(cardId) && !cardId.FirstOrDefault().Equals(Guid.Empty.ToString()))
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Qualpay.Customer.Card"), cardId.FirstOrDefault());

            var saveCardDetails = form[nameof(PaymentInfoModel.SaveCardDetails)];
            if (!StringValues.IsNullOrEmpty(saveCardDetails) && bool.TryParse(saveCardDetails.FirstOrDefault(), out var saveCard) && saveCard)
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Qualpay.Customer.Card.Save"), true);

            if (_qualpaySettings.UseEmbeddedFields)
            {
                //card details is already validated and tokenized by Qualpay
                var tokenizedCardId = form[nameof(PaymentInfoModel.TokenizedCardId)];
                if (!StringValues.IsNullOrEmpty(tokenizedCardId))
                    paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Qualpay.Customer.Card.Token"), tokenizedCardId.FirstOrDefault());
            }
            else
            {
                //set card details
                paymentRequest.CreditCardName = form[nameof(PaymentInfoModel.CardholderName)];
                paymentRequest.CreditCardNumber = form[nameof(PaymentInfoModel.CardNumber)];
                paymentRequest.CreditCardExpireMonth = int.Parse(form[nameof(PaymentInfoModel.ExpireMonth)]);
                paymentRequest.CreditCardExpireYear = int.Parse(form[nameof(PaymentInfoModel.ExpireYear)]);
                paymentRequest.CreditCardCvv2 = form[nameof(PaymentInfoModel.CardCode)];
            }

            return paymentRequest;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Qualpay/Configure";
        }

        /// <summary>
        /// Gets a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <param name="viewComponentName">View component name</param>
        public string GetPublicViewComponentName()
        {
            return QualpayDefaults.PAYMENT_INFO_VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { AdminWidgetZones.CustomerDetailsBlock };
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return QualpayDefaults.CUSTOMER_VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new QualpaySettings
            {
                UseSandbox = true,
                UseEmbeddedFields = true,
                UseCustomerVault = true,
                PaymentTransactionType = TransactionType.Sale
            });

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Payments.Qualpay.Domain.Authorization", "Authorization");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Payments.Qualpay.Domain.Sale", "Sale (authorization and capture)");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Customer", "Qualpay Vault Customer");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card", "Use a previously saved card");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card.ExpirationDate", "Expiration date");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card.Id", "ID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card.MaskedNumber", "Card number");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card.Save", "Save card data for future purchases");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card.Select", "Select a card");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card.Token", "Use a tokenized card");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card.Type", "Type");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Create", "Add to Vault");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Hint", "Qualpay Vault Customer ID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Customer.NotExists", "The customer is not yet in Qualpay Customer Vault");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.AdditionalFee", "Additional fee");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.AdditionalFeePercentage.Hint", "Determine whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.MerchantEmail", "Email");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.MerchantEmail.Hint", "Enter your email to subscribe to Qualpay news.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.MerchantId", "Merchant ID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.MerchantId.Hint", "Specify your Qualpay merchant identifier.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.MerchantId.Required", "Merchant ID is required if a Security key is present.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.PaymentTransactionType", "Transaction type");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.PaymentTransactionType.Hint", "Choose payment transaction type.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.ProfileId", "Profile ID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.ProfileId.Hint", "Specify your Qualpay profile identifier.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.ProfileId.Required", "Profile ID is required when Qualpay Recurring Billing is enabled.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.SecurityKey", "Security key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.SecurityKey.Hint", "Specify your Qualpay security key.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.SecurityKey.Required", "Security key is required if a Merchant ID is present.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseCustomerVault", "Use Customer Vault");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseCustomerVault.Hint", "Determine whether to use Qualpay Customer Vault feature. The Customer Vault reduces the amount of associated payment data that touches your servers and enables subsequent payment billing information to be fulfilled by Qualpay.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseEmbeddedFields", "Use Embedded Fields");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseEmbeddedFields.Hint", "Determine whether to use Qualpay Embedded Fields feature. Your customer will remain on your website, but payment information is collected and processed on Qualpay servers. Since your server is not processing customer payment data, your PCI DSS compliance scope is greatly reduced.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseEmbeddedFields.TransientKey.Required", "Qualpay Embedded Fields cannot be invoked without a transient key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseRecurringBilling", "Use Recurring Billing");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseRecurringBilling.Hint", "Determine whether to use Qualpay Recurring Billing feature. Support setting your customers up for recurring or subscription payments.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseSandbox", "Use Sandbox");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseSandbox.Hint", "Determine whether to enable sandbox (testing environment).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Fields.Webhook.Warning", "Webhook was not created (you'll not be able to handle recurring payments)");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.PaymentMethodDescription", "Pay by credit / debit card using Qualpay payment gateway");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Subscribe", "Stay informed");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Subscribe.Error", "An error has occurred");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Subscribe.Success", "You have subscribed to Qualpay news");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Qualpay.Unsubscribe.Success", "You have unsubscribed from Qualpay news");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<QualpaySettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Payments.Qualpay.Domain.Authorization");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Payments.Qualpay.Domain.Sale");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Customer");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card.ExpirationDate");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card.Id");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card.MaskedNumber");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card.Save");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card.Select");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card.Token");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Card.Type");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Create");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Customer.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Customer.NotExists");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.AdditionalFee");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.AdditionalFee.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.AdditionalFeePercentage");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.AdditionalFeePercentage.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.MerchantEmail");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.MerchantEmail.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.MerchantEmail.Required");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.MerchantId");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.MerchantId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.PaymentTransactionType");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.PaymentTransactionType.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.ProfileId");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.ProfileId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.ProfileId.Required");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.SecurityKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.SecurityKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.SecurityKey.Required");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseCustomerVault");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseCustomerVault.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseEmbeddedFields");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseEmbeddedFields.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseEmbeddedFields.TransientKey.Required");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseRecurringBilling");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseRecurringBilling.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseSandbox");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.UseSandbox.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Fields.Webhook.Warning");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.PaymentMethodDescription");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Subscribe");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Subscribe.Error");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Subscribe.Success");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Qualpay.Unsubscribe.Success");

            base.Uninstall();
        }

        #endregion

        #region Properies

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
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.Automatic;

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
        public string PaymentMethodDescription => _localizationService.GetResource("Plugins.Payments.Qualpay.PaymentMethodDescription");

        #endregion
    }
}