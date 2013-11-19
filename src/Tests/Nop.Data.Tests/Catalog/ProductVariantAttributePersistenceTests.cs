using System;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductVariantAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productVariantAttribute()
        {
            var pva = new ProductVariantAttribute
                      {
                          TextPrompt = "TextPrompt 1",
                          IsRequired = true,
                          AttributeControlType = AttributeControlType.DropdownList,
                          DisplayOrder = 1,
                          Product = GetTestProduct(),
                          ProductAttribute = new ProductAttribute()
                          {
                              Name = "Name 1",
                              Description = "Description 1",
                          }
                      };

            var fromDb = SaveAndLoadEntity(pva);
            fromDb.ShouldNotBeNull();
            fromDb.TextPrompt.ShouldEqual("TextPrompt 1");
            fromDb.IsRequired.ShouldEqual(true);
            fromDb.AttributeControlType.ShouldEqual(AttributeControlType.DropdownList);
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.Product.ShouldNotBeNull();

            fromDb.ProductAttribute.ShouldNotBeNull();
            fromDb.ProductAttribute.Name.ShouldEqual("Name 1");
        }

        protected Product GetTestProduct()
        {
            return new Product
            {
                Name = "Product name 1",
                CreatedOnUtc = new DateTime(2010, 01, 03),
                UpdatedOnUtc = new DateTime(2010, 01, 04),
            };
        }
    }
}
