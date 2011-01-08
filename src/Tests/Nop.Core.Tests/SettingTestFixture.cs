using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;
using Nop.Core.Domain.Configuration;

namespace Nop.Core.Tests
{
    [TestFixture]
    public class SettingTestFixture
    {
        [Test]
        public void Can_create_setting()
        {
            var setting = new Setting("Setting1", "Value1", "A test setting");
            setting.Name.ShouldEqual("Setting1");
            setting.Value.ShouldEqual("Value1");
            setting.Description.ShouldEqual("A test setting");
        }

        [Test]
        public void Can_get_typed_setting_value()
        {
            var setting = new Setting("IntSetting", "1000");
            setting.As<int>().ShouldBe<int>();
            setting.As<int>().ShouldEqual(1000);
        }
    }
}
