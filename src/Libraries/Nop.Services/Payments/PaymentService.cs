

using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Payment service
    /// </summary>
    public partial class PaymentService : IPaymentService
    {
        #region Fields

        private readonly PaymentSettings _paymentSettings;
        private readonly IPluginFinder _pluginFinder;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="paymentSettings">Payment settings</param>
        /// <param name="pluginFinder">Plugin finder</param>
        public PaymentService(PaymentSettings paymentSettings, IPluginFinder pluginFinder)
        {
            this._paymentSettings = paymentSettings;
            this._pluginFinder = pluginFinder;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active payment methods
        /// </summary>
        /// <returns>Payment methods</returns>
        public IList<IPaymentMethod> LoadActivePaymentMethods()
        {
            var systemNames = _paymentSettings.ActivePaymentMethodSystemNames;
            var providers = new List<IPaymentMethod>();
            foreach (var systemName in systemNames)
            {
                var provider = LoadPaymentMethodBySystemName(systemName);
                providers.Add(provider);
            }
            return providers;
        }

        /// <summary>
        /// Load payment provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found payment provider</returns>
        public IPaymentMethod LoadPaymentMethodBySystemName(string systemName)
        {
            var providers = LoadAllPaymentMethods();
            var provider = providers.SingleOrDefault(p => p.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
            return provider;
        }

        /// <summary>
        /// Load all payment providers
        /// </summary>
        /// <returns>Payment providers</returns>
        public IList<IPaymentMethod> LoadAllPaymentMethods()
        {
            var providers = _pluginFinder.GetPlugins<IPaymentMethod>();
            return providers.OrderBy(tp => tp.FriendlyName).ToList();
        }



        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest.OrderTotal == decimal.Zero)
            {
                var result = new ProcessPaymentResult()
                {
                    NewPaymentStatus = PaymentStatus.Paid
                };
                return result;
            }
            else
            {
                var paymentMethod = LoadPaymentMethodBySystemName(processPaymentRequest.PaymentMethodSystemName);
                if (paymentMethod == null)
                    throw new NopException("Payment method couldn't be loaded");
                return paymentMethod.ProcessPayment(processPaymentRequest);
            }
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //already paid or order.OrderTotal == decimal.Zero
            if (postProcessPaymentRequest.Order.PaymentStatus == PaymentStatus.Paid)
                return;

            var paymentMethod = LoadPaymentMethodBySystemName(postProcessPaymentRequest.Order.PaymentMethodSystemName);
            if (paymentMethod == null)
                throw new NopException("Payment method couldn't be loaded");
            paymentMethod.PostProcessPayment(postProcessPaymentRequest);
        }



        /// <summary>
        /// Gets an additional handling fee of a payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(string paymentMethodSystemName)
        {
            var paymentMethod = LoadPaymentMethodBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return decimal.Zero;

            decimal result = paymentMethod.GetAdditionalHandlingFee();
            if (result < decimal.Zero)
                result = decimal.Zero;
            result = Math.Round(result, 2);
            return result;
        }



        /// <summary>
        /// Gets a value indicating whether capture is supported by payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A value indicating whether capture is supported</returns>
        public bool CanCapture(string paymentMethodSystemName)
        {
            var paymentMethod = LoadPaymentMethodBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return false;
            return paymentMethod.CanCapture;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var paymentMethod = LoadPaymentMethodBySystemName(capturePaymentRequest.Order.PaymentMethodSystemName);
            if (paymentMethod == null)
                throw new NopException("Payment method couldn't be loaded");
            return paymentMethod.Capture(capturePaymentRequest);
        }



        /// <summary>
        /// Gets a value indicating whether partial refund is supported by payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A value indicating whether partial refund is supported</returns>
        public bool CanPartiallyRefund(string paymentMethodSystemName)
        {
            var paymentMethod = LoadPaymentMethodBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return false;
            return paymentMethod.CanPartiallyRefund;
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported by payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A value indicating whether refund is supported</returns>
        public bool CanRefund(string paymentMethodSystemName)
        {
            var paymentMethod = LoadPaymentMethodBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return false;
            return paymentMethod.CanRefund;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var paymentMethod = LoadPaymentMethodBySystemName(refundPaymentRequest.Order.PaymentMethodSystemName);
            if (paymentMethod == null)
                throw new NopException("Payment method couldn't be loaded");
            return paymentMethod.Refund(refundPaymentRequest);
        }
        


        /// <summary>
        /// Gets a value indicating whether void is supported by payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A value indicating whether void is supported</returns>
        public bool CanVoid(string paymentMethodSystemName)
        {
            var paymentMethod = LoadPaymentMethodBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return false;
            return paymentMethod.CanVoid;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var paymentMethod = LoadPaymentMethodBySystemName(voidPaymentRequest.Order.PaymentMethodSystemName);
            if (paymentMethod == null)
                throw new NopException("Payment method couldn't be loaded");
            return paymentMethod.Void(voidPaymentRequest);
        }



        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A recurring payment type of payment method</returns>
        public RecurringPaymentType SupportRecurringPayments(string paymentMethodSystemName)
        {
            var paymentMethod = LoadPaymentMethodBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return RecurringPaymentType.NotSupported;
            return paymentMethod.SupportRecurringPayments;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest.OrderTotal == decimal.Zero)
            {
                var result = new ProcessPaymentResult()
                {
                    NewPaymentStatus = PaymentStatus.Paid
                };
                return result;
            }
            else
            {
                var paymentMethod = LoadPaymentMethodBySystemName(processPaymentRequest.PaymentMethodSystemName);
                if (paymentMethod == null)
                    throw new NopException("Payment method couldn't be loaded");
                return paymentMethod.ProcessRecurringPayment(processPaymentRequest);
            }
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            if (cancelPaymentRequest.Order.OrderTotal == decimal.Zero)
                return new CancelRecurringPaymentResult();

            var paymentMethod = LoadPaymentMethodBySystemName(cancelPaymentRequest.Order.PaymentMethodSystemName);
            if (paymentMethod == null)
                throw new NopException("Payment method couldn't be loaded");
            return paymentMethod.CancelRecurringPayment(cancelPaymentRequest);
        }



        /// <summary>
        /// Gets a payment method type
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A payment method type</returns>
        public PaymentMethodType GetPaymentMethodType(string paymentMethodSystemName)
        {
            var paymentMethod = LoadPaymentMethodBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return PaymentMethodType.Unknown;
            return paymentMethod.PaymentMethodType;
        }

        /// <summary>
        /// Gets masked credit card number
        /// </summary>
        /// <param name="creditCardNumber">Credit card number</param>
        /// <returns>Masked credit card number</returns>
        public string GetMaskedCreditCardNumber(string creditCardNumber)
        {
            if (String.IsNullOrEmpty(creditCardNumber))
                return string.Empty;

            if (creditCardNumber.Length <= 4)
                return creditCardNumber;

            string last4 = creditCardNumber.Substring(creditCardNumber.Length - 4, 4);
            string maskedChars = string.Empty;
            for (int i = 0; i < creditCardNumber.Length - 4; i++)
            {
                maskedChars += "*";
            }
            return maskedChars + last4;
        }
        
        #endregion
    }
}
