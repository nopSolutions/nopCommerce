
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using System.IO;
using System.Xml.Serialization;

namespace Nop.Services.Customers
{
    public static class CustomerExtentions
    {
        public static T GetAttribute<T>(this Customer customer,
            string key)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (customer.CustomerAttributes == null)
                customer.CustomerAttributes = new List<CustomerAttribute>();
            var customerAttribute = customer.CustomerAttributes.FirstOrDefault(ca => ca.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (customerAttribute == null)
                return default(T);

            if (String.IsNullOrEmpty(customerAttribute.Value))
            {
                //empty attribute
                return default(T);
            }
            else
            {
                if (!TypeDescriptor.GetConverter(typeof(T)).CanConvertFrom(typeof(string)))
                    throw new NopException("Not supported customer attribute type");
                var attributeValue = (T)(TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(customerAttribute.Value));
                return attributeValue;

                //use the code below in order to support all serializable types (for example, ShippingOption)
                //or use custom TypeConverters like it's implemented for ISettings
                //using (var tr = new StringReader(customerAttribute.Value))
                //{
                //    var xmlS = new XmlSerializer(typeof(T));
                //    var attributeValue = (T)xmlS.Deserialize(tr);
                //    return attributeValue;
                //}

            }
        }
    }
}
