using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.Directory;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class PriceFormatterTests : ServiceTest
    {
        private CurrencySettings _currencySettings;
        private IPriceFormatter _priceFormatter;
        private ISettingService _settingService;

        private int _enLangId;
        private Currency _euro;
        private Currency _dollar;
        private Currency _pound;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _currencySettings = GetService<CurrencySettings>();
            _priceFormatter = GetService<IPriceFormatter>();
            _settingService = GetService<ISettingService>();

            var languageService = GetService<ILanguageService>();
            var currencyService = GetService<ICurrencyService>();

            var languages = await languageService.GetAllLanguagesAsync();

            _enLangId = languages.FirstOrDefault(p => p.Name == "EN")?.Id ?? 0;

            _euro = await currencyService.GetCurrencyByCodeAsync("EUR");
            _dollar = await currencyService.GetCurrencyByCodeAsync("USD");
            _pound = await currencyService.GetCurrencyByCodeAsync("GBP");
        }
        
        [Test]
        public async Task CanFormatPriceWithCustomCurrencyFormatting()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var formatPrice = await _priceFormatter.FormatPriceAsync(1234.5M, false, _euro, _enLangId, false, false);
            formatPrice.Should().Be("€1234.50");
        }

        [Test]
        public async Task CanFormatPriceWithDistinctCurrencyDisplayLocale()
        {
            var formatPrice = await _priceFormatter.FormatPriceAsync(1234.5M, false, _dollar, _enLangId, false, false);
            formatPrice.Should().Be("$1,234.50");
            formatPrice = await _priceFormatter.FormatPriceAsync(1234.5M, false, _pound, _enLangId, false, false);
            formatPrice.Should().Be("£1,234.50");
        }

        [Test]
        public async Task CanFormatPriceWithShowTax()
        {
            var formatPrice = await _priceFormatter.FormatPriceAsync(1234.5M, false, _dollar, _enLangId, true, true);
            formatPrice.Should().Be("$1,234.50 incl tax");
            formatPrice = await _priceFormatter.FormatPriceAsync(1234.5M, false, _dollar, _enLangId, false, true);
            formatPrice.Should().Be("$1,234.50 excl tax");
        }

        [Test]
        public async Task CanFormatPriceWithShowCurrencyCode()
        {
            _currencySettings.DisplayCurrencyLabel = true;
            await _settingService.SaveSettingAsync(_currencySettings);
            //recreate IPriceFormatter to read new currency settings
            var formatPrice = await GetService<IPriceFormatter>().FormatPriceAsync(1234.5M, true, _dollar, _enLangId, false, false);
            formatPrice.Should().Be("$1,234.50 (USD)");

            _currencySettings.DisplayCurrencyLabel = false;
            await _settingService.SaveSettingAsync(_currencySettings);
            //recreate IPriceFormatter to read new currency settings
            formatPrice = await GetService<IPriceFormatter>().FormatPriceAsync(1234.5M, true, _dollar, _enLangId, false, false);
            formatPrice.Should().Be("$1,234.50");
        }
    }
}