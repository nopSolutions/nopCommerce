using System.Collections.Generic;
using System.Linq;
using Autofac;
using NUnit.Framework;

namespace Nop.Core.Tests.Infrastructure.DependencyManagement
{
    [TestFixture]
    public class AutofacTests
    {
        public interface IFoo { }
        public class Foo1 : IFoo { }
        public class Foo2 : IFoo { }
        public class Foo3 : IFoo { }

        [Test(Description = "Exercises a problem in a previous version, to make sure older Autofac.dll isn't picked up")]
        public void EnumerablesFromDifferentLifetimeScopesShouldReturnDifferentCollections()
        {
            var rootBuilder = new ContainerBuilder();
            rootBuilder.RegisterType<Foo1>().As<IFoo>();
            var rootContainer = rootBuilder.Build();

            var scopeA = rootContainer.BeginLifetimeScope(
                scopeBuilder => scopeBuilder.RegisterType<Foo2>().As<IFoo>());
            var arrayA = scopeA.Resolve<IEnumerable<IFoo>>().ToArray();

            var scopeB = rootContainer.BeginLifetimeScope(
                scopeBuilder => scopeBuilder.RegisterType<Foo3>().As<IFoo>());
            var arrayB = scopeB.Resolve<IEnumerable<IFoo>>().ToArray();

            Assert.That(arrayA.Count(), Is.EqualTo(2));
            Assert.That(arrayA, Has.Some.TypeOf<Foo1>());
            Assert.That(arrayA, Has.Some.TypeOf<Foo2>());

            Assert.That(arrayB.Count(), Is.EqualTo(2));
            Assert.That(arrayB, Has.Some.TypeOf<Foo1>());
            Assert.That(arrayB, Has.Some.TypeOf<Foo3>());
        }
    }
}
