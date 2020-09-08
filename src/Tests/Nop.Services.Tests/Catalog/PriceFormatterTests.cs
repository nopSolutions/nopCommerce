using System.Globalization;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Nop.Core.Domain.Directory;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using NUnit.Framework;

namespace Nop.Services.Tests.Catalog
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

        [SetUp]
        public void SetUp()
        {
            _currencySettings = GetService<CurrencySettings>();
            _priceFormatter = GetService<IPriceFormatter>();
            _settingService = GetService<ISettingService>();

            var languageService = GetService<ILanguageService>();
            var currencyService = GetService<ICurrencyService>();

            _enLangId = languageService.GetAllLanguages().FirstOrDefault(p => p.Name == "English")?.Id ?? 0;

            _euro = currencyService.GetCurrencyByCode("EUR");
            _dollar = currencyService.GetCurrencyByCode("USD");
            _pound = currencyService.GetCurrencyByCode("GBP");
        }
        
        [Test]
        public void CanFormatPriceWithCustomCurrencyFormatting()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            _priceFormatter.FormatPrice(1234.5M, false, _euro, _enLangId, false, false).Should().Be("€1234.50");
        }

        [Test]
        public void CanFormatPriceWithDistinctCurrencyDisplayLocale()
        {
            _priceFormatter.FormatPrice(1234.5M, false, _dollar, _enLangId, false, false).Should().Be("$1,234.50");
            _priceFormatter.FormatPrice(1234.5M, false, _pound, _enLangId, false, false).Should().Be("£1,234.50");
        }

        [Test]
        public void CanFormatPriceWithShowTax()
        {
            _priceFormatter.FormatPrice(1234.5M, false, _dollar, _enLangId, true, true).Should().Be("$1,234.50 incl tax");
            _priceFormatter.FormatPrice(1234.5M, false, _dollar, _enLangId, false, true).Should().Be("$1,234.50 excl tax");
        }

        [Test]
        public void CanFormatPriceWithShowCurrencyCode()
        {
            _currencySettings.DisplayCurrencyLabel = true;
            _settingService.SaveSetting(_currencySettings);
            //recreate IPriceFormatter to read new currency settings
            GetService<IPriceFormatter>().FormatPrice(1234.5M, true, _dollar, _enLangId, false, false).Should().Be("$1,234.50 (USD)");

            _currencySettings.DisplayCurrencyLabel = false;
            _settingService.SaveSetting(_currencySettings);
            //recreate IPriceFormatter to read new currency settings
            GetService<IPriceFormatter>().FormatPrice(1234.5M, true, _dollar, _enLangId, false, false).Should().Be("$1,234.50");
        }
    }
}