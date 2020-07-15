using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Nop.Data;
using Nop.Core.Domain.Directory;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Stores;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Directory
{
    [TestFixture]
    public class CurrencyServiceTests : ServiceTest
    {
        private Mock<IRepository<Currency>> _currencyRepository;
        private Mock<IStoreMappingService> _storeMappingService;
        private CurrencySettings _currencySettings;
        private Mock<IEventPublisher> _eventPublisher;
        private ICurrencyService _currencyService;
        private IExchangeRatePluginManager _exchangeRatePluginManager;

        private Currency _currencyUSD, _currencyRUR, _currencyEUR;

        [SetUp]
        public new void SetUp()
        {
            _currencyUSD = new Currency
            {
                Id = 1,
                Name = "US Dollar",
                CurrencyCode = "USD",
                Rate = 1.2M,
                DisplayLocale = "en-US",
                CustomFormatting = "",
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                RoundingType = RoundingType.Rounding001
            };
            _currencyEUR = new Currency
            {
                Id = 2,
                Name = "Euro",
                CurrencyCode = "EUR",
                Rate = 1,
                DisplayLocale = "",
                CustomFormatting = "€0.00",
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                RoundingType = RoundingType.Rounding001
            };
            _currencyRUR = new Currency
            {
                Id = 3,
                Name = "Russian Rouble",
                CurrencyCode = "RUB",
                Rate = 34.5M,
                DisplayLocale = "ru-RU",
                CustomFormatting = "",
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                RoundingType = RoundingType.Rounding001
            };
            _currencyRepository = new Mock<IRepository<Currency>>();
            _currencyRepository.Setup(x => x.Table).Returns(new List<Currency> { _currencyUSD, _currencyEUR, _currencyRUR }.AsQueryable());
            _currencyRepository.Setup(x => x.GetById(_currencyUSD.Id)).Returns(_currencyUSD);
            _currencyRepository.Setup(x => x.GetById(_currencyEUR.Id)).Returns(_currencyEUR);
            _currencyRepository.Setup(x => x.GetById(_currencyRUR.Id)).Returns(_currencyRUR);

            _storeMappingService = new Mock<IStoreMappingService>();

            _currencySettings = new CurrencySettings
            {
                PrimaryStoreCurrencyId = _currencyUSD.Id,
                PrimaryExchangeRateCurrencyId = _currencyEUR.Id
            };

            _eventPublisher = new Mock<IEventPublisher>();
            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            var pluginService = new FakePluginService();
            _exchangeRatePluginManager = new ExchangeRatePluginManager(_currencySettings, new Mock<ICustomerService>().Object, pluginService);
            _currencyService = new CurrencyService(_currencySettings,
                new FakeCacheKeyService(),
                _eventPublisher.Object,
                _exchangeRatePluginManager,
                _currencyRepository.Object,
                _storeMappingService.Object);
        }

        [Test]
        public void Can_load_exchangeRateProviders()
        {
            RunWithTestServiceProvider(() =>
            {
                var providers = _exchangeRatePluginManager.LoadAllPlugins();
                providers.Should().NotBeNull();
                providers.Any().Should().BeTrue();
            });
        }

        [Test]
        public void Can_load_exchangeRateProvider_by_systemKeyword()
        {
            RunWithTestServiceProvider(() =>
            {
                var provider = _exchangeRatePluginManager.LoadPluginBySystemName("CurrencyExchange.TestProvider");
                provider.Should().NotBeNull();
            });
        }

        [Test]
        public void Can_load_active_exchangeRateProvider()
        {
            RunWithTestServiceProvider(() =>
            {
                var provider = _exchangeRatePluginManager.LoadPrimaryPlugin();
                provider.Should().NotBeNull();
            });
        }

        [Test]
        public void Can_convert_currency_1()
        {
            _currencyService.ConvertCurrency(10.1M, 1.5M).Should().Be(15.15M);
            _currencyService.ConvertCurrency(10.1M, 1).Should().Be(10.1M);
            _currencyService.ConvertCurrency(10.1M, 0).Should().Be(0);
            _currencyService.ConvertCurrency(0, 5).Should().Be(0);
        }

        [Test]
        public void Can_convert_currency_2()
        {
            RunWithTestServiceProvider(() =>
            {
                _currencyService.ConvertCurrency(10M, _currencyEUR, _currencyRUR).Should().Be(345M);
                _currencyService.ConvertCurrency(10.1M, _currencyEUR, _currencyEUR).Should().Be(10.1M);
                _currencyService.ConvertCurrency(10.1M, _currencyRUR, _currencyRUR).Should().Be(10.1M);
                _currencyService.ConvertCurrency(12M, _currencyUSD, _currencyRUR).Should().Be(345M);
                _currencyService.ConvertCurrency(345M, _currencyRUR, _currencyUSD).Should().Be(12M);
            });
        }
    }
}
