using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Nop.Core.Tests.Infrastructure.DependencyManagement
{
    [TestFixture]
    public class ServiceCollectionTests
    {
        public interface IFoo { }
        public class Foo1 : IFoo { }
        public class Foo2 : IFoo { }
        public class Foo3 : IFoo { }

        [Test]
        public void EnumerablesFromDifferentLifetimeScopesShouldReturnDifferentCollections()
        {
            var services = new ServiceCollection();
            services.AddTransient<IFoo, Foo1>();
            services.AddTransient<IFoo, Foo2>();
            var serviceProvider = services.BuildServiceProvider();
            var arrayA = serviceProvider.GetServices<IFoo>().ToArray();

            services.AddTransient<IFoo, Foo3>();
            serviceProvider = services.BuildServiceProvider();
            var arrayB = serviceProvider.GetServices<IFoo>().ToArray();

            Assert.That(arrayA.Count(), Is.EqualTo(2));
            Assert.That(arrayA, Has.Some.TypeOf<Foo1>());
            Assert.That(arrayA, Has.Some.TypeOf<Foo2>());

            Assert.That(arrayB.Count(), Is.EqualTo(3));
            Assert.That(arrayB, Has.Some.TypeOf<Foo1>());
            Assert.That(arrayB, Has.Some.TypeOf<Foo2>());
            Assert.That(arrayB, Has.Some.TypeOf<Foo3>());
        }
    }
}
