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
            var address = this.GetTestAddress();
            address.Country = this.GetTestCountry();

            var fromDb = SaveAndLoadEntity(address);
            fromDb.ShouldNotBeNull();

            fromDb.PropertiesShouldEqual(this.GetTestAddress());

            fromDb.Country.ShouldNotBeNull();
            fromDb.Country.PropertiesShouldEqual(this.GetTestCountry());
        }

        [Test]
        public void Can_save_and_load_address_with_stateProvince()
        {
            var address = this.GetTestAddress();
            address.Country = this.GetTestCountry();
            address.StateProvince = this.GetTestStateProvince();

            var fromDb = SaveAndLoadEntity(address);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestAddress());
            fromDb.StateProvince.ShouldNotBeNull();
            fromDb.StateProvince.PropertiesShouldEqual(this.GetTestStateProvince());
        }
    }
}
