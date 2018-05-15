using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class SpecificationAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_specificationAttribute()
        {
            var specificationAttribute = this.GetTestSpecificationAttribute();

            var fromDb = SaveAndLoadEntity(this.GetTestSpecificationAttribute());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(specificationAttribute);
        }

        [Test]
        public void Can_save_and_load_specificationAttribute_with_specificationAttributeOptions()
        {
            var specificationAttribute = this.GetTestSpecificationAttribute();
            specificationAttribute.SpecificationAttributeOptions.Add(this.GetTestSpecificationAttributeOption());
            var fromDb = SaveAndLoadEntity(specificationAttribute);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestSpecificationAttribute());

            fromDb.SpecificationAttributeOptions.ShouldNotBeNull();
            (fromDb.SpecificationAttributeOptions.Count == 1).ShouldBeTrue();
            fromDb.SpecificationAttributeOptions.First().PropertiesShouldEqual(this.GetTestSpecificationAttributeOption());
        }
    }
}
