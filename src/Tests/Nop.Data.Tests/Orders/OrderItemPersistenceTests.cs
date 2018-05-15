using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class OrderItemPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_orderItem()
        {
            var orderItem = this.GetTestOrderItem();
            orderItem.Order = this.GetTestOrder();
            orderItem.Order.Customer = this.GetTestCustomer();
            var fromDb = SaveAndLoadEntity(orderItem);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestOrderItem());

            fromDb.Order.ShouldNotBeNull();
            fromDb.Order.PropertiesShouldEqual(this.GetTestOrder());
        }

        [Test]
        public void Can_save_and_load_orderItem_with_giftCard()
        {
            var orderItem = this.GetTestOrderItem();
            orderItem.Order = this.GetTestOrder();
            orderItem.Order.Customer = this.GetTestCustomer();
            orderItem.AssociatedGiftCards.Add(this.GetTestGiftCard());

            var fromDb = SaveAndLoadEntity(orderItem);
            fromDb.ShouldNotBeNull();

            fromDb.AssociatedGiftCards.ShouldNotBeNull();
            (fromDb.AssociatedGiftCards.Count == 1).ShouldBeTrue();
            fromDb.AssociatedGiftCards.First().PropertiesShouldEqual(this.GetTestGiftCard());
        }
    }
}