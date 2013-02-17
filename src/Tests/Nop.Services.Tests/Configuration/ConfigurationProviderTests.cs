using Nop.Core.Configuration;
using Nop.Services.Configuration;
using Nop.Tests;
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
            settings.ServerName.ShouldEqual("Ruby");
            settings.Ip.ShouldEqual("192.168.0.1");
            settings.PortNumber.ShouldEqual(21);
            settings.Username.ShouldEqual("admin");
            settings.Password.ShouldEqual("password");
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
