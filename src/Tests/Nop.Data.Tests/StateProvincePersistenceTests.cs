using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class StateProvincePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_stateProvince()
        {
            var stateProvince = new StateProvince
            {
                Name = "California",
                Abbreviation = "CA",
                DisplayOrder = 1,
                Country = new Country()
                               {
                                   Name = "United States",
                                   AllowsRegistration = true,
                                   AllowsBilling = true,
                                   AllowsShipping = true,
                                   TwoLetterIsoCode = "US",
                                   ThreeLetterIsoCode = "USA",
                                   NumericIsoCode = 1,
                                   SubjectToVat = true,
                                   Published = true,
                                   DisplayOrder = 1,
                               }
            };

            var fromDb = SaveAndLoadEntity(stateProvince);
            fromDb.Name.ShouldEqual("California");
            fromDb.Abbreviation.ShouldEqual("CA");
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.Country.ShouldNotBeNull();
            fromDb.Country.Name.ShouldEqual("United States");
        }
    }
}
