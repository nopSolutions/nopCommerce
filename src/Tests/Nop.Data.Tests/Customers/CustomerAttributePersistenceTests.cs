using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Customers
{
    [TestFixture]
    public class CustomerAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_customerAttribute()
        {
            var ca = this.GetTestCustomerAttribute();

            var fromDb = SaveAndLoadEntity(this.GetTestCustomerAttribute());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(ca);
        }

        [Test]
        public void Can_save_and_load_customerAttribute_with_values()
        {
            var ca = this.GetTestCustomerAttribute();
            ca.CustomerAttributeValues.Add(this.GetTestCustomerAttributeValue());
            var fromDb = SaveAndLoadEntity(ca);
            fromDb.PropertiesShouldEqual(this.GetTestCustomerAttribute());

            fromDb.CustomerAttributeValues.ShouldNotBeNull();
            (fromDb.CustomerAttributeValues.Count == 1).ShouldBeTrue();
            fromDb.CustomerAttributeValues.First().PropertiesShouldEqual(this.GetTestCustomerAttributeValue());
        }
    }
}