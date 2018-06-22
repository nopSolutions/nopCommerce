using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Type converter of list of ShippingOption
    /// </summary>
    public class ShippingOptionListTypeConverter : TypeConverter
    {
        /// <summary>
        /// Gets a value indicating whether this converter can        
        /// convert an object in the given source type to the native type of the converter
        /// using the context.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="sourceType">Source type</param>
        /// <returns>Result</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the converter's native type.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="culture">Culture</param>
        /// <param name="value">Value</param>
        /// <returns>Result</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string)) 
                return base.ConvertFrom(context, culture, value);
            
            var valueStr = value as string;

            if (string.IsNullOrEmpty(valueStr)) 
                return null;

            List<ShippingOption> shippingOptions = null;

            try
            {
                using (var tr = new StringReader(valueStr))
                {
                    var xmlS = new XmlSerializer(typeof(List<ShippingOption>));
                    shippingOptions = (List<ShippingOption>)xmlS.Deserialize(tr);
                }
            }
            catch
            {
                //XML error
            }

            return shippingOptions;
        }

        /// <summary>
        /// Converts the given value object to the specified destination type using the specified context and arguments
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="culture">Culture</param>
        /// <param name="value">Value</param>
        /// <param name="destinationType">Destination type</param>
        /// <returns>Result</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string)) 
                return base.ConvertTo(context, culture, value, destinationType);

            if (!(value is List<ShippingOption>)) 
                return string.Empty;

            var sb = new StringBuilder();
            using (var tw = new StringWriter(sb))
            {
                var xmlS = new XmlSerializer(typeof(List<ShippingOption>));
                xmlS.Serialize(tw, value);
                var serialized = sb.ToString();
                return serialized;
            }
        }
    }
}