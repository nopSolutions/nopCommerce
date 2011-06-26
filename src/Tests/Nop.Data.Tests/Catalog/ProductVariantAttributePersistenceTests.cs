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
                          ProductVariant = new ProductVariant()
                                           {
                                               Name = "Product variant name 1",
                                               CreatedOnUtc = new DateTime(2010, 01, 03),
                                               UpdatedOnUtc = new DateTime(2010, 01, 04),
                                               Product = new Product()
                                                         {
                                                             Name = "Name 1",
                                                             Published = true,
                                                             Deleted = false,
                                                             CreatedOnUtc = new DateTime(2010, 01, 01),
                                                             UpdatedOnUtc = new DateTime(2010, 01, 02),
                                                         }
                                           },
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

            fromDb.ProductVariant.ShouldNotBeNull();
            fromDb.ProductVariant.Name.ShouldEqual("Product variant name 1");

            fromDb.ProductAttribute.ShouldNotBeNull();
            fromDb.ProductAttribute.Name.ShouldEqual("Name 1");
        }
    }
}
