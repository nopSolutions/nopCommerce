using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Shipping
{
    [TestFixture]
    public class ShipmentPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_shipment()
        {
            var shipment = this.GetTestShipment();
            shipment.Order.Customer = this.GetTestCustomer();
            var fromDb = SaveAndLoadEntity(shipment);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestShipment());
        }

        [Test]
        public void Can_save_and_load_shipment_with_items()
        {
            var shipment = this.GetTestShipment();
            shipment.ShipmentItems.Add(this.GetTestShipmentItem());
            shipment.Order.Customer = this.GetTestCustomer();
            var fromDb = SaveAndLoadEntity(shipment);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestShipment());

            fromDb.ShipmentItems.ShouldNotBeNull();
            (fromDb.ShipmentItems.Count == 1).ShouldBeTrue();
            fromDb.ShipmentItems.First().PropertiesShouldEqual(this.GetTestShipmentItem());
        }
    }
}