using Nop.Web.Factories;
using Nop.Services.Localization;
using NUnit.Framework;
using FluentAssertions;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

public class CountryModelFactoryTests : WebTest
{
    private ICountryModelFactory _countryModelFactory;
    private ILocalizationService _localizationService;
    private int _countryId;
    private int _invalidCountryId;
    private int _countryWithoutStatesId;

    [OneTimeSetUp]
    public void SetUp()
    {
        _countryModelFactory = GetService<ICountryModelFactory>();
        _localizationService = GetService<ILocalizationService>();

        _countryId = 41; //Canada
        _countryWithoutStatesId = 3; //Albania
        _invalidCountryId = 0;
    }

    [Test]
    public async Task CanGetStatesByValidCountryId()
    {
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_countryId, false);
        models.Should().NotBeNullOrEmpty();
        models.Count.Should().BeGreaterThan(1);

        models = await _countryModelFactory.GetStatesByCountryIdAsync(_countryWithoutStatesId, false);
        models.Should().NotBeNullOrEmpty();
        models.Count.Should().Be(1);
    }

    [Test]
    public async Task CanGetStatesByInvalidCountryIdWithoutSelectState()
    {
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_invalidCountryId, false);
        models.Should().NotBeNullOrEmpty();

        var result = await models.AnyAwaitAsync(
            async m => m.name == await _localizationService.GetResourceAsync("Address.Other")
        );
        result.Should().BeTrue();
    }

    [Test]
    public async Task CanGetStatesByInvalidCountryIdWithSelectState()
    {
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_invalidCountryId, true);
        models.Should().NotBeNullOrEmpty();

        var result = await models.AnyAwaitAsync(
            async m => m.name == await _localizationService.GetResourceAsync("Address.SelectState")
        );
        result.Should().BeTrue();
    }

    [Test]
    public async Task CanGetStatesByCountryIdWithoutSelectState()
    {
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_countryId, false);

        var result = await models.AllAwaitAsync(
            async m => m.name != await _localizationService.GetResourceAsync("Address.SelectState")
        );
        result.Should().BeTrue();
    }

    [Test]
    public async Task CanGetStatesByCountryIdWithSelectState()
    {
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_countryId, true);

        var result = await models.AnyAwaitAsync(
            async m => m.name == await _localizationService.GetResourceAsync("Address.SelectState")
        );
        result.Should().BeTrue();
    }
}
