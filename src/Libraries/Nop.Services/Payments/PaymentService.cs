using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Orders;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Payment service
    /// </summary>
    public partial class PaymentService : IPaymentService
    {
        #region Fields

        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly PaymentSettings _paymentSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public PaymentService(IPaymentPluginManager paymentPluginManager,
            PaymentSettings paymentSettings,
            ShoppingCartSettings shoppingCartSettings)
        {
            _paymentPluginManager = paymentPluginManager;
            _paymentSettings = paymentSettings;
            _shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public virtual ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest.OrderTotal == decimal.Zero)
            {
                var result = new ProcessPaymentResult
                {
                    NewPaymentStatus = PaymentStatus.Paid
                };
                return result;
            }

            //We should strip out any white space or dash in the CC number entered.
            if (!string.IsNullOrWhiteSpace(processPaymentRequest.CreditCardNumber))
            {
                processPaymentRequest.CreditCardNumber = processPaymentRequest.CreditCardNumber.Replace(" ", string.Empty);
                processPaymentRequest.CreditCardNumber = processPaymentRequest.CreditCardNumber.Replace("-", string.Empty);
            }

            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(processPaymentRequest.PaymentMethodSystemName)
                ?? throw new NopException("Payment method couldn't be loaded");

            return paymentMethod.ProcessPayment(processPaymentRequest);
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public virtual void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //already paid or order.OrderTotal == decimal.Zero
            if (postProcessPaymentRequest.Order.PaymentStatus == PaymentStatus.Paid)
                return;

            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(postProcessPaymentRequest.Order.PaymentMethodSystemName)
                ?? throw new NopException("Payment method couldn't be loaded");

            paymentMethod.PostProcessPayment(postProcessPaymentRequest);
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public virtual bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!_paymentSettings.AllowRePostingPayments)
                return false;

            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(order.PaymentMethodSystemName);
            if (paymentMethod == null)
                return false; //Payment method couldn't be loaded (for example, was uninstalled)

            if (paymentMethod.PaymentMethodType != PaymentMethodType.Redirection)
                return false;   //this option is available only for redirection payment methods

            if (order.Deleted)
                return false;  //do not allow for deleted orders

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;  //do not allow for cancelled orders

            if (order.PaymentStatus != PaymentStatus.Pending)
                return false;  //payment status should be Pending

            return paymentMethod.CanRePostProcessPayment(order);
        }

        /// <summary>
        /// Gets an additional handling fee of a payment method
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>Additional handling fee</returns>
        public virtual decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart, string paymentMethodSystemName)
        {
            if (string.IsNullOrEmpty(paymentMethodSystemName))
                return decimal.Zero;

            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return decimal.Zero;

            var result = paymentMethod.GetAdditionalHandlingFee(cart);
            if (result < decimal.Zero)
                result = decimal.Zero;

            if (!_shoppingCartSettings.RoundPricesDuringCalculation)
                return result;

            var priceCalculationService = EngineContext.Current.Resolve<IPriceCalculationService>();
            result = priceCalculationService.RoundPrice(result);

            return result;
        }

        /// <summary>
        /// Gets a value indicating whether capture is supported by payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A value indicating whether capture is supported</returns>
        public virtual bool SupportCapture(string paymentMethodSystemName)
        {
            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return false;
            return paymentMethod.SupportCapture;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public virtual CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(capturePaymentRequest.Order.PaymentMethodSystemName)
                ?? throw new NopException("Payment method couldn't be loaded");

            return paymentMethod.Capture(capturePaymentRequest);
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported by payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A value indicating whether partial refund is supported</returns>
        public virtual bool SupportPartiallyRefund(string paymentMethodSystemName)
        {
            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return false;
            return paymentMethod.SupportPartiallyRefund;
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported by payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A value indicating whether refund is supported</returns>
        public virtual bool SupportRefund(string paymentMethodSystemName)
        {
            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return false;
            return paymentMethod.SupportRefund;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public virtual RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(refundPaymentRequest.Order.PaymentMethodSystemName)
                ?? throw new NopException("Payment method couldn't be loaded");

            return paymentMethod.Refund(refundPaymentRequest);
        }

        /// <summary>
        /// Gets a value indicating whether void is supported by payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A value indicating whether void is supported</returns>
        public virtual bool SupportVoid(string paymentMethodSystemName)
        {
            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return false;
            return paymentMethod.SupportVoid;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public virtual VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(voidPaymentRequest.Order.PaymentMethodSystemName)
                ?? throw new NopException("Payment method couldn't be loaded");

            return paymentMethod.Void(voidPaymentRequest);
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A recurring payment type of payment method</returns>
        public virtual RecurringPaymentType GetRecurringPaymentType(string paymentMethodSystemName)
        {
            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return RecurringPaymentType.NotSupported;
            return paymentMethod.RecurringPaymentType;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public virtual ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest.OrderTotal == decimal.Zero)
            {
                var result = new ProcessPaymentResult
                {
                    NewPaymentStatus = PaymentStatus.Paid
                };
                return result;
            }

            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(processPaymentRequest.PaymentMethodSystemName)
                ?? throw new NopException("Payment method couldn't be loaded");

            return paymentMethod.ProcessRecurringPayment(processPaymentRequest);
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public virtual CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            if (cancelPaymentRequest.Order.OrderTotal == decimal.Zero)
                return new CancelRecurringPaymentResult();

            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(cancelPaymentRequest.Order.PaymentMethodSystemName)
                ?? throw new NopException("Payment method couldn't be loaded");

            return paymentMethod.CancelRecurringPayment(cancelPaymentRequest);
        }

        /// <summary>
        /// Gets masked credit card number
        /// </summary>
        /// <param name="creditCardNumber">Credit card number</param>
        /// <returns>Masked credit card number</returns>
        public virtual string GetMaskedCreditCardNumber(string creditCardNumber)
        {
            if (string.IsNullOrEmpty(creditCardNumber))
                return string.Empty;

            if (creditCardNumber.Length <= 4)
                return creditCardNumber;

            var last4 = creditCardNumber.Substring(creditCardNumber.Length - 4, 4);
            var maskedChars = string.Empty;
            for (var i = 0; i < creditCardNumber.Length - 4; i++)
            {
                maskedChars += "*";
            }

            return maskedChars + last4;
        }

        /// <summary>
        /// Calculate payment method fee
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="fee">Fee value</param>
        /// <param name="usePercentage">Is fee amount specified as percentage or fixed value?</param>
        /// <returns>Result</returns>
        public virtual decimal CalculateAdditionalFee(IList<ShoppingCartItem> cart, decimal fee, bool usePercentage)
        {
            if (fee <= 0)
                return fee;

            decimal result;
            if (usePercentage)
            {
                //percentage
                var orderTotalCalculationService = EngineContext.Current.Resolve<IOrderTotalCalculationService>();
                var orderTotalWithoutPaymentFee = orderTotalCalculationService.GetShoppingCartTotal(cart, usePaymentMethodAdditionalFee: false);
                result = (decimal)((float)orderTotalWithoutPaymentFee * (float)fee / 100f);
            }
            else
            {
                //fixed value
                result = fee;
            }

            return result;
        }

        /// <summary>
        /// Serialize CustomValues of ProcessPaymentRequest
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Serialized CustomValues</returns>
        public virtual string SerializeCustomValues(ProcessPaymentRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!request.CustomValues.Any())
                return null;

            //XmlSerializer won't serialize objects that implement IDictionary by default.
            //http://msdn.microsoft.com/en-us/magazine/cc164135.aspx 

            //also see http://ropox.ru/tag/ixmlserializable/ (Russian language)

            var ds = new DictionarySerializer(request.CustomValues);
            var xs = new XmlSerializer(typeof(DictionarySerializer));

            using (var textWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(textWriter))
                {
                    xs.Serialize(xmlWriter, ds);
                }

                var result = textWriter.ToString();
                return result;
            }
        }

        /// <summary>
        /// Deserialize CustomValues of Order
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Serialized CustomValues CustomValues</returns>
        public virtual Dictionary<string, object> DeserializeCustomValues(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (string.IsNullOrWhiteSpace(order.CustomValuesXml))
                return new Dictionary<string, object>();

            var serializer = new XmlSerializer(typeof(DictionarySerializer));

            using (var textReader = new StringReader(order.CustomValuesXml))
            {
                using (var xmlReader = XmlReader.Create(textReader))
                {
                    if (serializer.Deserialize(xmlReader) is DictionarySerializer ds)
                        return ds.Dictionary;
                    return new Dictionary<string, object>();
                }
            }
        }

        #endregion
    }
}