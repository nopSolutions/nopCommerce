using System.Collections.Generic;
using System.ComponentModel;
using Nop.Core.ComponentModel;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Services.Discounts;

namespace Nop.Core;

/// <summary>
/// Startup task for the registration custom type converters
/// </summary>
public partial class TypeConverterRegistrationStartUpTask : IStartupTask
{
    /// <summary>
    /// Executes a task
    /// </summary>
    public void Execute()
    {
        //lists
        TypeDescriptor.AddAttributes(typeof(List<int>), new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
        TypeDescriptor.AddAttributes(typeof(List<decimal>), new TypeConverterAttribute(typeof(GenericListTypeConverter<decimal>)));
        TypeDescriptor.AddAttributes(typeof(List<string>), new TypeConverterAttribute(typeof(GenericListTypeConverter<string>)));
        TypeDescriptor.AddAttributes(typeof(List<(int, decimal)>), new TypeConverterAttribute(typeof(GenericListTypeConverter<(int, decimal)>)));

        //dictionaries
        TypeDescriptor.AddAttributes(typeof(Dictionary<int, int>), new TypeConverterAttribute(typeof(GenericDictionaryTypeConverter<int, int>)));
        TypeDescriptor.AddAttributes(typeof(Dictionary<int, decimal>), new TypeConverterAttribute(typeof(GenericDictionaryTypeConverter<int, decimal>)));

        //shipping option
        TypeDescriptor.AddAttributes(typeof(ShippingOption), new TypeConverterAttribute(typeof(ShippingOptionTypeConverter)));
        TypeDescriptor.AddAttributes(typeof(List<ShippingOption>), new TypeConverterAttribute(typeof(ShippingOptionListTypeConverter)));
        TypeDescriptor.AddAttributes(typeof(IList<ShippingOption>), new TypeConverterAttribute(typeof(ShippingOptionListTypeConverter)));

        //pickup point
        TypeDescriptor.AddAttributes(typeof(PickupPoint), new TypeConverterAttribute(typeof(PickupPointTypeConverter)));


        TypeDescriptor.AddAttributes(typeof(DiscountPriceByQuantity), new TypeConverterAttribute(typeof(DiscountPriceByQuantityTypeConverter)));
        TypeDescriptor.AddAttributes(typeof(List<DiscountPriceByQuantity>), new TypeConverterAttribute(typeof(DiscountPriceByQuantityListTypeConverter)));
        TypeDescriptor.AddAttributes(typeof(IList<DiscountPriceByQuantity>), new TypeConverterAttribute(typeof(DiscountPriceByQuantityListTypeConverter)));
    }

    /// <summary>
    /// Gets order of this startup task implementation
    /// </summary>
    public int Order => 1;
}