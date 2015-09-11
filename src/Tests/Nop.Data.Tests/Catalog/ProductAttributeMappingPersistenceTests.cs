using System;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductAttributeMappingPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productAttributeMapping()
        {
            var productAttributeMapping = new ProductAttributeMapping
                      {
                          TextPrompt = "TextPrompt 1",
                          IsRequired = true,
                          AttributeControlType = AttributeControlType.DropdownList,
                          DisplayOrder = 1,
                          ValidationMinLength = 2,
                          ValidationMaxLength = 3,
                          ValidationFileAllowedExtensions = "ValidationFileAllowedExtensions 1",
                          ValidationFileMaximumSize = 4,
                          DefaultValue = "DefaultValue 1",
                          ConditionAttributeXml = "ConditionAttributeXml 1",
                          Product = GetTestProduct(),
                          ProductAttribute = new ProductAttribute
                          {
                              Name = "Name 1",
                              Description = "Description 1",
                          }
                      };

            var fromDb = SaveAndLoadEntity(productAttributeMapping);
            fromDb.ShouldNotBeNull();
            fromDb.TextPrompt.ShouldEqual("TextPrompt 1");
            fromDb.IsRequired.ShouldEqual(true);
            fromDb.AttributeControlType.ShouldEqual(AttributeControlType.DropdownList);
            fromDb.DisplayOrder.ShouldEqual(1);
            fromDb.ValidationMinLength.ShouldEqual(2);
            fromDb.ValidationMaxLength.ShouldEqual(3);
            fromDb.ValidationFileAllowedExtensions.ShouldEqual("ValidationFileAllowedExtensions 1");
            fromDb.ValidationFileMaximumSize.ShouldEqual(4);
            fromDb.DefaultValue.ShouldEqual("DefaultValue 1");
            fromDb.ConditionAttributeXml.ShouldEqual("ConditionAttributeXml 1");

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
