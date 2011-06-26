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
                           ProductVariant = new ProductVariant
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
                                                              UpdatedOnUtc = new DateTime(2010, 01, 02)
                                                          }
                                            }
                       };

            var fromDb = SaveAndLoadEntity(pvac);
            fromDb.ShouldNotBeNull();
            fromDb.AttributesXml.ShouldEqual("Some XML");
            fromDb.StockQuantity.ShouldEqual(2);
            fromDb.AllowOutOfStockOrders.ShouldEqual(true);
        }
    }
}