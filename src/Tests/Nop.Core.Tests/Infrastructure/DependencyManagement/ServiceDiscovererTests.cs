using System.Linq;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Tests.Infrastructure.DependencyManagement.Services;
using NUnit.Framework;

namespace Nop.Core.Tests.Infrastructure.DependencyManagement
{
    [TestFixture]
    public class AutoFacServiceDiscovererTests : ServiceDiscovererTests
    {
        [SetUp]
        public void SetUp()
        {
            engine = new NopEngine();
        }
    }

    public abstract class ServiceDiscovererTests
    {
        protected IEngine engine;

        [Test]
        public void Services_AreAdded_ToTheContainer()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService), typeof(NonAttributed));

            DependencyAttributeRegistrator registrator = new DependencyAttributeRegistrator(finder, engine);
            registrator.RegisterServices(registrator.FindServices());

            Assert.That(engine.Resolve<SelfService>(), Is.InstanceOf<SelfService>());
            Assert.That(new TestDelegate(() => engine.Resolve<NonAttributed>()), Throws.Exception);
        }

        [Test]
        public void Services_CanDepend_OnEachOther()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService), typeof(DependingService));

            DependencyAttributeRegistrator registrator = new DependencyAttributeRegistrator(finder, engine);
            registrator.RegisterServices(registrator.FindServices());

            var service = engine.Resolve<DependingService>();
            Assert.That(service, Is.InstanceOf<DependingService>());
            Assert.That(service.service, Is.InstanceOf<SelfService>());
        }

        [Test]
        public void Services_AreSingletons()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService));

            DependencyAttributeRegistrator registrator = new DependencyAttributeRegistrator(finder, engine);
            registrator.RegisterServices(registrator.FindServices());

            var one = engine.Resolve<SelfService>();
            var two = engine.Resolve<SelfService>();

            Assert.That(object.ReferenceEquals(one, two));
        }

        [Test]
        public void Services_AreAdded_ToTheContainer_WithServiceType()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(InterfacedService), typeof(NonAttributed));

            DependencyAttributeRegistrator registrator = new DependencyAttributeRegistrator(finder, engine);
            registrator.RegisterServices(registrator.FindServices());

            Assert.That(engine.Resolve<IService>(), Is.Not.Null);
            Assert.That(engine.Resolve<IService>(), Is.InstanceOf<InterfacedService>());
            Assert.That(new TestDelegate(() => engine.Resolve<NonAttributed>()), Throws.Exception);
        }

        [Test]
        public void GenericServices_CanBeResolved()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(GenericSelfService<>));

            DependencyAttributeRegistrator registrator = new DependencyAttributeRegistrator(finder, engine);
            registrator.RegisterServices(registrator.FindServices());

            Assert.That(engine.Resolve<GenericSelfService<int>>(), Is.InstanceOf<GenericSelfService<int>>());
            Assert.That(engine.Resolve<GenericSelfService<string>>(), Is.InstanceOf<GenericSelfService<string>>());
        }

        [Test]
        public void GenericServices_CanBeResolved_ByServiceInterface()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(GenericInterfacedService<>));

            DependencyAttributeRegistrator registrator = new DependencyAttributeRegistrator(finder, engine);
            registrator.RegisterServices(registrator.FindServices());

            Assert.That(engine.Resolve<IGenericService<int>>(), Is.InstanceOf<GenericInterfacedService<int>>());
            Assert.That(engine.Resolve<IGenericService<string>>(), Is.InstanceOf<GenericInterfacedService<string>>());
        }

        [Test]
        public void GenericServices_CanDepend_OnEachOther()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(GenericSelfService<>), typeof(GenericDependingService));

            DependencyAttributeRegistrator registrator = new DependencyAttributeRegistrator(finder, engine);
            registrator.RegisterServices(registrator.FindServices());

            var service = engine.Resolve<GenericDependingService>();
            Assert.That(service, Is.InstanceOf<GenericDependingService>());
            Assert.That(service.service, Is.InstanceOf<GenericSelfService<int>>());
        }

        [Test]
        public void Services_CanDepend_OnGenericServiceInterface()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(GenericInterfaceDependingService), typeof(GenericInterfacedService<>));

            DependencyAttributeRegistrator registrator = new DependencyAttributeRegistrator(finder, engine);
            registrator.RegisterServices(registrator.FindServices());

            var service = engine.Resolve<GenericInterfaceDependingService>();
            Assert.That(service, Is.InstanceOf<GenericInterfaceDependingService>());
            Assert.That(service.service, Is.InstanceOf<GenericInterfacedService<int>>());
        }

        [Test]
        public void GenericServices_CanDepend_OnService()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService), typeof(DependingGenericSelfService<>));

            DependencyAttributeRegistrator registrator = new DependencyAttributeRegistrator(finder, engine);
            registrator.RegisterServices(registrator.FindServices());

            var service = engine.Resolve<DependingGenericSelfService<string>>();
            Assert.That(service, Is.InstanceOf<DependingGenericSelfService<string>>());
            Assert.That(service.service, Is.InstanceOf<SelfService>());
        }

        [Test]
        public void CanResolve_ServiceWithDependency_OnComponentInstance()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(DependingServiceWithMissingDependency).Assembly.GetTypes());
            DependencyAttributeRegistrator registrator = new DependencyAttributeRegistrator(finder, engine);
            registrator.RegisterServices(registrator.FindServices());

            engine.ContainerManager.AddComponentInstance<UnregisteredDependency>(new UnregisteredDependency(), "ud");

            var service = engine.Resolve<DependingServiceWithMissingDependency>();
            Assert.That(service, Is.InstanceOf<DependingServiceWithMissingDependency>());
        }

        [Test]
        public void Resolves_OnlyRequestedConfiguration()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(HighService), typeof(LowService));

            DependencyAttributeRegistrator registrator = new DependencyAttributeRegistrator(finder, engine);
            var services = registrator.FilterServices(registrator.FindServices(), "High");
            registrator.RegisterServices(services);

            Assert.That(engine.Resolve<IBarometer>(), Is.InstanceOf<HighService>());
            Assert.That(engine.ResolveAll<IBarometer>().Count(), Is.EqualTo(1));
        }

        [Test]
        public void Requesting_MultipleConfigurations_GivesAllMatchingServices()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(HighService), typeof(LowService));

            DependencyAttributeRegistrator registrator = new DependencyAttributeRegistrator(finder, engine);
            var services = registrator.FilterServices(registrator.FindServices(), "High", "Medium", "Low");
            registrator.RegisterServices(services);

            Assert.That(engine.ResolveAll<IBarometer>().Count(), Is.EqualTo(2));
        }

        [Test]
        public void Requesting_NoConfigurations_DoesntResolveServices_ThatUsesConfigurations()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(HighService), typeof(LowService));

            DependencyAttributeRegistrator registrator = new DependencyAttributeRegistrator(finder, engine);
            var services = registrator.FilterServices(registrator.FindServices());
            registrator.RegisterServices(services);

            Assert.That(engine.ResolveAll<IBarometer>().Count(), Is.EqualTo(0));
        }

        [Test]
        public void RequstingConfiguration_AlsoRegisterd_ServicesWithoutConfiguration()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService), typeof(HighService), typeof(LowService));

            DependencyAttributeRegistrator registrator = new DependencyAttributeRegistrator(finder, engine);
            registrator.RegisterServices(registrator.FilterServices(registrator.FindServices(), "High"));

            Assert.That(engine.Resolve<SelfService>(), Is.InstanceOf<SelfService>());
            Assert.That(engine.Resolve<IBarometer>(), Is.InstanceOf<HighService>());
            Assert.That(engine.ResolveAll<IBarometer>().Count(), Is.EqualTo(1));
        }
    }
}
