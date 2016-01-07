using System;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductAttributeValuePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productAttributeValue()
        {
            var pav = new ProductAttributeValue
            {
                AttributeValueType = AttributeValueType.AssociatedToProduct,
                AssociatedProductId = 10,
                Name = "Name 1",
                ColorSquaresRgb = "12FF33",
                ImageSquaresPictureId = 1,
                PriceAdjustment = 1.1M,
                WeightAdjustment = 2.1M,
                Cost = 3.1M,
                Quantity = 2,
                IsPreSelected = true,
                DisplayOrder = 3,
                ProductAttributeMapping = new ProductAttributeMapping
                {
                    TextPrompt = "TextPrompt 1",
                    IsRequired = true,
                    AttributeControlType = AttributeControlType.DropdownList,
                    DisplayOrder = 1,
                    Product = GetTestProduct(),
                    ProductAttribute = new ProductAttribute
                    {
                        Name = "Name 1",
                        Description = "Description 1",
                    }
                }
            };

            var fromDb = SaveAndLoadEntity(pav);
            fromDb.ShouldNotBeNull();
            fromDb.AttributeValueType.ShouldEqual(AttributeValueType.AssociatedToProduct);
            fromDb.AssociatedProductId.ShouldEqual(10);
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.ColorSquaresRgb.ShouldEqual("12FF33");
            fromDb.ImageSquaresPictureId.ShouldEqual(1);
            fromDb.PriceAdjustment.ShouldEqual(1.1M);
            fromDb.WeightAdjustment.ShouldEqual(2.1M);
            fromDb.Cost.ShouldEqual(3.1M);
            fromDb.Quantity.ShouldEqual(2);
            fromDb.IsPreSelected.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(3);

            fromDb.ProductAttributeMapping.ShouldNotBeNull();
            fromDb.ProductAttributeMapping.TextPrompt.ShouldEqual("TextPrompt 1");
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
