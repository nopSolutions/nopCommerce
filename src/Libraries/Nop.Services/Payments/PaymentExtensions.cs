using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Orders;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Payment extensions
    /// </summary>
    public static class PaymentExtensions
    {
        /// <summary>
        /// Is payment method active?
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <param name="paymentSettings">Payment settings</param>
        /// <returns>Result</returns>
        public static bool IsPaymentMethodActive(this IPaymentMethod paymentMethod,
            PaymentSettings paymentSettings)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException("paymentMethod");

            if (paymentSettings == null)
                throw new ArgumentNullException("paymentSettings");

            if (paymentSettings.ActivePaymentMethodSystemNames == null)
                return false;
            foreach (string activeMethodSystemName in paymentSettings.ActivePaymentMethodSystemNames)
                if (paymentMethod.PluginDescriptor.SystemName.Equals(activeMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }

        /// <summary>
        /// Calculate payment method fee
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <param name="orderTotalCalculationService">Order total calculation service</param>
        /// <param name="cart">Shopping cart</param>
        /// <param name="fee">Fee value</param>
        /// <param name="usePercentage">Is fee amount specified as percentage or fixed value?</param>
        /// <returns>Result</returns>
        public static decimal CalculateAdditionalFee(this IPaymentMethod paymentMethod, 
            IOrderTotalCalculationService orderTotalCalculationService, IList<ShoppingCartItem> cart,
            decimal fee, bool usePercentage)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException("paymentMethod");
            if (fee <= 0)
                return fee;

            decimal result;
            if (usePercentage)
            {
                //percentage
                var orderTotalWithoutPaymentFee = orderTotalCalculationService.GetShoppingCartTotal(cart, usePaymentMethodAdditionalFee: false);
                result = (decimal)((((float)orderTotalWithoutPaymentFee) * ((float)fee)) / 100f);
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
        public static string SerializeCustomValues(this ProcessPaymentRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

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
        /// Deerialize CustomValues of Order
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Serialized CustomValues CustomValues</returns>
        public static Dictionary<string, object> DeserializeCustomValues(this Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var request = new ProcessPaymentRequest();
            return request.DeserializeCustomValues(order.CustomValuesXml);
        }
        /// <summary>
        /// Deerialize CustomValues of ProcessPaymentRequest
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="customValuesXml">Serialized CustomValues</param>
        /// <returns>Serialized CustomValues CustomValues</returns>
        public static Dictionary<string, object> DeserializeCustomValues(this ProcessPaymentRequest request, string customValuesXml)
        {
            if (string.IsNullOrWhiteSpace(customValuesXml))
            {
                return new Dictionary<string, object>();
            }

            var serializer = new XmlSerializer(typeof(DictionarySerializer));

            using (var textReader = new StringReader(customValuesXml))
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
        /// <summary>
        /// Dictonary serializer
        /// </summary>
        public class DictionarySerializer : IXmlSerializable
        {
            public Dictionary<string, object> Dictionary;

            public DictionarySerializer()
            {
                this.Dictionary = new Dictionary<string, object>();
            }

            public DictionarySerializer(Dictionary<string, object> dictionary)
            {
                this.Dictionary = dictionary;
            }

            public void WriteXml(XmlWriter writer)
            {
                if (!Dictionary.Any())
                    return;

                foreach (var key in this.Dictionary.Keys)
                {
                    writer.WriteStartElement("item");
                    writer.WriteElementString("key", key);
                    var value = this.Dictionary[key];
                    //please note that we use ToString() for objects here
                    //of course, we can Serialize them
                    //but let's keep it simple and leave it for developers to handle it
                    //just put required serialization into ToString method of your object(s)
                    //because some objects don't implement ISerializable
                    //the question is how should we deserialize null values?
                    writer.WriteElementString("value", value != null ? value.ToString() : null);
                    writer.WriteEndElement();
                }
            }

            public void ReadXml(XmlReader reader)
            {
                bool wasEmpty = reader.IsEmptyElement;
                reader.Read();
                if (wasEmpty)
                    return;
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.ReadStartElement("item");
                    string key = reader.ReadElementString("key");
                    string value = reader.ReadElementString("value");
                    this.Dictionary.Add(key, value);
                    reader.ReadEndElement();
                    reader.MoveToContent();
                }
                reader.ReadEndElement();
            }

            public XmlSchema GetSchema()
            {
                return null;
            }
        }
    }
}
