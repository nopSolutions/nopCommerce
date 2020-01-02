using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Manual.Models;
using Nop.Plugin.Payments.Manual.Validators;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.Manual
{
    /// <summary>
    /// Manual payment processor
    /// </summary>
    public class ManualPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPaymentService _paymentService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ManualPaymentSettings _manualPaymentSettings;

        #endregion

        #region Ctor

        public ManualPaymentProcessor(ILocalizationService localizationService,
            IPaymentService paymentService,
            ISettingService settingService,
            IWebHelper webHelper,
            ManualPaymentSettings manualPaymentSettings)
        {
            _localizationService = localizationService;
            _paymentService = paymentService;
            _settingService = settingService;
            _webHelper = webHelper;
            _manualPaymentSettings = manualPaymentSettings;
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
            var result = new ProcessPaymentResult
            {
                AllowStoringCreditCardNumber = true
            };
            switch (_manualPaymentSettings.TransactMode)
            {
                case TransactMode.Pending:
                    result.NewPaymentStatus = PaymentStatus.Pending;
                    break;
                case TransactMode.Authorize:
                    result.NewPaymentStatus = PaymentStatus.Authorized;
                    break;
                case TransactMode.AuthorizeAndCapture:
                    result.NewPaymentStatus = PaymentStatus.Paid;
                    break;
                default:
                    result.AddError("Not supported transaction type");
                    break;
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
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return _paymentService.CalculateAdditionalFee(cart,
                _manualPaymentSettings.AdditionalFee, _manualPaymentSettings.AdditionalFeePercentage);
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            return new CapturePaymentResult { Errors = new[] { "Capture method not supported" } };
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            return new RefundPaymentResult { Errors = new[] { "Refund method not supported" } };
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            return new VoidPaymentResult { Errors = new[] { "Void method not supported" } };
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult
            {
                AllowStoringCreditCardNumber = true
            };
            switch (_manualPaymentSettings.TransactMode)
            {
                case TransactMode.Pending:
                    result.NewPaymentStatus = PaymentStatus.Pending;
                    break;
                case TransactMode.Authorize:
                    result.NewPaymentStatus = PaymentStatus.Authorized;
                    break;
                case TransactMode.AuthorizeAndCapture:
                    result.NewPaymentStatus = PaymentStatus.Paid;
                    break;
                default:
                    result.AddError("Not supported transaction type");
                    break;
            }

            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
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
            var warnings = new List<string>();

            //validate
            var validator = new PaymentInfoValidator(_localizationService);
            var model = new PaymentInfoModel
            {
                CardholderName = form["CardholderName"],
                CardNumber = form["CardNumber"],
                CardCode = form["CardCode"],
                ExpireMonth = form["ExpireMonth"],
                ExpireYear = form["ExpireYear"]
            };
            var validationResult = validator.Validate(model);
            if (!validationResult.IsValid)
                warnings.AddRange(validationResult.Errors.Select(error => error.ErrorMessage));

            return warnings;
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest
            {
                CreditCardType = form["CreditCardType"],
                CreditCardName = form["CardholderName"],
                CreditCardNumber = form["CardNumber"],
                CreditCardExpireMonth = int.Parse(form["ExpireMonth"]),
                CreditCardExpireYear = int.Parse(form["ExpireYear"]),
                CreditCardCvv2 = form["CardCode"]
            };
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentManual/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return "PaymentManual";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new ManualPaymentSettings
            {
                TransactMode = TransactMode.Pending
            };
            _settingService.SaveSetting(settings);

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Manual.Instructions", "This payment method stores credit card information in database (it's not sent to any third-party processor). In order to store credit card information, you must be PCI compliant.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Manual.Fields.AdditionalFee", "Additional fee");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Manual.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Manual.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Manual.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Manual.Fields.TransactMode", "After checkout mark payment as");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Manual.Fields.TransactMode.Hint", "Specify transaction mode.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Manual.PaymentMethodDescription", "Pay by credit / debit card");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<ManualPaymentSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Manual.Instructions");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Manual.Fields.AdditionalFee");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Manual.Fields.AdditionalFee.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Manual.Fields.AdditionalFeePercentage");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Manual.Fields.AdditionalFeePercentage.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Manual.Fields.TransactMode");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Manual.Fields.TransactMode.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Manual.PaymentMethodDescription");

            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get { return false; }
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
            get { return _localizationService.GetResource("Plugins.Payments.Manual.PaymentMethodDescription"); }
        }

        #endregion

    }
}