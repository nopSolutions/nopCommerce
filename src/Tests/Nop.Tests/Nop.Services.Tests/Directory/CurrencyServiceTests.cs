using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Directory
{
    [TestFixture]
    public class CurrencyServiceTests : ServiceTest
    {
        private ICurrencyService _currencyService;
        private IExchangeRatePluginManager _exchangeRatePluginManager;

        private Currency _currencyUsd, _currencyRur, _currencyEur;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _currencyService = GetService<ICurrencyService>();
            _exchangeRatePluginManager = GetService<IExchangeRatePluginManager>();

            _currencyEur = await _currencyService.GetCurrencyByCodeAsync("EUR");
            _currencyUsd = await _currencyService.GetCurrencyByCodeAsync("USD");
            _currencyRur = await _currencyService.GetCurrencyByCodeAsync("RUB");
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
        public async Task CanConvertCurrency()
        {
            _currencyService.ConvertCurrency(10.1M, 1.5M).Should().Be(15.15M);
            _currencyService.ConvertCurrency(10.1M, 1).Should().Be(10.1M);
            _currencyService.ConvertCurrency(10.1M, 0).Should().Be(0);
            _currencyService.ConvertCurrency(0, 5).Should().Be(0);

            var newCurrency = await _currencyService.ConvertCurrencyAsync(10.1M, _currencyEur, _currencyEur);
            newCurrency.Should().Be(10.1M);
            newCurrency = await _currencyService.ConvertCurrencyAsync(10.1M, _currencyRur, _currencyRur);
            newCurrency.Should().Be(10.1M);
            newCurrency = await _currencyService.ConvertCurrencyAsync(12M, _currencyUsd, _currencyRur);
            newCurrency.Should().Be(759M);
            newCurrency = await _currencyService.ConvertCurrencyAsync(759M, _currencyRur, _currencyUsd);
            newCurrency.Should().Be(12M);
        }
    }
}
