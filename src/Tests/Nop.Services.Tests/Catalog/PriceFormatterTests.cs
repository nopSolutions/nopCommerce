using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class PriceFormatterTests : ServiceTest
    {
        IRepository<Currency> _currencyRepo;
        ICurrencyService _currencyService;
        CurrencySettings _currencySettings;
        IWorkContext _workContext;
        ILocalizationService _localizationService;
        TaxSettings _taxSettings;
        IPriceFormatter _priceFormatter;
        
        [SetUp]
        public new void SetUp()
        {
            var cacheManager = new NopNullCache();

            _workContext = null;

            _currencySettings = new CurrencySettings();
            var currency1 = new Currency
            {
                Id = 1,
                Name = "Euro",
                CurrencyCode = "EUR",
                DisplayLocale =  "",
                CustomFormatting = "€0.00",
                DisplayOrder = 1,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc= DateTime.UtcNow
            };
            var currency2 = new Currency
            {
                Id = 1,
                Name = "US Dollar",
                CurrencyCode = "USD",
                DisplayLocale = "en-US",
                CustomFormatting = "",
                DisplayOrder = 2,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc= DateTime.UtcNow
            };            
            _currencyRepo = MockRepository.GenerateMock<IRepository<Currency>>();
            _currencyRepo.Expect(x => x.Table).Return(new List<Currency>() { currency1, currency2 }.AsQueryable());

            var pluginFinder = new PluginFinder(new AppDomainTypeFinder());
            _currencyService = new CurrencyService(cacheManager, _currencyRepo, _currencySettings, pluginFinder,null);
            
            _taxSettings = new TaxSettings();

            _localizationService = MockRepository.GenerateMock<ILocalizationService>();
            _localizationService.Expect(x => x.GetResource("Products.InclTaxSuffix", 1, false)).Return("{0} incl tax");
            _localizationService.Expect(x => x.GetResource("Products.ExclTaxSuffix", 1, false)).Return("{0} excl tax");
            
            _priceFormatter = new PriceFormatter(_workContext, _currencyService,_localizationService, _taxSettings);
        }

        [Test]
        public void Can_formatPrice_with_custom_currencyFormatting()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var currency = new Currency()
            {
                Id = 1,
                Name = "Euro",
                CurrencyCode = "EUR",
                DisplayLocale =  "",
                CustomFormatting = "€0.00"
            };
            var language = new Language()
            {
                Id = 1,
                Name = "English",
                LanguageCulture = "en-US"
            };
            _priceFormatter.FormatPrice(1234.5M, false, currency, language, false, false).ShouldEqual("€1234.50");
        }

        [Test]
        public void Can_formatPrice_with_distinct_currencyDisplayLocale()
        {
            var usd_currency = new Currency()
            {
                Id = 1,
                Name = "US Dollar",
                CurrencyCode = "USD",
                DisplayLocale = "en-US",
            };
            var rub_currency = new Currency()
            {
                Id = 2,
                Name = "Russian Ruble",
                CurrencyCode = "RUB",
                DisplayLocale = "ru-RU",
            };
            var language = new Language()
            {
                Id = 1,
                Name = "English",
                LanguageCulture = "en-US"
            };
            _priceFormatter.FormatPrice(1234.5M, false, usd_currency, language, false, false).ShouldEqual("$1,234.50");
            _priceFormatter.FormatPrice(1234.5M, false, rub_currency, language, false, false).ShouldEqual("1 234,50р.");
        }

        [Test]
        public void Can_formatPrice_with_showTax()
        {
            var currency = new Currency()
            {
                Id = 1,
                Name = "US Dollar",
                CurrencyCode = "USD",
                DisplayLocale = "en-US",
            };
            var language = new Language()
            {
                Id = 1,
                Name = "English",
                LanguageCulture = "en-US"
            };
            _priceFormatter.FormatPrice(1234.5M, false, currency, language, true, true).ShouldEqual("$1,234.50 incl tax");
            _priceFormatter.FormatPrice(1234.5M, false, currency, language, false, true).ShouldEqual("$1,234.50 excl tax");

        }

        [Test]
        public void Can_formatPrice_with_showCurrencyCode()
        {
            var currency = new Currency()
            {
                Id = 1,
                Name = "US Dollar",
                CurrencyCode = "USD",
                DisplayLocale = "en-US",
            };
            var language = new Language()
            {
                Id = 1,
                Name = "English",
                LanguageCulture = "en-US"
            };
            _priceFormatter.FormatPrice(1234.5M, true, currency, language, false, false).ShouldEqual("$1,234.50 (USD)");

        }
    }
}
