using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Nop.Core.ComponentModel;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;

namespace Nop.Core
{
    /// <summary>
    /// Startup task for the registration custom type converters
    /// </summary>
    public class TypeConverterRegistrationStartUpTask : IStartupTask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public Task ExecuteAsync()
        {
            //lists
            TypeDescriptor.AddAttributes(typeof(List<int>), new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
            TypeDescriptor.AddAttributes(typeof(List<decimal>), new TypeConverterAttribute(typeof(GenericListTypeConverter<decimal>)));
            TypeDescriptor.AddAttributes(typeof(List<string>), new TypeConverterAttribute(typeof(GenericListTypeConverter<string>)));

            //dictionaries
            TypeDescriptor.AddAttributes(typeof(Dictionary<int, int>), new TypeConverterAttribute(typeof(GenericDictionaryTypeConverter<int, int>)));

            //shipping option
            TypeDescriptor.AddAttributes(typeof(ShippingOption), new TypeConverterAttribute(typeof(ShippingOptionTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(List<ShippingOption>), new TypeConverterAttribute(typeof(ShippingOptionListTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(IList<ShippingOption>), new TypeConverterAttribute(typeof(ShippingOptionListTypeConverter)));

            //pickup point
            TypeDescriptor.AddAttributes(typeof(PickupPoint), new TypeConverterAttribute(typeof(PickupPointTypeConverter)));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets order of this startup task implementation
        /// </summary>
        public int Order => 1;
    }
}
