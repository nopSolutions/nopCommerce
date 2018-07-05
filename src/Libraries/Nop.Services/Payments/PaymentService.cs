using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using Nop.Services.Plugins;

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
        private readonly ISettingService _settingService;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public PaymentService(PaymentSettings paymentSettings,
            IPluginFinder pluginFinder,
            ISettingService settingService,
            ShoppingCartSettings shoppingCartSettings)
        {
            this._paymentSettings = paymentSettings;
            this._pluginFinder = pluginFinder;
            this._settingService = settingService;
            this._shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Methods

        #region Payment methods

        /// <summary>
        /// Load active payment methods
        /// </summary>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <param name="filterByCountryId">Load records allowed only in a specified country; pass 0 to load all records</param>
        /// <returns>Payment methods</returns>
        public virtual IList<IPaymentMethod> LoadActivePaymentMethods(Customer customer = null, int storeId = 0, int filterByCountryId = 0)
        {
            return LoadAllPaymentMethods(customer, storeId, filterByCountryId)
                .Where(provider => _paymentSettings.ActivePaymentMethodSystemNames
                    .Contains(provider.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }

        /// <summary>
        /// Load payment provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found payment provider</returns>
        public virtual IPaymentMethod LoadPaymentMethodBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IPaymentMethod>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IPaymentMethod>();

            return null;
        }

        /// <summary>
        /// Load all payment providers
        /// </summary>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <param name="filterByCountryId">Load records allowed only in a specified country; pass 0 to load all records</param>
        /// <returns>Payment providers</returns>
        public virtual IList<IPaymentMethod> LoadAllPaymentMethods(Customer customer = null, int storeId = 0, int filterByCountryId = 0)
        {
            var paymentMethods = _pluginFinder.GetPlugins<IPaymentMethod>(customer: customer, storeId: storeId).ToList();
            if (filterByCountryId == 0)
                return paymentMethods;

            //filter by country
            var paymentMetodsByCountry = new List<IPaymentMethod>();
            foreach (var pm in paymentMethods)
            {
                var restictedCountryIds = GetRestictedCountryIds(pm);
                if (!restictedCountryIds.Contains(filterByCountryId))
                {
                    paymentMetodsByCountry.Add(pm);
                }
            }

            return paymentMetodsByCountry;
        }

        /// <summary>
        /// Is payment method active?
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <returns>Result</returns>
        public virtual bool IsPaymentMethodActive(IPaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException(nameof(paymentMethod));

            if (_paymentSettings.ActivePaymentMethodSystemNames == null)
                return false;

            foreach (var activeMethodSystemName in _paymentSettings.ActivePaymentMethodSystemNames)
                if (paymentMethod.PluginDescriptor.SystemName.Equals(activeMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }

        #endregion

        #region Restrictions

        /// <summary>
        /// Gets a list of country identifiers in which a certain payment method is now allowed
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <returns>A list of country identifiers</returns>
        public virtual IList<int> GetRestictedCountryIds(IPaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException(nameof(paymentMethod));

            var settingKey = $"PaymentMethodRestictions.{paymentMethod.PluginDescriptor.SystemName}";
            var restictedCountryIds = _settingService.GetSettingByKey<List<int>>(settingKey);
            if (restictedCountryIds == null)
                restictedCountryIds = new List<int>();
            return restictedCountryIds;
        }

        /// <summary>
        /// Saves a list of country identifiers in which a certain payment method is now allowed
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <param name="countryIds">A list of country identifiers</param>
        public virtual void SaveRestictedCountryIds(IPaymentMethod paymentMethod, List<int> countryIds)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException(nameof(paymentMethod));

            //we should be sure that countryIds is of type List<int> (not IList<int>)
            var settingKey = $"PaymentMethodRestictions.{paymentMethod.PluginDescriptor.SystemName}";
            _settingService.SetSetting(settingKey, countryIds);
        }

        #endregion

        #region Processing

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
                processPaymentRequest.CreditCardNumber = processPaymentRequest.CreditCardNumber.Replace(" ", "");
                processPaymentRequest.CreditCardNumber = processPaymentRequest.CreditCardNumber.Replace("-", "");
            }
            var paymentMethod = LoadPaymentMethodBySystemName(processPaymentRequest.PaymentMethodSystemName);
            if (paymentMethod == null)
                throw new NopException("Payment method couldn't be loaded");
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

            var paymentMethod = LoadPaymentMethodBySystemName(postProcessPaymentRequest.Order.PaymentMethodSystemName);
            if (paymentMethod == null)
                throw new NopException("Payment method couldn't be loaded");
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

            var paymentMethod = LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
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

            var paymentMethod = LoadPaymentMethodBySystemName(paymentMethodSystemName);
            if (paymentMethod == null)
                return decimal.Zero;

            var result = paymentMethod.GetAdditionalHandlingFee(cart);
            if (result < decimal.Zero)
                result = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                var priceCalculationService = EngineContext.Current.Resolve<IPriceCalculationService>();
                result = priceCalculationService.RoundPrice(result);
            }

            return result;
        }

        /// <summary>
        /// Gets a value indicating whether capture is supported by payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A value indicating whether capture is supported</returns>
        public virtual bool SupportCapture(string paymentMethodSystemName)
        {
            var paymentMethod = LoadPaymentMethodBySystemName(paymentMethodSystemName);
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
        public virtual bool SupportPartiallyRefund(string paymentMethodSystemName)
        {
            var paymentMethod = LoadPaymentMethodBySystemName(paymentMethodSystemName);
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
            var paymentMethod = LoadPaymentMethodBySystemName(paymentMethodSystemName);
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
        public virtual bool SupportVoid(string paymentMethodSystemName)
        {
            var paymentMethod = LoadPaymentMethodBySystemName(paymentMethodSystemName);
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
        public virtual RecurringPaymentType GetRecurringPaymentType(string paymentMethodSystemName)
        {
            var paymentMethod = LoadPaymentMethodBySystemName(paymentMethodSystemName);
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

            var paymentMethod = LoadPaymentMethodBySystemName(processPaymentRequest.PaymentMethodSystemName);
            if (paymentMethod == null)
                throw new NopException("Payment method couldn't be loaded");
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

            var paymentMethod = LoadPaymentMethodBySystemName(cancelPaymentRequest.Order.PaymentMethodSystemName);
            if (paymentMethod == null)
                throw new NopException("Payment method couldn't be loaded");
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
                    var ds = serializer.Deserialize(xmlReader) as DictionarySerializer;
                    if (ds != null)
                        return ds.Dictionary;
                    return new Dictionary<string, object>();
                }
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Dictionary serializer
    /// </summary>
    public class DictionarySerializer : IXmlSerializable
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public DictionarySerializer()
        {
            Dictionary = new Dictionary<string, object>();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="dictionary">Dictionary</param>
        public DictionarySerializer(Dictionary<string, object> dictionary)
        {
            Dictionary = dictionary;
        }

        /// <summary>
        /// Write XML
        /// </summary>
        /// <param name="writer">Writer</param>
        public void WriteXml(XmlWriter writer)
        {
            if (!Dictionary.Any())
                return;

            foreach (var key in Dictionary.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteElementString("key", key);
                var value = Dictionary[key];
                //please note that we use ToString() for objects here
                //of course, we can Serialize them
                //but let's keep it simple and leave it for developers to handle it
                //just put required serialization into ToString method of your object(s)
                //because some objects don't implement ISerializable
                //the question is how should we deserialize null values?
                writer.WriteElementString("value", value?.ToString());
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Read XML
        /// </summary>
        /// <param name="reader">Reader</param>
        public void ReadXml(XmlReader reader)
        {
            var wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                var key = reader.ReadElementString("key");
                var value = reader.ReadElementString("value");
                Dictionary.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        /// <summary>
        /// Get schema
        /// </summary>
        /// <returns>XML schema</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Dictionary
        /// </summary>
        public Dictionary<string, object> Dictionary;
    }
}