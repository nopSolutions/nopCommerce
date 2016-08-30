using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Core.Plugins;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Stores;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Directory
{
    [TestFixture]
    public class CurrencyServiceTests : ServiceTest
    {
        private IRepository<Currency> _currencyRepository;
        private IStoreMappingService _storeMappingService;
        private CurrencySettings _currencySettings;
        private IEventPublisher _eventPublisher;
        private ICurrencyService _currencyService;

        private Currency currencyUSD, currencyRUR, currencyEUR;
        
        [SetUp]
        public new void SetUp()
        {
            currencyUSD = new Currency
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
            };
            currencyEUR = new Currency
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
            };
            currencyRUR = new Currency
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
            };
            _currencyRepository = MockRepository.GenerateMock<IRepository<Currency>>();
            _currencyRepository.Expect(x => x.Table).Return(new List<Currency> { currencyUSD, currencyEUR, currencyRUR }.AsQueryable());
            _currencyRepository.Expect(x => x.GetById(currencyUSD.Id)).Return(currencyUSD);
            _currencyRepository.Expect(x => x.GetById(currencyEUR.Id)).Return(currencyEUR);
            _currencyRepository.Expect(x => x.GetById(currencyRUR.Id)).Return(currencyRUR);

            _storeMappingService = MockRepository.GenerateMock<IStoreMappingService>();

            var cacheManager = new NopNullCache();
            
            _currencySettings = new CurrencySettings();
            _currencySettings.PrimaryStoreCurrencyId = currencyUSD.Id;
            _currencySettings.PrimaryExchangeRateCurrencyId = currencyEUR.Id;

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));
            
            var pluginFinder = new PluginFinder();
            _currencyService = new CurrencyService(cacheManager,
                _currencyRepository, _storeMappingService, 
                _currencySettings, pluginFinder, _eventPublisher);
        }
        
        [Test]
        public void Can_load_exchangeRateProviders()
        {
            var providers = _currencyService.LoadAllExchangeRateProviders();
            providers.ShouldNotBeNull();
            (providers.Any()).ShouldBeTrue();
        }

        [Test]
        public void Can_load_exchangeRateProvider_by_systemKeyword()
        {
            var provider = _currencyService.LoadExchangeRateProviderBySystemName("CurrencyExchange.TestProvider");
            provider.ShouldNotBeNull();
        }

        [Test]
        public void Can_load_active_exchangeRateProvider()
        {
            var provider = _currencyService.LoadActiveExchangeRateProvider();
            provider.ShouldNotBeNull();
        }

        [Test]
        public void Can_convert_currency_1()
        {
            _currencyService.ConvertCurrency(10.1M, 1.5M).ShouldEqual(15.15M);
            _currencyService.ConvertCurrency(10.1M, 1).ShouldEqual(10.1M);
            _currencyService.ConvertCurrency(10.1M, 0).ShouldEqual(0);
            _currencyService.ConvertCurrency(0, 5).ShouldEqual(0);
        }

        [Test]
        public void Can_convert_currency_2()
        {
            _currencyService.ConvertCurrency(10M, currencyEUR, currencyRUR).ShouldEqual(345M);
            _currencyService.ConvertCurrency(10.1M, currencyEUR, currencyEUR).ShouldEqual(10.1M);
            _currencyService.ConvertCurrency(10.1M, currencyRUR, currencyRUR).ShouldEqual(10.1M);
            _currencyService.ConvertCurrency(12M, currencyUSD, currencyRUR).ShouldEqual(345M);
            _currencyService.ConvertCurrency(345M, currencyRUR, currencyUSD).ShouldEqual(12M);
        }
    }
}
