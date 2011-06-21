//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Principal;
//using System.Text;
//using Nop.Core.Configuration;
//using Nop.Core.Plugins;
//using NUnit.Framework;

//namespace Nop.Core.Tests.Plugin
//{
//    //TODO implement (plugins are into Plugins.cs file)
//    [TestFixture]
//    public class PluginFinderTests : TypeFindingBase
//    {
//        PluginFinder finder;
//        [SetUp]
//        public override void SetUp()
//        {
//            base.SetUp();

//            finder = new PluginFinder(typeFinder);
//        }

//        protected override Type[] GetTypes()
//        {
//            return new[] { typeof(TestPlugin1Implementation1), typeof(TestPlugin1Implementation2) };
//        }

//        [Test]
//        public void CanGetNavigationPlugIns()
//        {
//            IEnumerable<ITestPlugin1> plugIns = finder.GetPlugins<ITestPlugin1>();
//            Assert.AreEqual(2, plugIns.Count());
//        }
//    }
//}
