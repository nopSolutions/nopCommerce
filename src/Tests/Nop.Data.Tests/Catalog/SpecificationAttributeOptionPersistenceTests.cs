using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class SpecificationAttributeOptionPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_specificationAttributeOption()
        {
            var specificationAttributeOption = this.GetTestSpecificationAttributeOption();
            specificationAttributeOption.SpecificationAttribute = this.GetTestSpecificationAttribute();

            var fromDb = SaveAndLoadEntity(specificationAttributeOption);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestSpecificationAttributeOption());

            fromDb.SpecificationAttribute.ShouldNotBeNull();
            fromDb.SpecificationAttribute.PropertiesShouldEqual(this.GetTestSpecificationAttribute());
        }
    }
}
