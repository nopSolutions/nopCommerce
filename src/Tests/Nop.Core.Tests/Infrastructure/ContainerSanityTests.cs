using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.AutoFac;
using Nop.Core.Plugins;
using Nop.Core.Web;
using NUnit.Framework;

namespace Nop.Core.Tests.Infrastructure
{
    [TestFixture]
    public class AutoFacContainerSanityTests : ContainerSanityTests
    {
        protected override IEngine CreateEngine()
        {
            var engine = new NopEngine(new AutoFacServiceContainer(), EventBroker.Instance, new ContainerConfigurer());

            return engine;
        }
    }

    /// <summary>
    /// Class that allows you to unit test any IEngine implementations
    /// </summary>
    public abstract class ContainerSanityTests
    {
        IServiceContainer container;

        [SetUp]
        public void SetUp()
        {
            container = CreateEngine().Container;
        }

        protected abstract IEngine CreateEngine();

        [Test]
        public void CanRetrieve_ImportantServices()
        {
            Assert.That(container.Resolve<IWebContext>(), Is.Not.Null);
            Assert.That(container.Resolve<IEngine>(), Is.Not.Null);
            Assert.That(container.Resolve<IPluginBootstrapper>(), Is.Not.Null);
        }

        [Test]
        public void AddComponentLifeStyle_DoesNotReturnSameServiceTwiceWhenSingleton()
        {
            container.AddComponent<object>("testing");

            var class1 = container.Resolve<object>();
            var class2 = container.Resolve<object>();

            Assert.That(class1, Is.SameAs(class2));
        }

        [Test]
        public void AddComponentLifeStyle_DoesNotReturnSameServiceTwiceWhenTransient()
        {
            container.AddComponent<object>("testing", ComponentLifeStyle.Transient);

            var class1 = container.Resolve<object>();
            var class2 = container.Resolve<object>();

            Assert.That(class1, Is.Not.SameAs(class2));
        }
    }
}
