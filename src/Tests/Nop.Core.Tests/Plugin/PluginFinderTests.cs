using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Nop.Core.Configuration;
using Nop.Core.Plugins;
using NUnit.Framework;

namespace Nop.Core.Tests.Plugin
{
    [TestFixture]
    public class PluginFinderTests : TypeFindingBase
    {
        PluginFinder finder;
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            finder = new PluginFinder(typeFinder);
        }

        protected override Type[] GetTypes()
        {
            return new Type[0];
        }

        //[Test]
        //public void CanGetNavigationPlugIns()
        //{
        //    IEnumerable<NavigationPluginAttribute> plugIns = finder.GetPlugins<NavigationPluginAttribute>();
        //    EnumerableAssert.Count(2, plugIns);
        //}

        //[Test]
        //public void CanGet_SortNavigation_PlugIns()
        //{
        //    IList<NavigationPluginAttribute> plugIns = new List<NavigationPluginAttribute>(finder.GetPlugins<NavigationPluginAttribute>());
        //    EnumerableAssert.Count(2, plugIns);

        //    NavigationPluginAttribute plugin1 = plugIns[0];
        //    Assert.AreEqual("chill", plugin1.Name);
        //    Assert.AreEqual("Chill in", plugin1.Title);

        //    NavigationPluginAttribute plugin2 = plugIns[1];
        //    Assert.AreEqual("buzz", plugin2.Name);
        //    Assert.AreEqual("Buzz out", plugin2.Title);
        //}

        //[Test]
        //public void Doesnt_GetNavigationPlugins_ThatRequires_SpecialAuthorization()
        //{
        //    IPrincipal user = CreatePrincipal("Joe", "Carpenter");
        //    IEnumerable<NavigationPluginAttribute> plugIns = finder.GetPlugins<NavigationPluginAttribute>(user);
        //    EnumerableAssert.Count(1, plugIns);
        //}

        //[Test]
        //public void CanGet_Restricted_NavigationPlugins_IfAuthorized()
        //{
        //    IPrincipal user = CreatePrincipal("Bill", "ÜberEditor");
        //    IEnumerable<NavigationPluginAttribute> plugIns = finder.GetPlugins<NavigationPluginAttribute>(user);
        //    EnumerableAssert.Count(2, plugIns);
        //}

        //[Test]
        //public void CanGet_ToolbarPlugIns()
        //{
        //    IEnumerable<ToolbarPluginAttribute> plugIns = finder.GetPlugins<ToolbarPluginAttribute>();
        //    EnumerableAssert.Count(2, plugIns);
        //}

        //[Test]
        //public void CanGetSortToolbarPlugIns()
        //{
        //    IList<ToolbarPluginAttribute> plugIns = new List<ToolbarPluginAttribute>(finder.GetPlugins<ToolbarPluginAttribute>());
        //    EnumerableAssert.Count(2, plugIns);

        //    ToolbarPluginAttribute plugin1 = plugIns[0];
        //    Assert.AreEqual("peace", plugin1.Name);
        //    Assert.AreEqual("Don't worry be happy", plugin1.Title);

        //    ToolbarPluginAttribute plugin2 = plugIns[1];
        //    Assert.AreEqual("panic", plugin2.Name);
        //    Assert.AreEqual("Worry we're coming", plugin2.Title);
        //}

        //[Test]
        //public void Doesnt_GetToolbarPlugins_ThatRequires_SpecialAuthorization()
        //{
        //    IPrincipal user = CreatePrincipal("Joe", "Carpenter");
        //    IEnumerable<ToolbarPluginAttribute> plugIns = finder.GetPlugins<ToolbarPluginAttribute>(user);
        //    EnumerableAssert.Count(1, plugIns);
        //}

        //[Test]
        //public void CanGet_AllRestrictedToolbarPlugins_IfAuthorized()
        //{
        //    IPrincipal user = CreatePrincipal("Bill", "ÜberEditor");
        //    IEnumerable<ToolbarPluginAttribute> plugIns = finder.GetPlugins<ToolbarPluginAttribute>(user);
        //    EnumerableAssert.Count(2, plugIns);
        //}

        //[Test]
        //public void CanRemovePlugins_ThroughConfiguration()
        //{
        //    int initialCount = finder.GetPlugins<NavigationPluginAttribute>().Count();
        //    finder = new PluginFinder(typeFinder, CreateEngineSection(new[] { new InterfacePluginElement { Name = "chill" } }));

        //    IEnumerable<NavigationPluginAttribute> plugins = finder.GetPlugins<NavigationPluginAttribute>();

        //    Assert.That(plugins.Count(), Is.EqualTo(initialCount - 1), "Found unexpected items, e.g.:" + plugins.FirstOrDefault());
        //}

        //EngineSection CreateEngineSection(InterfacePluginElement[] removedElements)
        //{
        //    return new EngineSection
        //    {
        //        InterfacePlugins = new InterfacePluginCollection
        //        {
        //            RemovedElements = removedElements
        //        }
        //    };
        //}

        
    }
}
