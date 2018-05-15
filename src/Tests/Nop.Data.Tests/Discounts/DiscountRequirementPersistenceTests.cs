using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Discounts
{
    [TestFixture]
    public class DiscountRequirementPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_discountRequirement()
        {
            var discountRequirement = this.GetTestDiscountRequirement();
            discountRequirement.Discount = this.GetTestDiscount();

            var fromDb = SaveAndLoadEntity(discountRequirement);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestDiscountRequirement());

            fromDb.Discount.ShouldNotBeNull();
            fromDb.Discount.PropertiesShouldEqual(this.GetTestDiscount());
        }
    }
}