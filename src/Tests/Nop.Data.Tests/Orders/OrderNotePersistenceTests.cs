using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class OrderNotePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_orderNote()
        {
            var on = this.GetTestOrderNote();
            on.Order = this.GetTestOrder();
            on.Order.Customer = this.GetTestCustomer();
            var fromDb = SaveAndLoadEntity(on);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestOrderNote());

            fromDb.Order.ShouldNotBeNull();
            fromDb.Order.PropertiesShouldEqual(this.GetTestOrder());
        }
    }
}