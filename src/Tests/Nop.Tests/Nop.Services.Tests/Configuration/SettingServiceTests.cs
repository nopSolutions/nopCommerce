using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Services.Configuration;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Configuration
{
    [TestFixture]
    public class SettingServiceTests : BaseNopTest
    {
        private ISettingService _settingService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _settingService = GetService<ISettingService>();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _settingService.DeleteSettingAsync<TestSettings>();
        }

        [Test]
        public async Task TestCrud()
        {
            var newSettings = new Setting { Name = "SettingServiceTests.TestCrud", Value = "TestCrud", StoreId = 1 };

            var updateSettings = new Setting
            {
                Name = "SettingServiceTests.TestCrud", Value = "TestCrud updated", StoreId = 1
            };

            await TestCrud(newSettings,
                async (setting) =>
                {
                    await _settingService.SetSettingAsync(setting.Name, setting.Value, setting.StoreId);
                    var s = await _settingService.GetSettingAsync("SettingServiceTests.TestCrud", 1);
                    setting.Id = s.Id;

                }, updateSettings,
                async (setting) =>
                    await _settingService.SetSettingAsync(setting.Name, setting.Value, setting.StoreId),
                _settingService.GetSettingByIdAsync,
                (first, second) => first.StoreId == second.StoreId &&
                                   first.Name.Equals(second.Name, StringComparison.InvariantCultureIgnoreCase) &&
                                   first.Value == second.Value, _settingService.DeleteSettingAsync);
        }

        [Test]
        public async Task CanGetSettings()
        {
            var setting = await _settingService.LoadSettingAsync<CatalogSettings>();

            setting.Should().NotBeNull();
            setting.DefaultViewMode.Should().Be("grid");
            setting.NewProductsPageSizeOptions.Should().Be("6, 3, 9");
        }

        [Test]
        public async Task CanGetAllSettings()
        {
            var settings = await _settingService.GetAllSettingsAsync();
            settings.Should().NotBeNull();
            settings.Any().Should().BeTrue();
        }

        [Test]
        public async Task CanGetSettingByKey()
        {
            var setting = await _settingService.GetSettingByKeyAsync<string>("CatalogSettings.DefaultViewMode");
            setting.Should().Be("grid");
        }

        [Test]
        public async Task CanGetTypedSettingValueByKey()
        {
            var setting = await _settingService.GetSettingByKeyAsync<int>("CatalogSettings.NumberOfProductTags");
            setting.Should().Be(15);
        }

        [Test]
        public async Task DefaultValueReturnedIfSettingDoesNotExist()
        {
            var setting = await _settingService.GetSettingByKeyAsync("NonExistentKey", 1.0f);
            setting.Should().Be(1.0f);
        }

        [Test]
        public async Task CanDeleteSetting()
        {
            var setting = new TestSettings
            {
                ServerName = "Ruby",
                Ip = "192.168.0.1",
                PortNumber = 21,
                Username = "admin",
                Password = "password"
            };

            await _settingService.SaveSettingAsync(setting);
            setting = await _settingService.LoadSettingAsync<TestSettings>();
            setting.Should().NotBeNull();
            setting.Username.Should().Be("admin");
            setting.Password.Should().Be("password");
            await _settingService.DeleteSettingAsync(setting, settings => settings.Username);
            await _settingService.DeleteSettingAsync(
                await _settingService.GetSettingAsync(_settingService.GetSettingKey(setting,
                    settings => settings.Password)));
            setting = await _settingService.LoadSettingAsync<TestSettings>();
            setting.Should().NotBeNull();
            setting.Username.Should().BeNullOrEmpty();
            setting.Password.Should().BeNullOrEmpty();
            var ipExists = await _settingService.SettingExistsAsync(setting, settings => setting.Ip);
            ipExists.Should().BeTrue();
            await _settingService.DeleteSettingAsync<TestSettings>();
            var portNumber =
                await _settingService.GetSettingAsync(_settingService.GetSettingKey(setting,
                    settings => settings.PortNumber));

            portNumber.Should().BeNull();

            ipExists = await _settingService.SettingExistsAsync(setting, settings => setting.Ip);
            ipExists.Should().BeFalse();
        }

        [Test]
        public async Task CanSaveSetting()
        {
            var setting = new TestSettings
            {
                ServerName = "Ruby",
                Ip = "192.168.0.1",
                PortNumber = 21,
                Username = "admin",
                Password = "password"
            };

            await _settingService.SaveSettingAsync(setting);
            setting = await _settingService.LoadSettingAsync<TestSettings>();
            setting.Should().NotBeNull();

            setting.ServerName.Should().Be("Ruby");
            setting.Ip.Should().Be("192.168.0.1");
            setting.PortNumber.Should().Be(21);
            setting.Username.Should().Be("admin");
            setting.Password.Should().Be("password");

            setting.PortNumber = 24;

            await _settingService.SaveSettingAsync(setting, settings => settings.PortNumber);

            setting = await _settingService.LoadSettingAsync<TestSettings>();
            setting.Should().NotBeNull();

            setting.PortNumber.Should().Be(24);

            setting.Ip = "192.168.1.1";

            await _settingService.SaveSettingOverridablePerStoreAsync(setting, settings => settings.Ip, false);

            var ip = (await _settingService.GetSettingAsync(
                _settingService.GetSettingKey(setting, settings => settings.Ip)))?.Value;

            ip.Should().NotBeNull().And.Be("192.168.1.1");

            setting.Ip = "192.168.0.2";

            await _settingService.SaveSettingOverridablePerStoreAsync(setting, settings => settings.Ip, false, 1);

            ip = (await _settingService.GetSettingAsync(
                _settingService.GetSettingKey(setting, settings => settings.Ip), 1))?.Value;

            ip.Should().BeNullOrEmpty();

            await _settingService.SaveSettingOverridablePerStoreAsync(setting, settings => settings.Ip, true, 1);

            ip = (await _settingService.GetSettingAsync(
                _settingService.GetSettingKey(setting, settings => settings.Ip), 1))?.Value;

            ip.Should().NotBeNull().And.Be("192.168.0.2");
        }

        #region Nested class

        public class TestSettings : ISettings
        {
            public string ServerName { get; set; }
            public string Ip { get; set; }
            public int PortNumber { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        #endregion
    }
}
