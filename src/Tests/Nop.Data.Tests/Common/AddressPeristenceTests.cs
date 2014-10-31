using System;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Common
{
    [TestFixture]
    public class AddressPeristenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_address()
        {
            var address = new Address
            {
                FirstName = "FirstName 1",
                LastName = "LastName 1",
                Email = "Email 1",
                Company = "Company 1",
                City = "City 1",
                Address1 = "Address1",
                Address2 = "Address2",
                ZipPostalCode = "ZipPostalCode 1",
                PhoneNumber = "PhoneNumber 1",
                FaxNumber = "FaxNumber 1",
                CustomAttributes = "CustomAttributes 1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                Country = GetTestCountry()
            };

            var fromDb = SaveAndLoadEntity(address);
            fromDb.ShouldNotBeNull();

            fromDb.FirstName.ShouldEqual("FirstName 1");
            fromDb.LastName.ShouldEqual("LastName 1");
            fromDb.Email.ShouldEqual("Email 1");
            fromDb.Company.ShouldEqual("Company 1");
            fromDb.City.ShouldEqual("City 1");
            fromDb.Address1.ShouldEqual("Address1");
            fromDb.Address2.ShouldEqual("Address2");
            fromDb.ZipPostalCode.ShouldEqual("ZipPostalCode 1");
            fromDb.PhoneNumber.ShouldEqual("PhoneNumber 1");
            fromDb.FaxNumber.ShouldEqual("FaxNumber 1");
            fromDb.CustomAttributes.ShouldEqual("CustomAttributes 1");
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            fromDb.Country.ShouldNotBeNull();
            fromDb.Country.Name.ShouldEqual("United States");

        }

        [Test]
        public void Can_save_and_load_address_with_stateProvince()
        {
            var address = new Address
            {
                FirstName = "FirstName 1",
                LastName = "LastName 1",
                Email = "Email 1",
                Company = "Company 1",
                City = "City 1",
                Address1 = "Address1",
                Address2 = "Address2",
                ZipPostalCode = "ZipPostalCode 1",
                PhoneNumber = "PhoneNumber 1",
                FaxNumber = "FaxNumber 1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                Country = GetTestCountry(),
                StateProvince = GetTestStateProvince(),
            };

            var fromDb = SaveAndLoadEntity(address);
            fromDb.ShouldNotBeNull();

            fromDb.StateProvince.ShouldNotBeNull();
            fromDb.StateProvince.Name.ShouldEqual("California");
        }

        protected Country GetTestCountry()
        {
            return new Country
            {
                Name = "United States",
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 1,
                SubjectToVat = true,
                Published = true,
                DisplayOrder = 1
            };
        }

        protected StateProvince GetTestStateProvince()
        {
            return new StateProvince
            {
                Name = "California",
                Abbreviation = "CA",
                DisplayOrder = 1,
                Country = GetTestCountry()
            };
        }
    }
}
