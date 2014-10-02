using Nop.Core.Domain.Directory;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Directory
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
                Published = true,
                DisplayOrder = 1,
                Country = new Country
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
                               }
            };

            var fromDb = SaveAndLoadEntity(stateProvince);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("California");
            fromDb.Abbreviation.ShouldEqual("CA");
            fromDb.Published.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.Country.ShouldNotBeNull();
            fromDb.Country.Name.ShouldEqual("United States");
        }
    }
}
