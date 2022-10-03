using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Directory
{
    [TestFixture]
    public class CurrencyServiceTests : ServiceTest
    {
        private CurrencySettings _currencySettings;
        private ICurrencyService _currencyService;
        private IExchangeRatePluginManager _exchangeRatePluginManager;

        private Currency _currencyUsd, _currencyRur, _currencyEur, _zeroRateCurrency, _primaryCurrencyCode;
        private IDictionary<string, Currency> _currenciesByCode;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _currencyService = GetService<ICurrencyService>();
            _currencySettings = GetService<CurrencySettings>();
            _exchangeRatePluginManager = GetService<IExchangeRatePluginManager>();

            _currencyEur = await _currencyService.GetCurrencyByCodeAsync("EUR");
            _currencyUsd = await _currencyService.GetCurrencyByCodeAsync("USD");
            _currencyRur = await _currencyService.GetCurrencyByCodeAsync("RUB");
            
            _zeroRateCurrency = new Currency { Rate = decimal.Zero, Id = int.MaxValue, Name = "Multiverse Credits", CurrencyCode = "MUC" };
            _primaryCurrencyCode = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryExchangeRateCurrencyId);

            _currenciesByCode = new Dictionary<string, Currency>{
                [_primaryCurrencyCode.CurrencyCode] = _primaryCurrencyCode,
                [_currencyEur.CurrencyCode] = _currencyEur,
                [_currencyUsd.CurrencyCode] = _currencyUsd,
                [_currencyRur.CurrencyCode] = _currencyRur,
                [_zeroRateCurrency.CurrencyCode] = _zeroRateCurrency
            };
        }

        [Test]
        public async Task CanLoadExchangeRateProviders()
        {
            var providers = await _exchangeRatePluginManager.LoadAllPluginsAsync();
            providers.Should().NotBeNull();
            providers.Any().Should().BeTrue();
        }

        [Test]
        public async Task CanLoadExchangeRateProviderBySystemKeyword()
        {
            var provider = await _exchangeRatePluginManager.LoadPluginBySystemNameAsync("CurrencyExchange.TestProvider");
            provider.Should().NotBeNull();
        }

        [Test]
        public async Task CanLoadActiveExchangeRateProvider()
        {
            var provider = await _exchangeRatePluginManager.LoadPrimaryPluginAsync();
            provider.Should().NotBeNull();
        }

        [Test]
        public void CanConvertCurrency()
        {
            _currencyService.ConvertCurrency(10.1M, 1.5M).Should().Be(15.15M);
            _currencyService.ConvertCurrency(10.1M, 1).Should().Be(10.1M);
            _currencyService.ConvertCurrency(10.1M, 0).Should().Be(0);
            _currencyService.ConvertCurrency(0, 5).Should().Be(0);
        }

        [Test]
        public async Task CanConvertToPrimaryStoreCurrency()
        {
            var newCurrency = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(8.686M, _currencyEur);
            newCurrency.Should().Be(10.1M);
            newCurrency = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(638.825M, _currencyRur);
            newCurrency.Should().Be(10.1M);
            newCurrency = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(759M, _currencyUsd);
            newCurrency.Should().Be(759M);
        }

        [Test]
        public async Task CanConvertFromPrimaryStoreCurrency()
        {
            var newCurrency = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(10.1M, _currencyEur);
            newCurrency.Should().Be(8.686M);
            newCurrency = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(10.1M, _currencyRur);
            newCurrency.Should().Be(638.825M);
            newCurrency = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(759M, _currencyUsd);
            newCurrency.Should().Be(759M);
        }

        [Test]
        public async Task ConvertToPrimaryExchangeRateCurrencyAsyncWithZeroSourceRateThrowsNopExceptionAsync()
        {
            Func<Task> act = async () => await _currencyService.ConvertToPrimaryExchangeRateCurrencyAsync(100, _zeroRateCurrency);

            await act.Should().ThrowAsync<NopException>().WithMessage("*[Multiverse Credits]*");
        }

        [Test]
        public async Task ConvertFromPrimaryExchangeRateCurrencyAsyncWithZeroTargetRateThrowsNopExceptionAsync()
        {
            Func<Task> act = async () => await _currencyService.ConvertFromPrimaryExchangeRateCurrencyAsync(100, _zeroRateCurrency);

            await act.Should().ThrowAsync<NopException>().WithMessage("*[Multiverse Credits]*");
        }

        [Test]
        public async Task ConvertToPrimaryExchangeRateCurrencyAsyncShouldConvertFromEURAsync()
        {
            var newCurrency = await _currencyService.ConvertToPrimaryExchangeRateCurrencyAsync(86.00M, _currencyEur);

            newCurrency.Should().Be(100M);
        }

        [Test]
        public async Task ConvertFromPrimaryExchangeRateCurrencyAsyncShouldConvertToEURAsync()
        {
            var newCurrency =  await _currencyService.ConvertFromPrimaryExchangeRateCurrencyAsync(100M, _currencyEur);

            newCurrency.Should().Be(86.00M);
        }

        [Test]
        public async Task ConvertCurrencyAsyncWithSameCurrencyCodesDoesNotChangeAmountAsync(
            [Values("EUR", "USD", "RUB", "MUC")] string code)
        {
            var currencyCode = _currenciesByCode[code];
            var newCurrency = await _currencyService.ConvertCurrencyAsync(100M, currencyCode, currencyCode);

            newCurrency.Should().Be(100M);
        }

        [Test]
        public async Task ConvertCurrencyAsyncWithPrimaryAsSourceAndTargetDoesNotChangeAmountAsync()
        {
            var newCurrency = await _currencyService.ConvertCurrencyAsync(100M, _primaryCurrencyCode, _primaryCurrencyCode);

            newCurrency.Should().Be(100M);
        }

        [Test]        
        public async Task TestCrud()
        {
            var insertItem = new Currency
            {
                Name = "Test name",
                CurrencyCode = "tn"
            };

            var updateItem = new Currency
            {
                Name = "Test name 1",
                CurrencyCode = "tn"
            };

            await TestCrud(insertItem, _currencyService.InsertCurrencyAsync, updateItem, _currencyService.UpdateCurrencyAsync, _currencyService.GetCurrencyByIdAsync, (item, other) => item.Name.Equals(other.Name), _currencyService.DeleteCurrencyAsync);
        }
    }
}
