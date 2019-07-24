using System;
using System.Linq;
using Nop.Services.Configuration;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Configuration
{
    [TestFixture]
    public class ConfigFileSettingServiceTests : ServiceTest
    {
        // requires following settings to exist in app.config
        // Setting1 : "SomeValue" : string
        // Setting2 : 25 : int
        // Setting3 : 25/12/2010 : Date

        ISettingService config;

        [SetUp]
        public new void SetUp()
        {
            config = new ConfigFileSettingService(null,null,null);
        }

        [Test]
        public void Can_get_all_settings()
        {
            var settings = config.GetAllSettings();
            settings.ShouldNotBeNull();
            (settings.Any()).ShouldBeTrue();
        }

        [Test]
        public void Can_get_setting_by_key()
        {
            var setting = config.GetSettingByKey<string>("Setting1");
            setting.ShouldEqual("SomeValue");
        }

        [Test]
        public void Can_get_typed_setting_value_by_key()
        {
            var setting = config.GetSettingByKey<DateTime>("Setting3");
            setting.ShouldEqual(new DateTime(2010, 12, 25));
        }

        [Test]
        public void Default_value_returned_if_setting_does_not_exist()
        {
            var setting = config.GetSettingByKey("NonExistentKey", 100);
            setting.ShouldEqual(100);
        }
    }
}
