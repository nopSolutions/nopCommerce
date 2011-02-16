using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.AutoFac;
using Nop.Core.Tests.Infrastructure.Services;
using NUnit.Framework;

namespace Nop.Core.Tests.Infrastructure
{
    [TestFixture]
    public class AutoFacServiceDiscovererTests : ServiceDiscovererTests
    {
        [SetUp]
        public void SetUp()
        {
            container = new AutoFacServiceContainer();
        }
    }

    public abstract class ServiceDiscovererTests
    {
        protected IServiceContainer container;

        [Test]
        public void Services_AreAdded_ToTheContainer()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService), typeof(NonAttributed));

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.RegisterServices(registrator.FindServices());

            Assert.That(container.Resolve<SelfService>(), Is.InstanceOf<SelfService>());
            Assert.That(new TestDelegate(() => container.Resolve<NonAttributed>()), Throws.Exception);
        }

        [Test]
        public void Services_CanDepend_OnEachOther()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService), typeof(DependingService));

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.RegisterServices(registrator.FindServices());

            var service = container.Resolve<DependingService>();
            Assert.That(service, Is.InstanceOf<DependingService>());
            Assert.That(service.service, Is.InstanceOf<SelfService>());
        }

        [Test]
        public void Services_AreSingletons()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService));

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.RegisterServices(registrator.FindServices());

            var one = container.Resolve<SelfService>();
            var two = container.Resolve<SelfService>();

            Assert.That(object.ReferenceEquals(one, two));
        }

        [Test]
        public void Services_AreAdded_ToTheContainer_WithServiceType()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(InterfacedService), typeof(NonAttributed));

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.RegisterServices(registrator.FindServices());

            Assert.That(container.Resolve<IService>(), Is.Not.Null);
            Assert.That(container.Resolve<IService>(), Is.InstanceOf<InterfacedService>());
            Assert.That(new TestDelegate(() => container.Resolve<NonAttributed>()), Throws.Exception);
        }

        [Test]
        public void GenericServices_CanBeResolved()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(GenericSelfService<>));

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.RegisterServices(registrator.FindServices());

            Assert.That(container.Resolve<GenericSelfService<int>>(), Is.InstanceOf<GenericSelfService<int>>());
            Assert.That(container.Resolve<GenericSelfService<string>>(), Is.InstanceOf<GenericSelfService<string>>());
        }

        [Test]
        public void GenericServices_CanBeResolved_ByServiceInterface()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(GenericInterfacedService<>));

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.RegisterServices(registrator.FindServices());

            Assert.That(container.Resolve<IGenericService<int>>(), Is.InstanceOf<GenericInterfacedService<int>>());
            Assert.That(container.Resolve<IGenericService<string>>(), Is.InstanceOf<GenericInterfacedService<string>>());
        }

        [Test]
        public void GenericServices_CanDepend_OnEachOther()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(GenericSelfService<>), typeof(GenericDependingService));

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.RegisterServices(registrator.FindServices());

            var service = container.Resolve<GenericDependingService>();
            Assert.That(service, Is.InstanceOf<GenericDependingService>());
            Assert.That(service.service, Is.InstanceOf<GenericSelfService<int>>());
        }

        [Test]
        public void Services_CanDepend_OnGenericServiceInterface()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(GenericInterfaceDependingService), typeof(GenericInterfacedService<>));

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.RegisterServices(registrator.FindServices());

            var service = container.Resolve<GenericInterfaceDependingService>();
            Assert.That(service, Is.InstanceOf<GenericInterfaceDependingService>());
            Assert.That(service.service, Is.InstanceOf<GenericInterfacedService<int>>());
        }

        [Test]
        public void GenericServices_CanDepend_OnService()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService), typeof(DependingGenericSelfService<>));

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.RegisterServices(registrator.FindServices());

            var service = container.Resolve<DependingGenericSelfService<string>>();
            Assert.That(service, Is.InstanceOf<DependingGenericSelfService<string>>());
            Assert.That(service.service, Is.InstanceOf<SelfService>());
        }

        [Test]
        public void CanResolve_ServiceWithDependency_OnComponentInstance()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(DependingServiceWithMissingDependency).Assembly.GetTypes());
            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.RegisterServices(registrator.FindServices());

            container.AddComponentInstance<UnregisteredDependency>(new UnregisteredDependency(), "ud");

            var service = container.Resolve<DependingServiceWithMissingDependency>();
            Assert.That(service, Is.InstanceOf<DependingServiceWithMissingDependency>());
        }

        [Test]
        public void Resolves_OnlyRequestedConfiguration()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(HighService), typeof(LowService));

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            var services = registrator.FilterServices(registrator.FindServices(), "High");
            registrator.RegisterServices(services);

            Assert.That(container.Resolve<IBarometer>(), Is.InstanceOf<HighService>());
            Assert.That(container.ResolveAll<IBarometer>().Count(), Is.EqualTo(1));
        }

        [Test]
        public void Requesting_MultipleConfigurations_GivesAllMatchingServices()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(HighService), typeof(LowService));

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            var services = registrator.FilterServices(registrator.FindServices(), "High", "Medium", "Low");
            registrator.RegisterServices(services);

            Assert.That(container.ResolveAll<IBarometer>().Count(), Is.EqualTo(2));
        }

        [Test]
        public void Requesting_NoConfigurations_DoesntResolveServices_ThatUsesConfigurations()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(HighService), typeof(LowService));

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            var services = registrator.FilterServices(registrator.FindServices());
            registrator.RegisterServices(services);

            Assert.That(container.ResolveAll<IBarometer>().Count(), Is.EqualTo(0));
        }

        [Test]
        public void RequstingConfiguration_AlsoRegisterd_ServicesWithoutConfiguration()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService), typeof(HighService), typeof(LowService));

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.RegisterServices(registrator.FilterServices(registrator.FindServices(), "High"));

            Assert.That(container.Resolve<SelfService>(), Is.InstanceOf<SelfService>());
            Assert.That(container.Resolve<IBarometer>(), Is.InstanceOf<HighService>());
            Assert.That(container.ResolveAll<IBarometer>().Count(), Is.EqualTo(1));
        }
    }
}
