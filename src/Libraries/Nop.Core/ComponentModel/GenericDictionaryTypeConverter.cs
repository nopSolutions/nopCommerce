using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Nop.Core.ComponentModel
{
    /// <summary>
    /// Generic Dictionary type converted
    /// </summary>
    /// <typeparam name="K">Key type (simple)</typeparam>
    /// <typeparam name="V">Value type (simple)</typeparam>
    public class GenericDictionaryTypeConverter<K, V> : TypeConverter
    {
        /// <summary>
        /// Type converter
        /// </summary>
        protected readonly TypeConverter typeConverterKey;

        /// <summary>
        /// Type converter
        /// </summary>
        protected readonly TypeConverter typeConverterValue;

        public GenericDictionaryTypeConverter()
        {
            typeConverterKey = TypeDescriptor.GetConverter(typeof(K));
            if (typeConverterKey == null)
                throw new InvalidOperationException("No type converter exists for type " + typeof(K).FullName);
            typeConverterValue = TypeDescriptor.GetConverter(typeof(V));
            if (typeConverterValue == null)
                throw new InvalidOperationException("No type converter exists for type " + typeof(V).FullName);
        }

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

            var input = (string)value;
            var items = string.IsNullOrEmpty(input) ? Array.Empty<string>() : input.Split(';').Select(x => x.Trim()).ToArray();

            var result = new Dictionary<K, V>();
            Array.ForEach(items, s =>
            {
                var keyValueStr = string.IsNullOrEmpty(s) ? Array.Empty<string>() : s.Split(',').Select(x => x.Trim()).ToArray();
                if (keyValueStr.Length != 2)
                    return;

                object dictionaryKey = (K)typeConverterKey.ConvertFromInvariantString(keyValueStr[0]);
                object dictionaryValue = (V)typeConverterValue.ConvertFromInvariantString(keyValueStr[1]);
                if (dictionaryKey == null || dictionaryValue == null)
                    return;

                if (!result.ContainsKey((K)dictionaryKey))
                    result.Add((K)dictionaryKey, (V)dictionaryValue);
            });

            return result;
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

            var result = string.Empty;
            if (value == null)
                return result;

            //we don't use string.Join() because it doesn't support invariant culture
            var counter = 0;
            var dictionary = (IDictionary<K, V>)value;
            foreach (var keyValue in dictionary)
            {
                result += $"{Convert.ToString(keyValue.Key, CultureInfo.InvariantCulture)}, {Convert.ToString(keyValue.Value, CultureInfo.InvariantCulture)}";
                //don't add ; after the last element
                if (counter != dictionary.Count - 1)
                    result += ";";
                counter++;
            }

            return result;
        }
    }
}