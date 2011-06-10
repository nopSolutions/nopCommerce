using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class ProductAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productAttribute()
        {
            var pa = new ProductAttribute
            {
                Name = "Name 1",
                Description = "Description 1",
            };

            var fromDb = SaveAndLoadEntity(pa);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.Description.ShouldEqual("Description 1");
        }
        
        [Test]
        public void Can_save_and_load_productAttribute_with_productVariantAttributes()
        {
            var pa = new ProductAttribute
                     {
                         Name = "Name 1",
                         Description = "Description 1"
                     };
            pa.ProductVariantAttributes.Add
                (
                    new ProductVariantAttribute
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
                                CreatedOnUtc = new DateTime(2010, 01,01),
                                UpdatedOnUtc = new DateTime(2010, 01,02),
                            }
                        }

                    }
                );
            var fromDb = SaveAndLoadEntity(pa);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");


            fromDb.ProductVariantAttributes.ShouldNotBeNull();
            (fromDb.ProductVariantAttributes.Count == 1).ShouldBeTrue();
            fromDb.ProductVariantAttributes.First().TextPrompt.ShouldEqual("TextPrompt 1");
        }
    }
}