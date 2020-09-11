using System.Linq;
using FluentAssertions;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using NUnit.Framework;

namespace Nop.Services.Tests.Directory
{
    [TestFixture]
    public class CurrencyServiceTests : ServiceTest
    {
        private ICurrencyService _currencyService;
        private IExchangeRatePluginManager _exchangeRatePluginManager;

        private Currency _currencyUsd, _currencyRur, _currencyEur;

        [SetUp]
        public void SetUp()
        {
            _currencyService = GetService<ICurrencyService>();
            _exchangeRatePluginManager = GetService<IExchangeRatePluginManager>();

            _currencyEur = _currencyService.GetCurrencyByCode("EUR");
            _currencyUsd = _currencyService.GetCurrencyByCode("USD");
            _currencyRur = _currencyService.GetCurrencyByCode("RUB");
        }

        [Test]
        public void CanLoadExchangeRateProviders()
        {
            var providers = _exchangeRatePluginManager.LoadAllPlugins();
            providers.Should().NotBeNull();
            providers.Any().Should().BeTrue();
        }

        [Test]
        public void CanLoadExchangeRateProviderBySystemKeyword()
        {
            var provider = _exchangeRatePluginManager.LoadPluginBySystemName("CurrencyExchange.TestProvider");
            provider.Should().NotBeNull();
        }

        [Test]
        public void CanLoadActiveExchangeRateProvider()
        {
            var provider = _exchangeRatePluginManager.LoadPrimaryPlugin();
            provider.Should().NotBeNull();
        }

        [Test]
        public void CanConvertCurrency()
        {
            _currencyService.ConvertCurrency(10.1M, 1.5M).Should().Be(15.15M);
            _currencyService.ConvertCurrency(10.1M, 1).Should().Be(10.1M);
            _currencyService.ConvertCurrency(10.1M, 0).Should().Be(0);
            _currencyService.ConvertCurrency(0, 5).Should().Be(0);

            _currencyService.ConvertCurrency(10.1M, _currencyEur, _currencyEur).Should().Be(10.1M);
            _currencyService.ConvertCurrency(10.1M, _currencyRur, _currencyRur).Should().Be(10.1M);
            _currencyService.ConvertCurrency(12M, _currencyUsd, _currencyRur).Should().Be(759M);
            _currencyService.ConvertCurrency(759M, _currencyRur, _currencyUsd).Should().Be(12M);
        }
    }
}
