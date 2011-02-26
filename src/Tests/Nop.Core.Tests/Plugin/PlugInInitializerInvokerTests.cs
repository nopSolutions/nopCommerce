using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Plugin
{
    [TestFixture]
    public class PlugInInitializerInvokerTests : TestsBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            PlugInInitializer1.WasInitialized = false;
            PlugInInitializer2.WasInitialized = false;
            PlugInInitializer3.WasInitialized = false;
            ThrowingPlugin1.WasInitialized = false;
            ThrowingPlugin2.WasInitialized = false;
        }

        [Test]
        public void AssemblyDefinedPluginInitializer_IsInvoked()
        {
            ITypeFinder typeFinder = new Fakes.FakeTypeFinder(typeof(PlugInInitializer1).Assembly, new[] { typeof(PlugInInitializer1) });

            var invoker = new PluginBootstrapper(typeFinder);
            PlugInInitializer1.WasInitialized = false;
            invoker.InitializePlugins(null, invoker.GetPluginDefinitions());

            Assert.That(PlugInInitializer1.WasInitialized, Is.True);
        }

        [Test]
        public void AutoInitializePluginInitializer_IsInvoked()
        {
            ITypeFinder typeFinder = new Fakes.FakeTypeFinder(new[] { typeof(PlugInInitializer2) });

            var invoker = new PluginBootstrapper(typeFinder);
            PlugInInitializer2.WasInitialized = false;
            invoker.InitializePlugins(null, invoker.GetPluginDefinitions());

            Assert.That(PlugInInitializer2.WasInitialized, Is.True);
        }

        [Test]
        public void PluginInitializer_WithoutInitializerDefinition_IsNotInvoked()
        {
            ITypeFinder typeFinder = new Fakes.FakeTypeFinder(new[] { typeof(PlugInInitializer3) });

            var invoker = new PluginBootstrapper(typeFinder);
            invoker.InitializePlugins(null, invoker.GetPluginDefinitions());

            Assert.That(PlugInInitializer3.WasInitialized, Is.False);
        }

        [Test]
        public void Initializers_AreExecuted_AfterException()
        {
            ITypeFinder typeFinder = new Fakes.FakeTypeFinder(new[] { typeof(ThrowingPlugin1), typeof(PlugInInitializer2) });

            PluginBootstrapper invoker = new PluginBootstrapper(typeFinder);
            PlugInInitializer2.WasInitialized = false;

            ThrowingPlugin1.Throw = true;
            PluginInitializationException ex = ExceptionAssert.Throws<PluginInitializationException>(delegate
            {
                invoker.InitializePlugins(null, invoker.GetPluginDefinitions());
            });
            ThrowingPlugin1.Throw = false;

            Assert.That(PlugInInitializer2.WasInitialized, Is.True);
        }

        [Test]
        public void InnerException_IsInnerException_OfInitializationException()
        {
            ITypeFinder typeFinder = new Fakes.FakeTypeFinder(new[] { typeof(ThrowingPlugin1), typeof(PlugInInitializer2) });

            mocks.ReplayAll();

            PluginBootstrapper invoker = new PluginBootstrapper(typeFinder);
            PlugInInitializer2.WasInitialized = false;

            ThrowingPlugin1.Throw = true;
            PluginInitializationException ex = ExceptionAssert.Throws<PluginInitializationException>(delegate
            {
                invoker.InitializePlugins(null, invoker.GetPluginDefinitions());
            });
            ThrowingPlugin1.Throw = false;

            Assert.That(ex.InnerException, Is.TypeOf(typeof(SomeException)));
            Assert.That(ex.Message.Contains("ThrowingPlugin1 isn't happy."));
        }

        [Test]
        public void InnerExceptions_AreAdded_ToInitializationException()
        {
            ITypeFinder typeFinder = new Fakes.FakeTypeFinder(new[] { typeof(ThrowingPlugin1), typeof(PlugInInitializer2), typeof(ThrowingPlugin2) });

            PluginBootstrapper invoker = new PluginBootstrapper(typeFinder);
            PlugInInitializer2.WasInitialized = false;

            ThrowingPlugin1.Throw = true;
            ThrowingPlugin2.Throw = true;
            PluginInitializationException ex = ExceptionAssert.Throws<PluginInitializationException>(delegate
            {
                invoker.InitializePlugins(null, invoker.GetPluginDefinitions());
            });
            ThrowingPlugin1.Throw = false;
            ThrowingPlugin2.Throw = false;

            Assert.That(ex.InnerExceptions.Length, Is.EqualTo(2));
            Assert.That(ex.Message.Contains("ThrowingPlugin1 isn't happy."));
            Assert.That(ex.Message.Contains("ThrowingPlugin2 is really mad."));
        }

        //[Test]
        //public void AssemblyDefined_PluginInitializers_CanBeRemoved_ByName()
        //{
        //    ITypeFinder typeFinder = new Fakes.FakeTypeFinder(typeof(PlugIn1).Assembly, new[] { typeof(PlugIn1) });

        //    EngineSection config = CreateConfiguration(null, new[]
        //    {
        //        new PluginInitializerElement { Name = typeof(PlugIn1).Name }
        //    });
        //    PluginBootstrapper invoker = new PluginBootstrapper(typeFinder, config);
        //    invoker.InitializePlugins(null, invoker.GetPluginDefinitions());

        //    Assert.That(PlugIn1.WasInitialized, Is.False);
        //}

        //[Test]
        //public void AssemblyDefined_PluginInitializers_CanBeRemoved_ByType()
        //{
        //    ITypeFinder typeFinder = new Fakes.FakeTypeFinder(typeof(PlugIn1).Assembly, new[] { typeof(PlugIn1) });

        //    EngineSection config = CreateConfiguration(null, new[]
        //    {
        //        new PluginInitializerElement { Name = "ignored", Type = typeof(PlugIn1).AssemblyQualifiedName }
        //    });
        //    PluginBootstrapper invoker = new PluginBootstrapper(typeFinder, config);
        //    invoker.InitializePlugins(null, invoker.GetPluginDefinitions());

        //    Assert.That(PlugIn1.WasInitialized, Is.False);
        //}

        //[Test]
        //public void AutoInitialized_PluginInitializers_CanBeRemoved_UsingConfiguration_ByName()
        //{
        //    ITypeFinder typeFinder = new Fakes.FakeTypeFinder(typeof(PlugIn1).Assembly, new[] { typeof(PlugIn2) });

        //    EngineSection config = CreateConfiguration(null, new[]
        //    {
        //        new PluginInitializerElement { Name = typeof(PlugIn2).Name }
        //    });
        //    PluginBootstrapper invoker = new PluginBootstrapper(typeFinder, config);
        //    invoker.InitializePlugins(null, invoker.GetPluginDefinitions());

        //    Assert.That(PlugIn2.WasInitialized, Is.False);
        //}

        //[Test]
        //public void AutoInitialized_PluginInitializers_CanBeRemoved_UsingConfiguration_ByType()
        //{
        //    ITypeFinder typeFinder = new Fakes.FakeTypeFinder(typeof(PlugIn1).Assembly, new[] { typeof(PlugIn2) });

        //    EngineSection config = CreateConfiguration(null, new[]
        //    {
        //        new PluginInitializerElement { Name = "ignored", Type = typeof(PlugIn2).AssemblyQualifiedName }
        //    });
        //    PluginBootstrapper invoker = new PluginBootstrapper(typeFinder, config);
        //    invoker.InitializePlugins(null, invoker.GetPluginDefinitions());

        //    Assert.That(PlugIn2.WasInitialized, Is.False);
        //}

        //[Test]
        //public void Plugins_CanBeInitialized_FromConfiguration()
        //{
        //    ITypeFinder typeFinder = new Fakes.FakeTypeFinder(typeof(PlugIn1).Assembly, new[] { typeof(PlugIn3) });

        //    EngineSection config = CreateConfiguration(new[]
        //    {
        //        new PluginInitializerElement { Name = "ignored", Type = typeof(PlugIn3).AssemblyQualifiedName }
        //    }, null);
        //    PluginBootstrapper invoker = new PluginBootstrapper(typeFinder, config);
        //    invoker.InitializePlugins(null, invoker.GetPluginDefinitions());

        //    Assert.That(PlugIn3.WasInitialized, Is.True);
        //}

        //EngineSection CreateConfiguration(PluginInitializerElement[] addedPlugins, PluginInitializerElement[] removedPlugins)
        //{
        //    return new EngineSection
        //    {
        //        PluginInitializers = new PluginInitializerCollection
        //        {
        //            AllElements = addedPlugins,
        //            RemovedElements = removedPlugins
        //        }
        //    };
        //}
    }
}
