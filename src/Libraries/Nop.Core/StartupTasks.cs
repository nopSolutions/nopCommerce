using System.Collections.Generic;
using System.ComponentModel;
using Nop.Core.ComponentModel;
using Nop.Core.Tasks;

namespace Nop.Core
{
    public class TypeDescriptorRegistrationStartUpTask : IStartupTask
    {
        public void Execute()
        {
            //int
            TypeDescriptor.AddAttributes(typeof(List<int>), 
                new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));

            //decimal
            TypeDescriptor.AddAttributes(typeof(List<decimal>),
                new TypeConverterAttribute(typeof(GenericListTypeConverter<decimal>)));
        }
    }
}
