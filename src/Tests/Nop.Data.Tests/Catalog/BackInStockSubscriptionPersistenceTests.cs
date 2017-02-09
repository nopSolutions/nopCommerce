using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class BackInStockSubscriptionPersistenceTests : PersistenceTest
    {
        [OneTimeSetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void Can_save_and_load_backInStockSubscription()
        {
            var backInStockSubscription = this.GetTestBackInStockSubscription();

            var fromDb = SaveAndLoadEntity(this.GetTestBackInStockSubscription());
            fromDb.ShouldNotBeNull();
            fromDb.Product.ShouldNotBeNull();
            fromDb.Customer.ShouldNotBeNull();

            fromDb.PropertiesShouldEqual(backInStockSubscription);
            fromDb.Product.PropertiesShouldEqual(this.GetTestProduct());
            fromDb.Customer.PropertiesShouldEqual(this.GetTestCustomer());
        }        
    }
}
