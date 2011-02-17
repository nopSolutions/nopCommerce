using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Registers service in the Nop inversion of container upon start.
    /// </summary>
    public class ServiceRegistrator
    {
        ITypeFinder finder;
        private IEngine engine;

        public ServiceRegistrator(ITypeFinder finder, IEngine engine)
        {
            this.finder = finder;
            this.engine = engine;
        }

        public virtual IEnumerable<AttributeInfo<ServiceAttribute>> FindServices()
        {
            foreach (Type type in finder.FindClassesOfType<object>())
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
                info.Attribute.RegisterService(info, engine.ContainerManager);
            }
        }

        public virtual IEnumerable<AttributeInfo<ServiceAttribute>> FilterServices(IEnumerable<AttributeInfo<ServiceAttribute>> services, params string[] configurationKeys)
        {
            return services.Where(s => s.Attribute.Configuration == null || configurationKeys.Contains(s.Attribute.Configuration));
        }
    }
}
