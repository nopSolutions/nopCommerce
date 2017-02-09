using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class CheckoutAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_checkoutAttribute()
        {
            var checkoutAttribute = this.GetTestCheckoutAttribute();

            var fromDb = SaveAndLoadEntity(this.GetTestCheckoutAttribute());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(checkoutAttribute);
        }

        [Test]
        public void Can_save_and_load_checkoutAttribute_with_values()
        {
            var checkoutAttribute = this.GetTestCheckoutAttribute();

            checkoutAttribute.CheckoutAttributeValues.Add(this.GetTestCheckoutAttributeValue());
            var fromDb = SaveAndLoadEntity(checkoutAttribute);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestCheckoutAttribute());

            fromDb.CheckoutAttributeValues.ShouldNotBeNull();
            (fromDb.CheckoutAttributeValues.Count == 1).ShouldBeTrue();
            fromDb.CheckoutAttributeValues.First().PropertiesShouldEqual(this.GetTestCheckoutAttributeValue());
        }
    }
}