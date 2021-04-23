using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Configuration;
using Nop.Services.Configuration;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Configuration
{
    [TestFixture]
    public class ConfigurationProviderTests : ServiceTest
    {
        ISettingService _settingService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _settingService = new ConfigFileSettingService(null, null);
        }

        [Test]
        public async Task CanGetSettings()
        {
            // requires settings to be set in app.config in format TestSettings.[PropertyName]
            var settings = await _settingService.LoadSettingAsync<TestSettings>();
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
