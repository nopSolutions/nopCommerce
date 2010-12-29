using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using StructureMap;

namespace Nop.Web.Framework.IoC
{
    public class StructureMapDependencyResolver : IDependencyResolver
    {
        private readonly IContainer container;

        public StructureMapDependencyResolver(IContainer container) {
            this.container = container;
        }

        public object GetService(Type serviceType) {
            var instance = container.TryGetInstance(serviceType);
            if (instance == null && !serviceType.IsAbstract) {
                instance = AddTypeAndTryGetInstance(serviceType);
            }
            return instance;
        }

        private object AddTypeAndTryGetInstance(Type serviceType) {
            container.Configure(c => c.AddType(serviceType, serviceType));
            return container.TryGetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return container.GetAllInstances(serviceType).Cast<object>();
        }
    }
}
