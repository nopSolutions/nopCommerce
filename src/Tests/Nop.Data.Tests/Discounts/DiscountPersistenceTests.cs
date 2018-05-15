using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Discounts
{
    [TestFixture]
    public class DiscountPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_discount()
        {
            var discount = this.GetTestDiscount();

            var fromDb = SaveAndLoadEntity(this.GetTestDiscount());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(discount);
        }

        [Test]
        public void Can_save_and_load_discount_with_discountRequirements()
        {
            var discount = this.GetTestDiscount();
            discount.DiscountRequirements.Add(this.GetTestDiscountRequirement());
            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestDiscount());
            
            fromDb.DiscountRequirements.ShouldNotBeNull();
            (fromDb.DiscountRequirements.Count == 1).ShouldBeTrue();
            fromDb.DiscountRequirements.First().PropertiesShouldEqual(this.GetTestDiscountRequirement());
        }

        [Test]
        public void Can_save_and_load_discount_with_appliedProducts()
        {
            var discount = this.GetTestDiscount();
            discount.AppliedToProducts.Add(this.GetTestProduct());
            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestDiscount());

            fromDb.AppliedToProducts.ShouldNotBeNull();
            (fromDb.AppliedToProducts.Count == 1).ShouldBeTrue();
            fromDb.AppliedToProducts.First().PropertiesShouldEqual(this.GetTestProduct());

        }

        [Test]
        public void Can_save_and_load_discount_with_appliedCategories()
        {
            var discount = this.GetTestDiscount();
            discount.AppliedToCategories.Add(this.GetTestCategory());
            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestDiscount());

            fromDb.AppliedToCategories.ShouldNotBeNull();
            (fromDb.AppliedToCategories.Count == 1).ShouldBeTrue();
            fromDb.AppliedToCategories.First().PropertiesShouldEqual(this.GetTestCategory());
        }
    }
}