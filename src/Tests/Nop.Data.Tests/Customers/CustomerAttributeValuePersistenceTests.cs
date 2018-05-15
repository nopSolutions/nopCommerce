using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Customers
{
    [TestFixture]
    public class CheckoutAttributeValuePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_customerAttributeValue()
        {
            var cav = this.GetTestCustomerAttributeValue();
            cav.CustomerAttribute = this.GetTestCustomerAttribute();

            var fromDb = SaveAndLoadEntity(cav);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestCustomerAttributeValue());

            fromDb.CustomerAttribute.ShouldNotBeNull();
            fromDb.CustomerAttribute.PropertiesShouldEqual(this.GetTestCustomerAttribute());
        }
    }
}