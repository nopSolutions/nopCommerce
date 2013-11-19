using System.Linq;
using Nop.Core.Domain.Catalog;
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
            var specificationAttribute = new SpecificationAttribute
            {
                Name = "Name 1",
                DisplayOrder = 1,
            };

            var fromDb = SaveAndLoadEntity(specificationAttribute);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.DisplayOrder.ShouldEqual(1);
        }

        [Test]
        public void Can_save_and_load_specificationAttribute_with_specificationAttributeOptions()
        {
            var specificationAttribute = new SpecificationAttribute
            {
                Name = "Name 1",
                DisplayOrder = 1
            };
            specificationAttribute.SpecificationAttributeOptions.Add
                (
                    new SpecificationAttributeOption
                    {
                        Name = "SpecificationAttributeOption name 1",
                        DisplayOrder = 1,
                    }
                );
            var fromDb = SaveAndLoadEntity(specificationAttribute);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");


            fromDb.SpecificationAttributeOptions.ShouldNotBeNull();
            (fromDb.SpecificationAttributeOptions.Count == 1).ShouldBeTrue();
            fromDb.SpecificationAttributeOptions.First().Name.ShouldEqual("SpecificationAttributeOption name 1");
        }
    }
}
