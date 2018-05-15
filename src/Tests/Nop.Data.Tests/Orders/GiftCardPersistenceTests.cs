using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class GiftCardPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_giftCard()
        {
            var giftCard = this.GetTestGiftCard();

            var fromDb = SaveAndLoadEntity(this.GetTestGiftCard());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(giftCard);
        }

        [Test]
        public void Can_save_and_load_giftCard_with_usageHistory()
        {
            var giftCard = this.GetTestGiftCard();
            var gcuh = this.GetTestGiftCardUsageHistory();
            gcuh.UsedWithOrder = this.GetTestOrder();
            gcuh.UsedWithOrder.Customer = this.GetTestCustomer();
            gcuh.GiftCard = null;
            giftCard.GiftCardUsageHistory.Add(gcuh);
            var fromDb = SaveAndLoadEntity(giftCard);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestGiftCard());

            fromDb.GiftCardUsageHistory.ShouldNotBeNull();
            (fromDb.GiftCardUsageHistory.Count == 1).ShouldBeTrue();
            fromDb.GiftCardUsageHistory.First().PropertiesShouldEqual(this.GetTestGiftCardUsageHistory());
        }
        
        [Test]
        public void Can_save_and_load_giftCard_with_associatedItem()
        {
            var giftCard = this.GetTestGiftCard();
            giftCard.PurchasedWithOrderItem = this.GetTestOrderItem();
            giftCard.PurchasedWithOrderItem.Order = this.GetTestOrder();
            giftCard.PurchasedWithOrderItem.Order.Customer = this.GetTestCustomer();
            var fromDb = SaveAndLoadEntity(giftCard);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestGiftCard());

            fromDb.PurchasedWithOrderItem.ShouldNotBeNull();
            fromDb.PurchasedWithOrderItem.Product.ShouldNotBeNull();
            fromDb.PurchasedWithOrderItem.Product.PropertiesShouldEqual(this.GetTestProduct());
        }
    }
}