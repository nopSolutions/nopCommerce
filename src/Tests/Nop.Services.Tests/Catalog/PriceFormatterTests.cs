using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Tax;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class PriceFormatterTests
    {
        ICurrencyService _currencyService;
        IWorkContext _workContext;
        ILocalizationService _localizationService;
        TaxSettings _taxSettings;
        IPriceFormatter _priceFormatter;

        public PriceFormatterTests()
        {
        }

        [SetUp]
        public void SetUp()
        {
            _taxSettings = new TaxSettings();
            
            _currencyService = MockRepository.GenerateMock<ICurrencyService>();
            _currencyService.Expect(x => x.GetAllCurrencies()).Return(new List<Currency>() { new Currency(), new Currency() });

            _localizationService = MockRepository.GenerateMock<ILocalizationService>();
            _localizationService.Expect(x => x.GetResource("Products.InclTaxSuffix", 1, false)).Return("{0} incl tax");
            _localizationService.Expect(x => x.GetResource("Products.ExclTaxSuffix", 1, false)).Return("{0} excl tax");
            
            _priceFormatter = new PriceFormatter(_workContext, _currencyService,_localizationService, _taxSettings);
        }

        [Test]
        public void Can_formatPrice_with_custom_currencyFormatting()
        {
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
            _priceFormatter.FormatPrice(1234.5M, false, currency, language, false, false).ShouldEqual("€1234,50");
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
