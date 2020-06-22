using System.Collections.Generic;
using System.Linq;
using Autofac;
using FluentAssertions;
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

            arrayA.Count().Should().Be(2);
            arrayA.Should().Contain(x => x is Foo1);
            arrayA.Should().Contain(x => x is Foo2);

            arrayB.Count().Should().Be(2);
            arrayB.Should().Contain(x => x is Foo1);
            arrayB.Should().Contain(x => x is Foo3);
        }
    }
}
