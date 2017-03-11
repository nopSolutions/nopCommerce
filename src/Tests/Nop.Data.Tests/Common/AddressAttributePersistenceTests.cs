using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Common
{
    [TestFixture]
    public class AddressAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_addressAttribute()
        {
            var ca = this.GetTestAddressAttribute();

            var fromDb = SaveAndLoadEntity(this.GetTestAddressAttribute());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(ca);
        }

        [Test]
        public void Can_save_and_load_addressAttribute_with_values()
        {
            var ca = this.GetTestAddressAttribute();
            ca.AddressAttributeValues.Add(this.GetTestAddressAttributeValue());
            var fromDb = SaveAndLoadEntity(ca);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestAddressAttribute());
            fromDb.AddressAttributeValues.ShouldNotBeNull();
            (fromDb.AddressAttributeValues.Count == 1).ShouldBeTrue();
            fromDb.AddressAttributeValues.First().PropertiesShouldEqual(this.GetTestAddressAttributeValue());
        }
    }
}