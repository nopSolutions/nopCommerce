using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class SettingPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_setting()
        {
            var setting = new Setting
            {
                Name = "Setting1",
                Value = "Value1",
                Description = "Description1"
            };

            var fromDb = SaveAndLoadEntity(setting);
            fromDb.Name.ShouldEqual("Setting1");
            fromDb.Value.ShouldEqual("Value1");
            fromDb.Description.ShouldEqual("Description1");
        }
    }
}
