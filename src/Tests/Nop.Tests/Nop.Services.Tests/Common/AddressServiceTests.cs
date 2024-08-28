using FluentAssertions;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Common;

[TestFixture]
public class AddressServiceTests : ServiceTest
{
    private IAddressService _addressService;
    private ICountryService _countryService;
    private ILocalizationService _localizationService;
    private IStateProvinceService _stateProvinceService;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        _localizationService = GetService<ILocalizationService>();
        _countryService = GetService<ICountryService>();
        _stateProvinceService = GetService<IStateProvinceService>();

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            //zipPostalCode, Country, StateProvince 
            ["Address.LineFormat"] = "{6}{0}{1}"
        });

        _addressService = GetService<IAddressService>();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Address.LineFormat"] = "{0}{1}{2}{3}{4}{5}{6}"
        });
    }

    [Test]
    public async Task CanFormatAddressAsync()
    {
        var address = await _addressService.GetAddressByIdAsync(1);

        var country = await _countryService.GetCountryByAddressAsync(address);
        var countryName = country != null ? await _localizationService.GetLocalizedAsync(country, x => x.Name, 1) : string.Empty;
        var stateProvince = await _stateProvinceService.GetStateProvinceByAddressAsync(address);
        var stateProvinceName = stateProvince != null ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, 1) : string.Empty;

        var (addressLine, addressFields) = await _addressService.FormatAddressAsync(address, 1);

        addressFields[0].Value.Should().Be(address.ZipPostalCode);
        addressFields[1].Value.Should().Be(countryName);
        addressFields[2].Value.Should().Be(stateProvinceName);

        addressLine.Should().Be($"{address.ZipPostalCode}, {countryName}, {stateProvinceName}");
    }
}
