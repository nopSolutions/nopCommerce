using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductSpecificationAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productSpecificationAttribute()
        {
            var productSpecificationAttribute = this.GetTestProductSpecificationAttribute();

            var fromDb = SaveAndLoadEntity(this.GetTestProductSpecificationAttribute());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(productSpecificationAttribute);

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.PropertiesShouldEqual(productSpecificationAttribute.Product);

            fromDb.SpecificationAttributeOption.ShouldNotBeNull();
            fromDb.SpecificationAttributeOption.PropertiesShouldEqual(productSpecificationAttribute.SpecificationAttributeOption);

            fromDb.SpecificationAttributeOption.SpecificationAttribute.ShouldNotBeNull();
            fromDb.SpecificationAttributeOption.SpecificationAttribute.PropertiesShouldEqual(productSpecificationAttribute.SpecificationAttributeOption.SpecificationAttribute);
        }
    }
}
