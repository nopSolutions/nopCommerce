using System.ComponentModel;
using System.Globalization;

namespace Nop.Core.ComponentModel;

/// <summary>
/// Generic List type converted
/// </summary>
/// <typeparam name="T">Type</typeparam>
public partial class GenericListTypeConverter<T> : TypeConverter
{
    /// <summary>
    /// Type converter
    /// </summary>
    protected readonly TypeConverter typeConverter;

    public GenericListTypeConverter()
    {
        typeConverter = TypeDescriptor.GetConverter(typeof(T));
        if (typeConverter == null)
            throw new InvalidOperationException("No type converter exists for type " + typeof(T).FullName);
    }

    /// <summary>
    /// Get string array from a comma-separate string
    /// </summary>
    /// <param name="input">Input</param>
    /// <returns>Array</returns>
    protected virtual string[] GetStringArray(string input)
    {
        return string.IsNullOrEmpty(input) ? Array.Empty<string>() : input.Split(',').Select(x => x.Trim()).ToArray();
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
        if (sourceType != typeof(string))
            return base.CanConvertFrom(context, sourceType);

        var items = GetStringArray(sourceType.ToString());
        return items.Any();
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
        if (value is not string && value != null)
            return base.ConvertFrom(context, culture, value);

        var items = GetStringArray((string)value);

        return items.Select(typeConverter.ConvertFromInvariantString)
            .Where(item => item != null)
            .Cast<T>()
            .ToList();
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

        var cultureInvariantStrings = ((IList<T>)value)
            .Select(o => Convert.ToString(o, CultureInfo.InvariantCulture));

        result = string.Join(',', cultureInvariantStrings);

        return result;
    }
}