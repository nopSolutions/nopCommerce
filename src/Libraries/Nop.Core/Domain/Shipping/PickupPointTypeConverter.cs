using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Type converter for "PickupPoint"
    /// </summary>
    public partial class PickupPointTypeConverter : TypeConverter
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
                return true;

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
            if (value is not string) 
                return base.ConvertFrom(context, culture, value);
            
            var valueStr = value as string;
            if (string.IsNullOrEmpty(valueStr)) 
                return null;

            PickupPoint pickupPoint = null;

            try
            {
                using var tr = new StringReader(valueStr);
                pickupPoint = (PickupPoint)new XmlSerializer(typeof(PickupPoint)).Deserialize(tr);
            }
            catch
            {
                // ignored
            }

            return pickupPoint;
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

            if (value is not PickupPoint) 
                return string.Empty;

            var sb = new StringBuilder();
            using var tw = new StringWriter(sb);
            new XmlSerializer(typeof(PickupPoint)).Serialize(tw, value);

            return sb.ToString();
        }
    }
}