using System;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductVariantAttributeValuePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productVariantAttributeValue()
        {
            var pvav = new ProductVariantAttributeValue
            {
                Name = "Name 1",
                PriceAdjustment = 1.1M,
                WeightAdjustment = 2.1M,
                IsPreSelected = true,
                DisplayOrder = 3,
                ProductVariantAttribute = new ProductVariantAttribute
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
                }
            };

            var fromDb = SaveAndLoadEntity(pvav);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.PriceAdjustment.ShouldEqual(1.1M);
            fromDb.WeightAdjustment.ShouldEqual(2.1M);
            fromDb.IsPreSelected.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(3);

            fromDb.ProductVariantAttribute.ShouldNotBeNull();
            fromDb.ProductVariantAttribute.TextPrompt.ShouldEqual("TextPrompt 1");
        }
    }
}
