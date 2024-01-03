using System.Globalization;
using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Catalog;

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

    [Test]
    public async Task CanFormatWithMinimumAndMaximumDecimalValues()
    {
        // Test with minimum decimal value
        var minDecimalValue = decimal.MinValue;
        var formatMinDecimal = await _priceFormatter.FormatPriceAsync(minDecimalValue);
        formatMinDecimal.Should().NotBeNull();

        // Test with maximum decimal value
        var maxDecimalValue = decimal.MaxValue;
        var formatMaxDecimal = await _priceFormatter.FormatPriceAsync(maxDecimalValue);
        formatMaxDecimal.Should().NotBeNull();
    }

    [Test]
    public async Task CanFormatWithZeroValuesForPrices()
    {
        // Test with zero price
        var zeroPrice = 0M;
        var formatZeroPrice = await _priceFormatter.FormatPriceAsync(zeroPrice);
        formatZeroPrice.Should().NotBeNull();
    }

    [Test]
    public async Task CanHandleNullOrEmptyCurrencyCodes()
    {
        // Test with null currency code
        string nullCurrencyCode = null;
        var formatNullCurrency = await _priceFormatter.FormatPriceAsync(100M, true, nullCurrencyCode, _enLangId, false);
        formatNullCurrency.Should().NotBeNull();

        // Test with empty currency code
        string emptyCurrencyCode = string.Empty;
        var formatEmptyCurrency = await _priceFormatter.FormatPriceAsync(100M, true, emptyCurrencyCode, _enLangId, false);
        formatEmptyCurrency.Should().NotBeNull();
    }
    [Test]
    public async Task CanFormatRentalProductPriceForDays()
    {
        // Create or fetch a rental product with a known price
        var rentalProduct = new Product
        {
            IsRental = true,
            RentalPricePeriod = RentalPricePeriod.Days,
            RentalPriceLength = 5 
        };

        decimal productPrice = 50M; 
        var formattedRentalPrice = await _priceFormatter.FormatRentalProductPeriodAsync(rentalProduct, productPrice.ToString());

        // Assert that the formatted rental price for days is not null or empty
        formattedRentalPrice.Should().NotBeNullOrEmpty();

        // Assert that the formatted rental price contains the expected details
        formattedRentalPrice.Should().Contain("50"); 
        formattedRentalPrice.Should().Contain("5"); 
        formattedRentalPrice.Should().Contain("day");
    }

    [Test]
    public void FormatBasePrice_WhenProductIsNull_ReturnsNull()
    {
        // Arrange
        Product nullProduct = null;
        decimal? productPrice = 10.0M;

        // Act and Assert
        Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _priceFormatter.FormatBasePriceAsync(nullProduct, productPrice)
        );
    }
    [Test]
    public async Task FormatBasePrice_WhenBasePriceNotEnabled_ReturnsNull()
    {
        // Arrange
        var product = new Product { BasepriceEnabled = false };
        decimal? productPrice = 10.0M;

        // Act
        var result = await _priceFormatter.FormatBasePriceAsync(product, productPrice);

        // Assert
        Assert.IsNull(result);
    }
    [Test]
    public void FormatTaxRate_ReturnsCorrectFormat()
    {
        // Arrange
        decimal taxRate = 0.123456m;
        string expectedFormat = "0.123456";

        // Act
        string result = _priceFormatter.FormatTaxRate(taxRate);

        // Assert
        Assert.AreEqual(expectedFormat, result);
    }


}