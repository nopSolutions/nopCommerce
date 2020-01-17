using System;
using FluentAssertions;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.Common
{
    [TestFixture]
    public class AddressTests
    {
        [Test]
        public void Can_clone_address()
        {
            var address = new Address
            {
                Id = 1,
                FirstName = "FirstName 1",
                LastName = "LastName 1",
                Email = "Email 1",
                Company = "Company 1",
                CountryId = 3,
                Country = new Country { Id = 3, Name = "United States" },
                StateProvinceId = 4,
                StateProvince = new StateProvince { Id = 4, Name = "LA" },
                City = "City 1",
                County = "County 1",
                Address1 = "Address1",
                Address2 = "Address2",
                ZipPostalCode = "ZipPostalCode 1",
                PhoneNumber = "PhoneNumber 1",
                FaxNumber = "FaxNumber 1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
            };

            var newAddress = address.Clone() as Address;
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

            newAddress.Country.Should().NotBeNull();
            newAddress.CountryId.Should().Be(3);
            newAddress.Country.Name.Should().Be("United States");

            newAddress.StateProvince.Should().NotBeNull();
            newAddress.StateProvinceId.Should().Be(4);
            newAddress.StateProvince.Name.Should().Be("LA");
        }
    }
}
