using System;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductVariantAttributeCombinationPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productVariantAttributeCombination()
        {
            var pvac = new ProductVariantAttributeCombination
                       {
                           AttributesXml = "Some XML",
                           StockQuantity = 2,
                           AllowOutOfStockOrders = true,
                           Sku = "Sku1",
                           ManufacturerPartNumber = "ManufacturerPartNumber1",
                           Gtin = "Gtin1",
                           OverriddenPrice = 0.01M,
                           Product = GetTestProduct()
                       };

            var fromDb = SaveAndLoadEntity(pvac);
            fromDb.ShouldNotBeNull();
            fromDb.AttributesXml.ShouldEqual("Some XML");
            fromDb.StockQuantity.ShouldEqual(2);
            fromDb.AllowOutOfStockOrders.ShouldEqual(true);
            fromDb.Sku.ShouldEqual("Sku1");
            fromDb.ManufacturerPartNumber.ShouldEqual("ManufacturerPartNumber1");
            fromDb.Gtin.ShouldEqual("Gtin1");
            fromDb.OverriddenPrice.ShouldEqual(0.01M);
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