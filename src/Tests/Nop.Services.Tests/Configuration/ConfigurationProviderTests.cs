using FluentAssertions;
using Nop.Core.Configuration;
using Nop.Services.Configuration;
using NUnit.Framework;

namespace Nop.Services.Tests.Configuration
{
    [TestFixture]
    public class ConfigurationProviderTests : ServiceTest
    {
        ISettingService _settingService;

        [SetUp]
        public new void SetUp()
        {
            _settingService = new ConfigFileSettingService(null, null, null);
        }

        [Test]
        public void Can_get_settings()
        {
            // requires settings to be set in app.config in format TestSettings.[PropertyName]
            var settings = _settingService.LoadSetting<TestSettings>();
            settings.ServerName.Should().Be("Ruby");
            settings.Ip.Should().Be("192.168.0.1");
            settings.PortNumber.Should().Be(21);
            settings.Username.Should().Be("admin");
            settings.Password.Should().Be("password");
        }
    }

    public class TestSettings : ISettings
    {
        public string ServerName { get; set; }
        public string Ip { get; set; }
        public int PortNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
