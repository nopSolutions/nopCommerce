using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Shipping
{
    [TestFixture]
    public class ShipmentItemPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_shipmentItem()
        {
            var shipmentItem = this.GetTestShipmentItem();
            shipmentItem.Shipment = this.GetTestShipment();
            shipmentItem.Shipment.Order.Customer = this.GetTestCustomer();
            var fromDb = SaveAndLoadEntity(shipmentItem);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestShipmentItem());

            fromDb.Shipment.ShouldNotBeNull();
            fromDb.Shipment.PropertiesShouldEqual(this.GetTestShipment());
        }
    }
}