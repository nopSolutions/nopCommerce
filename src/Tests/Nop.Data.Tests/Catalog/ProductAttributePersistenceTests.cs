using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productAttribute()
        {
            var pa = this.GetTestProductAttribute();

            var fromDb = SaveAndLoadEntity(this.GetTestProductAttribute());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(pa);
        }       
    }
}