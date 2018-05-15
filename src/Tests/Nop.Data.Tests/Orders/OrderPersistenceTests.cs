using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class OrderPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_order()
        {
            var order = this.GetTestOrder();

            order.Customer = this.GetTestCustomer();
            order.PickupAddress = this.GetTestAddress();

            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestOrder());
            fromDb.Customer.PropertiesShouldEqual(this.GetTestCustomer());
            fromDb.PickupAddress.PropertiesShouldEqual(this.GetTestAddress());
            fromDb.BillingAddress.PropertiesShouldEqual(this.GetTestAddress());
        }

        [Test]
        public void Can_save_and_load_order_with_shipping_address()
        {
            var order = this.GetTestOrder();

            order.Customer = this.GetTestCustomer();
            order.PickupAddress = this.GetTestAddress();
            order.ShippingAddress = this.GetTestAddress();

            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();
            fromDb.ShippingAddress.ShouldNotBeNull();
            fromDb.ShippingAddress.PropertiesShouldEqual(this.GetTestAddress());
        }

        [Test]
        public void Can_save_and_load_order_with_usedRewardPoints()
        {
            var order = this.GetTestOrder();
            order.RedeemedRewardPointsEntry = this.GetTestRewardPointsHistory();
            order.RedeemedRewardPointsEntry.UsedWithOrder = order;

            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestOrder());

            fromDb.RedeemedRewardPointsEntry.ShouldNotBeNull();
            fromDb.RedeemedRewardPointsEntry.PropertiesShouldEqual(this.GetTestRewardPointsHistory());
        }

        [Test]
        public void Can_save_and_load_order_with_discountUsageHistory()
        {
            var order = this.GetTestOrder();
            var discountUsageHistory = this.GetTestDiscountUsageHistory();
            order.Customer = this.GetTestCustomer();
            discountUsageHistory.Order = order;
            order.DiscountUsageHistory.Add(discountUsageHistory);

            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestOrder());

            fromDb.DiscountUsageHistory.ShouldNotBeNull();
            fromDb.DiscountUsageHistory.First().PropertiesShouldEqual(this.GetTestDiscountUsageHistory());
        }

        [Test]
        public void Can_save_and_load_order_with_giftCardUsageHistory()
        {
            var order = this.GetTestOrder();
            order.GiftCardUsageHistory.Add(this.GetTestGiftCardUsageHistory());
            order.Customer = this.GetTestCustomer();
            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.GiftCardUsageHistory.ShouldNotBeNull();
            fromDb.GiftCardUsageHistory.Count.ShouldEqual(1);
            fromDb.GiftCardUsageHistory.First().PropertiesShouldEqual(this.GetTestGiftCardUsageHistory());
        }
        
        [Test]
        public void Can_save_and_load_order_with_orderNotes()
        {
            var order = this.GetTestOrder();
            order.OrderNotes.Add(this.GetTestOrderNote());
            order.Customer = this.GetTestCustomer();
            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.OrderNotes.ShouldNotBeNull();
            fromDb.OrderNotes.Count.ShouldEqual(1);
            fromDb.OrderNotes.First().PropertiesShouldEqual(this.GetTestOrderNote());
        }

        [Test]
        public void Can_save_and_load_order_with_orderItems()
        {
            var order = this.GetTestOrder();
            order.OrderItems.Add(this.GetTestOrderItem());
            order.Customer = this.GetTestCustomer();
            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.OrderItems.ShouldNotBeNull();
            fromDb.OrderItems.Count.ShouldEqual(1);
            fromDb.OrderItems.First().PropertiesShouldEqual(this.GetTestOrderItem());
        }
        
        [Test]
        public void Can_save_and_load_order_with_shipments()
        {
            var order = this.GetTestOrder();
            order.Shipments.Add(this.GetTestShipment());
            order.Shipments.First().Order = order;
            order.Customer = this.GetTestCustomer();
            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.Shipments.ShouldNotBeNull();
            fromDb.Shipments.Count.ShouldEqual(1);
            fromDb.Shipments.First().PropertiesShouldEqual(this.GetTestShipment());
        }
    }
}
