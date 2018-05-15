using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductAttributeMappingPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productAttributeMapping()
        {
            var productAttributeMapping = this.GetTestProductAttributeMapping();

            var fromDb = SaveAndLoadEntity(this.GetTestProductAttributeMapping());
            fromDb.ShouldNotBeNull();
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(productAttributeMapping);
            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.PropertiesShouldEqual(productAttributeMapping.Product);
        }
    }
}
