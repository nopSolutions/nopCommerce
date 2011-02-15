using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Registers service in the N2 inversion of container upon start.
    /// </summary>
    public class ServiceRegistrator
    {
        ITypeFinder finder;
        IServiceContainer container;

        public ServiceRegistrator(ITypeFinder finder, IServiceContainer container)
        {
            this.finder = finder;
            this.container = container;
        }

        public virtual IEnumerable<AttributeInfo<ServiceAttribute>> FindServices()
        {
            foreach (Type type in finder.Find(typeof(object)))
            {
                var attributes = type.GetCustomAttributes(typeof(ServiceAttribute), false);
                foreach (ServiceAttribute attribute in attributes)
                {
                    yield return new AttributeInfo<ServiceAttribute> { Attribute = attribute, DecoratedType = type };
                }
            }
        }

        public virtual void RegisterServices(IEnumerable<AttributeInfo<ServiceAttribute>> services)
        {
            foreach (var info in services)
            {
                Type serviceType = info.Attribute.ServiceType ?? info.DecoratedType;
                container.AddComponent(serviceType, info.DecoratedType,
                                       info.Attribute.Key ?? info.DecoratedType.FullName);
                //container.AddComponent(info.Attribute.Key ?? info.DecoratedType.FullName, serviceType, info.DecoratedType);
            }
        }

        public virtual IEnumerable<AttributeInfo<ServiceAttribute>> FilterServices(IEnumerable<AttributeInfo<ServiceAttribute>> services, params string[] configurationKeys)
        {
            return services.Where(s => s.Attribute.Configuration == null || configurationKeys.Contains(s.Attribute.Configuration));
        }
    }
}
