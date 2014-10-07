using System;
using Nop.Core.Domain.Catalog;
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
            var productSpecificationAttribute = new ProductSpecificationAttribute
            {
                AttributeType = SpecificationAttributeType.Hyperlink,
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                Product = new Product
                {
                    Name = "Name 1",
                    Published = true,
                    Deleted = false,
                    CreatedOnUtc = new DateTime(2010, 01, 01),
                    UpdatedOnUtc = new DateTime(2010, 01, 02)
                },
                SpecificationAttributeOption = new SpecificationAttributeOption
                {
                    Name = "SpecificationAttributeOption name 1",
                    DisplayOrder = 1,
                    SpecificationAttribute = new SpecificationAttribute
                    {
                        Name = "SpecificationAttribute name 1",
                        DisplayOrder = 2,
                    }
                }
            };

            var fromDb = SaveAndLoadEntity(productSpecificationAttribute);
            fromDb.ShouldNotBeNull();
            fromDb.AttributeType.ShouldEqual(SpecificationAttributeType.Hyperlink);
            fromDb.AllowFiltering.ShouldEqual(true);
            fromDb.ShowOnProductPage.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.Name.ShouldEqual("Name 1");

            fromDb.SpecificationAttributeOption.ShouldNotBeNull();
            fromDb.SpecificationAttributeOption.Name.ShouldEqual("SpecificationAttributeOption name 1");
        }
    }
}
