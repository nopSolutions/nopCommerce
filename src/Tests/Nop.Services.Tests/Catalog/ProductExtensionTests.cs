using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class ProductExtensionTests : ServiceTest
    {
        [SetUp]
        public new void SetUp()
        {

        }

        [Test]
        public void Can_parse_allowed_quantities()
        {
            var product = new Product
            {
                AllowedQuantities = "1, 5,4,10,sdf"
            };

            var result = product.ParseAllowedQuatities();
            result.Length.ShouldEqual(4);
            result[0].ShouldEqual(1);
            result[1].ShouldEqual(5);
            result[2].ShouldEqual(4);
            result[3].ShouldEqual(10);
        }

        [Test]
        public void Can_calculate_total_quantity_when_we_do_not_use_multiple_warehouses()
        {
            var product = new Product
            {
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                UseMultipleWarehouses = false,
                StockQuantity = 6,
            };
            product.ProductWarehouseInventory.Add(new ProductWarehouseInventory
            {
                WarehouseId = 1,
                StockQuantity = 7,
            });
            product.ProductWarehouseInventory.Add(new ProductWarehouseInventory
            {
                WarehouseId = 2,
                StockQuantity = 8,
            });
            product.ProductWarehouseInventory.Add(new ProductWarehouseInventory
            {
                WarehouseId = 3,
                StockQuantity = -2,
            });


            var result = product.GetTotalStockQuantity(true);
            result.ShouldEqual(6);
        }

        [Test]
        public void Can_calculate_total_quantity_when_we_do_use_multiple_warehouses_with_reserved()
        {
            var product = new Product
            {
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                UseMultipleWarehouses = true,
                StockQuantity = 6,
            };
            product.ProductWarehouseInventory.Add(new ProductWarehouseInventory
            {
                WarehouseId = 1,
                StockQuantity = 7,
                ReservedQuantity = 4,
            });
            product.ProductWarehouseInventory.Add(new ProductWarehouseInventory
            {
                WarehouseId = 2,
                StockQuantity = 8,
                ReservedQuantity = 1,
            });
            product.ProductWarehouseInventory.Add(new ProductWarehouseInventory
            {
                WarehouseId = 3,
                StockQuantity = -2,
            });

            var result = product.GetTotalStockQuantity(true);
            result.ShouldEqual(8);
        }

        [Test]
        public void Can_calculate_total_quantity_when_we_do_use_multiple_warehouses_without_reserved()
        {
            var product = new Product
            {
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                UseMultipleWarehouses = true,
                StockQuantity = 6,
            };
            product.ProductWarehouseInventory.Add(new ProductWarehouseInventory
            {
                WarehouseId = 1,
                StockQuantity = 7,
                ReservedQuantity = 4,
            });
            product.ProductWarehouseInventory.Add(new ProductWarehouseInventory
            {
                WarehouseId = 2,
                StockQuantity = 8,
                ReservedQuantity = 1,
            });
            product.ProductWarehouseInventory.Add(new ProductWarehouseInventory
            {
                WarehouseId = 3,
                StockQuantity = -2,
            });

            var result = product.GetTotalStockQuantity(false);
            result.ShouldEqual(13);
        }

        [Test]
        public void Can_calculate_total_quantity_when_we_do_use_multiple_warehouses_with_warehouse_specified()
        {
            var product = new Product
            {
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                UseMultipleWarehouses = true,
                StockQuantity = 6,
            };
            product.ProductWarehouseInventory.Add(new ProductWarehouseInventory
            {
                WarehouseId = 1,
                StockQuantity = 7,
                ReservedQuantity = 4,
            });
            product.ProductWarehouseInventory.Add(new ProductWarehouseInventory
            {
                WarehouseId = 2,
                StockQuantity = 8,
                ReservedQuantity = 1,
            });
            product.ProductWarehouseInventory.Add(new ProductWarehouseInventory
            {
                WarehouseId = 3,
                StockQuantity = -2,
            });

            var result = product.GetTotalStockQuantity(true, 1);
            result.ShouldEqual(3);
        }
    }
}
