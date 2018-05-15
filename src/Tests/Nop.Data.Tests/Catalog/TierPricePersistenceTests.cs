using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class TierPricePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_tierPrice()
        {
            var tierPrice = this.GetTestTierPrice();
            tierPrice.Product = this.GetTestProduct();
            var fromDb = SaveAndLoadEntity(tierPrice);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestTierPrice());
            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.PropertiesShouldEqual(this.GetTestProduct());
        }

        [Test]
        public void Can_save_and_load_tierPriceWithCustomerRole()
        {
            var tierPrice = this.GetTestTierPrice();
            tierPrice.CustomerRole = this.GetTestCustomerRole();
            tierPrice.Product = this.GetTestProduct();

            var fromDb = SaveAndLoadEntity(tierPrice);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestTierPrice());

            fromDb.CustomerRole.ShouldNotBeNull();
            fromDb.CustomerRole.PropertiesShouldEqual(this.GetTestCustomerRole());
        }
    }
}
