using Nop.Web.Factories;
using Nop.Services.Localization;
using NUnit.Framework;
using FluentAssertions;


namespace Nop.Tests.Nop.Web.Tests.Public.Factories;
public class CountryModelFactoryTests : WebTest
{
    private ICountryModelFactory _countryModelFactory;
    private ILocalizationService _localizationService;
    private string _countryId;
    private string _invalidCountryId;

    [OneTimeSetUp]
    public void SetUp()
    {
        _countryModelFactory = GetService<ICountryModelFactory>();
        _localizationService = GetService<ILocalizationService>();

        _countryId = "41";
        _invalidCountryId = "0";
    }

    [Test]
    public async Task CanGetStatesByValidCountryId()
    {
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_countryId, false);
        models.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task CanGetStatesByInvalidCountryId()
    {
        var models = await _countryModelFactory.GetStatesByCountryIdAsync(_invalidCountryId, false);
        models.Should().NotBeNullOrEmpty();
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
