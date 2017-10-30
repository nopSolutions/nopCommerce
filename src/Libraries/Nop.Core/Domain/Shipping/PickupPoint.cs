using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Pickup point
    /// </summary>
    public partial class PickupPoint
    {
        /// <summary>
        /// Gets or sets an identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a system name of the pickup point provider
        /// </summary>
        public string ProviderSystemName { get; set; }

        /// <summary>
        /// Gets or sets an address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets a city
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets a state abbreviation
        /// </summary>
        public string StateAbbreviation { get; set; }

        /// <summary>
        /// Gets or sets a two-letter ISO country code
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets a zip postal code
        /// </summary>
        public string ZipPostalCode { get; set; }

        /// <summary>
        /// Gets or sets a latitude
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Gets or sets a longitude
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Gets or sets a fee for the pickup
        /// </summary>
        public decimal PickupFee { get; set; }

        /// <summary>
        /// Gets or sets an opening hours
        /// </summary>
        public string OpeningHours { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// Type converter for "PickupPoint"
    /// </summary>
    public class PickupPointTypeConverter : TypeConverter
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
            if (value is string)
            {
                PickupPoint pickupPoint = null;
                var valueStr = value as string;
                if (!string.IsNullOrEmpty(valueStr))
                {
                    try
                    {
                        using (var tr = new StringReader(valueStr))
                        {
                            pickupPoint = (PickupPoint)(new XmlSerializer(typeof(PickupPoint)).Deserialize(tr));
                        }
                    }
                    catch { }
                }

                return pickupPoint;
            }

            return base.ConvertFrom(context, culture, value);
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
            if (destinationType == typeof(string))
            {
                var pickupPoint = value as PickupPoint;
                if (pickupPoint != null)
                {
                    var sb = new StringBuilder();
                    using (var tw = new StringWriter(sb))
                    {
                        new XmlSerializer(typeof(PickupPoint)).Serialize(tw, value);

                        return sb.ToString();
                    }
                }

                return string.Empty;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
