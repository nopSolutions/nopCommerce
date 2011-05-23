using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Tests
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
                DisplayOrder = 1
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
        }

        [Test]
        public void Can_save_and_load_language_with_localeStringResources()
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
                StateProvinces = new List<StateProvince>()
                {
                    new StateProvince()
                    {
                        Name = "California",
                        Abbreviation= "CA",
                        DisplayOrder = 1
                    }
                }
            };

            var fromDb = SaveAndLoadEntity(country);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("United States");

            fromDb.StateProvinces.ShouldNotBeNull();
            (fromDb.StateProvinces.Count == 1).ShouldBeTrue();
            fromDb.StateProvinces.First().Name.ShouldEqual("California");
        }
    }
}
