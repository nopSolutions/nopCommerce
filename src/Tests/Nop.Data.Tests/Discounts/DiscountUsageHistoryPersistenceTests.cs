using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Discounts
{
    [TestFixture]
    public class DiscountUsageHistoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_discountUsageHistory()
        {
            var discount = this.GetTestDiscountUsageHistory();
         
            var fromDb = SaveAndLoadEntity(this.GetTestDiscountUsageHistory());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(discount);

            fromDb.Discount.ShouldNotBeNull();
            fromDb.Order.ShouldNotBeNull();
            fromDb.Order.PropertiesShouldEqual(discount.Order);
        }
    }
}