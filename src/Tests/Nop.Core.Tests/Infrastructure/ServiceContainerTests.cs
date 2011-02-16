using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.AutoFac;
using Nop.Core.Plugins;
using Nop.Core.Tests.Infrastructure.Services;
using NUnit.Framework;

namespace Nop.Core.Tests.Infrastructure
{
    [TestFixture]
    public class AutoFacServiceContainerTests : ServiceContainerTests
    {
        [SetUp]
        public void SetUp()
        {
            container = new AutoFacServiceContainer();
        }
    }

    public abstract class ServiceContainerTests
    {
        protected IServiceContainer container;

        [Test]
        public void Resolve_Service_Generic()
        {
            container.AddComponent<IService, InterfacedService>("key");

            var service = container.Resolve<IService>();

            Assert.That(service, Is.InstanceOf<InterfacedService>());
        }

        [Test]
        public void Resolve_Service_WithParameter()
        {
            container.AddComponent<IService, InterfacedService>("key");

            var service = container.Resolve(typeof(IService));

            Assert.That(service, Is.InstanceOf<InterfacedService>());
        }

        [Test]
        public void ResolveAll_SingleServices()
        {
            container.AddComponent<IService, InterfacedService>("key");

            var services = container.ResolveAll<IService>();

            Assert.That(services.Count(), Is.EqualTo(1));
            Assert.That(services.First(), Is.InstanceOf<InterfacedService>());
        }

        [Test]
        public void ResolveAll_MultipleServices_Generic()
        {
            container.AddComponent<Startable, ServiceA>("keyA");
            container.AddComponent<Startable, ServiceB>("keyB");
            container.AddComponent<Startable, ServiceC>("keyC");

            var services = container.ResolveAll<Startable>();

            Assert.That(services.Count(), Is.EqualTo(3));
            Assert.That(services.Any(s => s.GetType() == typeof(ServiceA)));
            Assert.That(services.Any(s => s.GetType() == typeof(ServiceB)));
            Assert.That(services.Any(s => s.GetType() == typeof(ServiceC)));
        }

        [Test]
        public void ResolveAll_MultipleServices_WithParameter()
        {
            container.AddComponent<Startable, ServiceA>("keyA");
            container.AddComponent<Startable, ServiceB>("keyB");
            container.AddComponent<Startable, ServiceC>("keyC");

            var services = container.ResolveAll<Startable>().OfType<object>();

            Assert.That(services.Count(), Is.EqualTo(3));
            Assert.That(services.Any(s => s.GetType() == typeof(ServiceA)));
            Assert.That(services.Any(s => s.GetType() == typeof(ServiceB)));
            Assert.That(services.Any(s => s.GetType() == typeof(ServiceC)));
        }

        public class Startable : IAutoStart
        {
            public static int counter = 0;

            public int startOrder = 0;
            public bool started = false;
            public bool stopped = false;
            public int timesStarted = 0;
            public int timesStopped = 0;

            #region IAutoStart Members

            public void Start()
            {
                startOrder = ++counter;
                started = true;
                timesStarted++;
            }

            public void Stop()
            {
                stopped = true;
                timesStopped++;
            }

            #endregion
        }

        public class ServiceA : Startable
        {
        }

        public class ServiceB : Startable
        {
        }

        public class ServiceC : Startable
        {
        }

        [Test]
        public void AutoStartServices_AreStarted()
        {
            container.AddComponent<Startable>("key");
            container.StartComponents();
            var s = container.Resolve<Startable>();

            Assert.That(s.started, Is.True);
        }

        [Test]
        public void AutoStartServices_RetrieveBeforeGeneralStart_AreSameInstance()
        {
            container.AddComponent(typeof(IAutoStart), typeof(Startable), "key");

            var s1 = container.Resolve<IAutoStart>();
            container.StartComponents();
            var s2 = container.Resolve<IAutoStart>();

            Assert.That(s1, Is.SameAs(s2));
        }

        [Test]
        public void AutoStartServices_AreStarted_WhenAddedAfterStartSignal()
        {
            container.StartComponents();
            container.AddComponent(typeof(IAutoStart), typeof(Startable), "key");
            var s = (Startable)container.Resolve<IAutoStart>();

            Assert.That(s.started, Is.True);
        }

        [Test]
        public void AutoStartServices_AreStarted_Once()
        {
            container.AddComponent(typeof(IAutoStart), typeof(Startable), "key");
            container.Resolve<IAutoStart>();
            container.StartComponents();
            container.Resolve<IAutoStart>();
            var s = (Startable)container.Resolve<IAutoStart>();

            Assert.That(s.timesStarted, Is.EqualTo(1));
        }
    }
}
