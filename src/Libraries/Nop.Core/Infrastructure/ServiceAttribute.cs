using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Markes a service that is registered in automatically registered in Nop's inversion of control container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute()
        {
        }

        public ServiceAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }

        /// <summary>The type of service the attributed class represents.</summary>
        public Type ServiceType { get; protected set; }

        /// <summary>Optional key to associate with the service.</summary>
        public string Key { get; set; }

        /// <summary>Configurations for which this service is registered.</summary>
        public string Configuration { get; set; }
    }
}
