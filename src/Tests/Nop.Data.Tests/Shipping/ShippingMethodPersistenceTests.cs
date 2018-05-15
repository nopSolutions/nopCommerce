using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Shipping
{
    [TestFixture]
    public class ShippingMethodPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_shippingMethod()
        {
            var shippingMethod = this.GetTestShippingMethod();

            var fromDb = SaveAndLoadEntity(this.GetTestShippingMethod());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(shippingMethod);
        }

        [Test]
        public void Can_save_and_load_shippingMethod_with_restriction()
        {
            var shippingMethod = this.GetTestShippingMethod();
            shippingMethod.RestrictedCountries.Add(this.GetTestCountry());

            var fromDb = SaveAndLoadEntity(shippingMethod);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestShippingMethod());

            fromDb.RestrictedCountries.ShouldNotBeNull();
            (fromDb.RestrictedCountries.Count == 1).ShouldBeTrue();
            fromDb.RestrictedCountries.First().PropertiesShouldEqual(this.GetTestCountry());
        }
    }
}