using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Common
{
    [TestFixture]
    public class CheckoutAttributeValuePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_addressAttributeValue()
        {
            var cav = this.GetTestAddressAttributeValue();
            cav.AddressAttribute = this.GetTestAddressAttribute();

            var fromDb = SaveAndLoadEntity(cav);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestAddressAttributeValue());

            fromDb.AddressAttribute.ShouldNotBeNull();
            fromDb.AddressAttribute.PropertiesShouldEqual(this.GetTestAddressAttribute());
        }
    }
}