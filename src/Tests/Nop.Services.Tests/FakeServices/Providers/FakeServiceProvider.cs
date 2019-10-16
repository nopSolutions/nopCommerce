using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Tests;

namespace Nop.Services.Tests.FakeServices.Providers
{
    public class FakeServiceProvider : TestServiceProvider
    {
        private readonly List<(Type type, object service)> _services = new List<(Type, object)>();

        public FakeServiceProvider(params object[] services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            foreach (var service in services)
            {
                _services.Add((service.GetType(), service));
            }
        }

        public override object GetService(Type serviceType)
        {
            var service = base.GetService(serviceType);

            if (service is null)
                return _services.FirstOrDefault(t => serviceType.IsAssignableFrom(t.type)).service;

            return service;
        }
    }
}
