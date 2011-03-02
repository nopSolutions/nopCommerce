using System.Collections.Generic;
using System.ComponentModel;
using Nop.Core.ComponentModel;
using Nop.Core.Domain.Shipping;
using Nop.Core.Tasks;

namespace Nop.Core
{
    public class TypeDescriptorRegistrationStartUpTask : IStartupTask
    {
        public void Execute()
        {
            //List<int>
            TypeDescriptor.AddAttributes(typeof(List<int>), 
                new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));

            //List<decimal>
            TypeDescriptor.AddAttributes(typeof(List<decimal>),
                new TypeConverterAttribute(typeof(GenericListTypeConverter<decimal>)));

            //ShippingOption
            TypeDescriptor.AddAttributes(typeof(ShippingOption),
                new TypeConverterAttribute(typeof(ShippingOptionTypeConverter)));
        }
    }
}
