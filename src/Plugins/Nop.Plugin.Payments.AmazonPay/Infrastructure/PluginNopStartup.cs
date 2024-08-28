using System.ComponentModel;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.AmazonPay.Enums;
using Nop.Plugin.Payments.AmazonPay.Services;

namespace Nop.Plugin.Payments.AmazonPay.Infrastructure;

/// <summary>
/// Represents object for the configuring services on application startup
/// </summary>
public class PluginNopStartup : INopStartup
{
    /// <summary>
    /// Add and configure any of the middleware
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="configuration">Configuration of the application</param>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        TypeDescriptor.AddAttributes(typeof(List<ButtonPlacement>), new TypeConverterAttribute(typeof(ButtonPlacementListTypeConverter)));

        //register services and interfaces
        services.AddScoped<AmazonPayApiService>();
        services.AddScoped<AmazonPayCheckoutService>();
        services.AddScoped<AmazonPayPaymentService>();
        services.AddScoped<AmazonPayCustomerService>();
        services.AddScoped<AmazonPayOnboardingService>();

        services.AddSingleton<DisallowedProducts>();
    }

    /// <summary>
    /// Configure the using of added middleware
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public void Configure(IApplicationBuilder application)
    {
    }

    /// <summary>
    /// Gets order of this startup configuration implementation
    /// </summary>
    public int Order => 1;

    #region Nested class

    /// <summary>
    /// Type converter of list of ButtonPlacementListType
    /// </summary>
    public class ButtonPlacementListTypeConverter : TypeConverter
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
            if (value is not string valueStr)
                return base.ConvertFrom(context, culture, value);

            if (string.IsNullOrEmpty(valueStr))
                return new List<ButtonPlacement>();

            return valueStr.Replace(" ", "").Split(",").Select(p => (int.TryParse(p, out var val), val))
                .Where(p => p.Item1).Select(p => (ButtonPlacement)p.val).ToList();
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

            if (value is not List<ButtonPlacement> data)
                return string.Empty;

            return string.Join(",", data.Select(p => (int)p));
        }
    }

    #endregion
}