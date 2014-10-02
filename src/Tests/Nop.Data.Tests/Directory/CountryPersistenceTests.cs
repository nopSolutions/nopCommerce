using System.Linq;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Shipping;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Directory
{
    [TestFixture]
    public class CountryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_country()
        {
            var country = new Country
            {
                Name = "United States",
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 1,
                SubjectToVat = true,
                Published = true,
                DisplayOrder = 1,
                LimitedToStores = true
            };

            var fromDb = SaveAndLoadEntity(country);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("United States");
            fromDb.AllowsBilling.ShouldEqual(true);
            fromDb.AllowsShipping.ShouldEqual(true);
            fromDb.TwoLetterIsoCode.ShouldEqual("US");
            fromDb.ThreeLetterIsoCode.ShouldEqual("USA");
            fromDb.NumericIsoCode.ShouldEqual(1);
            fromDb.SubjectToVat.ShouldEqual(true);
            fromDb.Published.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);
            fromDb.LimitedToStores.ShouldEqual(true);
        }

        [Test]
        public void Can_save_and_load_country_with_states()
        {
            var country = new Country
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
            country.StateProvinces.Add
                (
                    new StateProvince
                    {
                        Name = "California",
                        Abbreviation = "CA",
                        DisplayOrder = 1
                    }
                );
            var fromDb = SaveAndLoadEntity(country);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("United States");

            fromDb.StateProvinces.ShouldNotBeNull();
            (fromDb.StateProvinces.Count == 1).ShouldBeTrue();
            fromDb.StateProvinces.First().Name.ShouldEqual("California");
        }

        [Test]
        public void Can_save_and_load_country_with_restrictions()
        {
            var country = new Country
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
            country.RestrictedShippingMethods.Add
                (
                    new ShippingMethod
                    {
                        Name = "By train",
                    }
                );
            var fromDb = SaveAndLoadEntity(country);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("United States");

            fromDb.RestrictedShippingMethods.ShouldNotBeNull();
            (fromDb.RestrictedShippingMethods.Count == 1).ShouldBeTrue();
            fromDb.RestrictedShippingMethods.First().Name.ShouldEqual("By train");
        }
    }
}
