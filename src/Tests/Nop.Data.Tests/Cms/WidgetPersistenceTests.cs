using System;
using System.Linq;
using Nop.Core.Domain.Cms;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Cms
{
    [TestFixture]
    public class WidgetPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_widget()
        {
            var widget = new Widget
            {
                WidgetZone = WidgetZone.AfterContent,
                PluginSystemName = "TestPluginSystemName",
                DisplayOrder = 1,
            };

            var fromDb = SaveAndLoadEntity(widget);
            fromDb.ShouldNotBeNull();
            fromDb.WidgetZone.ShouldEqual(WidgetZone.AfterContent);
            fromDb.PluginSystemName.ShouldEqual("TestPluginSystemName");
            fromDb.DisplayOrder.ShouldEqual(1);
        }
    }
}
