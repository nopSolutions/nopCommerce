using System;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Tests;
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
                Address1 = "Address1",
                Address2 = "Address2",
                ZipPostalCode = "ZipPostalCode 1",
                PhoneNumber = "PhoneNumber 1",
                FaxNumber = "FaxNumber 1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
            };

            var newAddress = address.Clone() as Address;
            newAddress.ShouldNotBeNull();
            newAddress.Id.ShouldEqual(0);
            newAddress.FirstName.ShouldEqual("FirstName 1");
            newAddress.LastName.ShouldEqual("LastName 1");
            newAddress.Email.ShouldEqual("Email 1");
            newAddress.Company.ShouldEqual("Company 1");
            newAddress.City.ShouldEqual("City 1");
            newAddress.Address1.ShouldEqual("Address1");
            newAddress.Address2.ShouldEqual("Address2");
            newAddress.ZipPostalCode.ShouldEqual("ZipPostalCode 1");
            newAddress.PhoneNumber.ShouldEqual("PhoneNumber 1");
            newAddress.FaxNumber.ShouldEqual("FaxNumber 1");
            newAddress.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            newAddress.Country.ShouldNotBeNull();
            newAddress.CountryId.ShouldEqual(3);
            newAddress.Country.Name.ShouldEqual("United States");

            newAddress.StateProvince.ShouldNotBeNull();
            newAddress.StateProvinceId.ShouldEqual(4);
            newAddress.StateProvince.Name.ShouldEqual("LA");
        }
    }
}
