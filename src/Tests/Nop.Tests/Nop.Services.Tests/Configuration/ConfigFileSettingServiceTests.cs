using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Services.Configuration;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Configuration
{
    [TestFixture]
    public class ConfigFileSettingServiceTests : ServiceTest
    {
        // requires following settings to exist in app.config
        // Setting1 : "SomeValue" : string
        // Setting2 : 25 : int
        // Setting3 : 25/12/2010 : Date

        private ISettingService _config;

        [OneTimeSetUp]
        public void SetUp()
        {
            _config = new ConfigFileSettingService(null,null);
        }

        [Test]
        public async Task Can_get_all_settings()
        {
            var settings = await _config.GetAllSettingsAsync();
            settings.Should().NotBeNull();
            settings.Any().Should().BeTrue();
        }

        [Test]
        public async Task Can_get_setting_by_key()
        {
            var setting = await _config.GetSettingByKeyAsync<string>("Setting1");
            setting.Should().Be("SomeValue");
        }

        [Test]
        public async Task Can_get_typed_setting_value_by_key()
        {
            var setting = await _config.GetSettingByKeyAsync<DateTime>("Setting3");
            setting.Should().Be(new DateTime(2010, 12, 25));
        }

        [Test]
        public async Task Default_value_returned_if_setting_does_not_exist()
        {
            var setting = await _config.GetSettingByKeyAsync("NonExistentKey", 100);
            setting.Should().Be(100);
        }
    }
}
