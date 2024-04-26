using FluentAssertions;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Domain.Common;

[TestFixture]
public class AddressTests : BaseNopTest
{
    private IAddressService _addressService;

    [OneTimeSetUp]
    public void SetUp()
    {
        _addressService = GetService<IAddressService>();
    }

    [Test]
    public void CanCloneAddress()
    {
        var address = new Address
        {
            Id = 1,
            FirstName = "FirstName 1",
            LastName = "LastName 1",
            Email = "Email 1",
            Company = "Company 1",
            CountryId = 3,
            StateProvinceId = 4,
            City = "City 1",
            County = "County 1",
            Address1 = "Address1",
            Address2 = "Address2",
            ZipPostalCode = "ZipPostalCode 1",
            PhoneNumber = "PhoneNumber 1",
            FaxNumber = "FaxNumber 1",
            CreatedOnUtc = new DateTime(2010, 01, 01),
        };

        var newAddress = _addressService.CloneAddress(address);
        newAddress.Should().NotBeNull();
        newAddress.Id.Should().Be(0);
        newAddress.FirstName.Should().Be("FirstName 1");
        newAddress.LastName.Should().Be("LastName 1");
        newAddress.Email.Should().Be("Email 1");
        newAddress.Company.Should().Be("Company 1");
        newAddress.City.Should().Be("City 1");
        newAddress.County.Should().Be("County 1");
        newAddress.Address1.Should().Be("Address1");
        newAddress.Address2.Should().Be("Address2");
        newAddress.ZipPostalCode.Should().Be("ZipPostalCode 1");
        newAddress.PhoneNumber.Should().Be("PhoneNumber 1");
        newAddress.FaxNumber.Should().Be("FaxNumber 1");
        newAddress.CreatedOnUtc.Should().Be(new DateTime(2010, 01, 01));
        newAddress.CountryId.Should().Be(3);
        newAddress.StateProvinceId.Should().Be(4);
    }

    [Test]
    public async Task CanValidateAddressAsync()
    {
        //Valid address
        var validAddress = new Address
        {
            Id = 1,
            FirstName = "FirstName 1",
            LastName = "LastName 1",
            Email = "Email 1",
            Company = "Company 1",
            CountryId = 3,
            StateProvinceId = 4,
            City = "City 1",
            County = "County 1",
            Address1 = "Address1",
            Address2 = "Address2",
            ZipPostalCode = "ZipPostalCode 1",
            PhoneNumber = "PhoneNumber 1",
            FaxNumber = "FaxNumber 1",
            CreatedOnUtc = new DateTime(2010, 01, 01),
        };

        var isAddressValid = await _addressService.IsAddressValidAsync(validAddress);
        isAddressValid.Should().BeTrue();

        //InValid address 1 - empty FirstName
        var invalidAddress1 = new Address
        {
            Id = 1,
            FirstName = "",
            LastName = "LastName 1",
            Email = "Email 1",
            Company = "Company 1",
            CountryId = 3,
            StateProvinceId = 4,
            City = "City 1",
            County = "County 1",
            Address1 = "Address1",
            Address2 = "Address2",
            ZipPostalCode = "ZipPostalCode 1",
            PhoneNumber = "PhoneNumber 1",
            FaxNumber = "FaxNumber 1",
            CreatedOnUtc = new DateTime(2010, 01, 01),
        };

        isAddressValid = await _addressService.IsAddressValidAsync(invalidAddress1);
        isAddressValid.Should().BeFalse();

        //InValid address 2 - empty LastName
        var invalidAddress2 = new Address
        {
            Id = 1,
            FirstName = "FirstName 1",
            LastName = "",
            Email = "Email 1",
            Company = "Company 1",
            CountryId = 3,
            StateProvinceId = 4,
            City = "City 1",
            County = "County 1",
            Address1 = "Address1",
            Address2 = "Address2",
            ZipPostalCode = "ZipPostalCode 1",
            PhoneNumber = "PhoneNumber 1",
            FaxNumber = "FaxNumber 1",
            CreatedOnUtc = new DateTime(2010, 01, 01),
        };

        isAddressValid = await _addressService.IsAddressValidAsync(invalidAddress2);
        isAddressValid.Should().BeFalse();

        //InValid address 3 - empty Email
        var invalidAddress3 = new Address
        {
            Id = 1,
            FirstName = "FirstName 1",
            LastName = "LastName 1",
            Email = "",
            Company = "Company 1",
            CountryId = 3,
            StateProvinceId = 4,
            City = "City 1",
            County = "County 1",
            Address1 = "Address1",
            Address2 = "Address2",
            ZipPostalCode = "ZipPostalCode 1",
            PhoneNumber = "PhoneNumber 1",
            FaxNumber = "FaxNumber 1",
            CreatedOnUtc = new DateTime(2010, 01, 01),
        };

        isAddressValid = await _addressService.IsAddressValidAsync(invalidAddress3);
        isAddressValid.Should().BeFalse();
    }
}