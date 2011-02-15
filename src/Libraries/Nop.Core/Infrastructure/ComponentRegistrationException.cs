using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Nop.Core.Infrastructure
{
    [Serializable]
    public class ComponentRegistrationException : Exception
    {
        public ComponentRegistrationException(string serviceName)
            : base(String.Format("Component {0} could not be found but is registered in the n2/engine/components section", serviceName))
        {
        }

        protected ComponentRegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
