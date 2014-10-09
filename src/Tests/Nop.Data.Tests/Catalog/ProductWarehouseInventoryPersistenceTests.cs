using System;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductWarehouseInventoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productWarehouseInventory()
        {
            var pwi = new ProductWarehouseInventory
            {
                Product = new Product
                {
                    Name = "Name 1",
                    Published = true,
                    Deleted = false,
                    CreatedOnUtc = new DateTime(2010, 01, 01),
                    UpdatedOnUtc = new DateTime(2010, 01, 02)
                },
                Warehouse = new Warehouse
                {
                    Name = "Name 2",
                    AddressId = 1,
                },
                StockQuantity = 3,
                ReservedQuantity = 4,
            };

            var fromDb = SaveAndLoadEntity(pwi);
            fromDb.ShouldNotBeNull();
            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.Name.ShouldEqual("Name 1");
            fromDb.Warehouse.ShouldNotBeNull();
            fromDb.Warehouse.Name.ShouldEqual("Name 2");
            fromDb.StockQuantity.ShouldEqual(3);
            fromDb.ReservedQuantity.ShouldEqual(4);
        }
    }
}
