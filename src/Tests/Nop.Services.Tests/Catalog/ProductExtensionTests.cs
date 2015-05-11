using System;
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

            var result = product.ParseAllowedQuantities();
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

        [Test]
        public void Can_calculate_rental_periods_for_days()
        {
            var product = new Product
            {
                IsRental = true,
                RentalPricePeriod = RentalPricePeriod.Days
            };

            //rental period length = 1 day
            product.RentalPriceLength = 1;
            //the same date
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //1 day
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 6)).ShouldEqual(1);
            //2 days
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 7)).ShouldEqual(2);
            //3 days
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 8)).ShouldEqual(3);

            //rental period length = 2 days
            product.RentalPriceLength = 2;
            //the same date
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //1 day
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 6)).ShouldEqual(1);
            //2 days
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 7)).ShouldEqual(1);
            //3 days
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 8)).ShouldEqual(2);

        }
        [Test]
        public void Can_calculate_rental_periods_for_weeks()
        {
            var product = new Product
            {
                IsRental = true,
                RentalPricePeriod = RentalPricePeriod.Weeks
            };

            //rental period length = 1 week
            product.RentalPriceLength = 1;
            //the same date
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //several days but less than a week
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 3)).ShouldEqual(1);
            //1 week
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 12)).ShouldEqual(1);
            //several days but less than two weeks
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 13)).ShouldEqual(2);
            //2 weeks
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 19)).ShouldEqual(2);
            //3 weeks
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 26)).ShouldEqual(3);

            //rental period length = 2 weeks
            product.RentalPriceLength = 2;
            //the same date
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //several days but less than a week
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 3)).ShouldEqual(1);
            //1 week
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 12)).ShouldEqual(1);
            //several days but less than two weeks
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 13)).ShouldEqual(1);
            //2 weeks
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 19)).ShouldEqual(1);
            //3 weeks
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 26)).ShouldEqual(2);

        }
        [Test]
        public void Can_calculate_rental_periods_for_months()
        {
            var product = new Product
            {
                IsRental = true,
                RentalPricePeriod = RentalPricePeriod.Months
            };

            //rental period length = 1 month
            product.RentalPriceLength = 1;
            //the same date
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //several days but less than a month
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 4)).ShouldEqual(1);
            //1 month
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 4, 5)).ShouldEqual(1);
            //1 month and 1 day
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 4, 6)).ShouldEqual(2);
            //several days but less than two months
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 4, 13)).ShouldEqual(2);
            //2 months
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 5, 5)).ShouldEqual(2);
            //3 months
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 5, 8)).ShouldEqual(3);
            //several more unit tests
            product.GetRentalPeriods(new DateTime(1900, 1, 1), new DateTime(1900, 1, 1)).ShouldEqual(1);
            product.GetRentalPeriods(new DateTime(1900, 1, 1), new DateTime(1900, 1, 2)).ShouldEqual(1);
            product.GetRentalPeriods(new DateTime(1900, 1, 2), new DateTime(1900, 1, 1)).ShouldEqual(1);
            product.GetRentalPeriods(new DateTime(1900, 1, 1), new DateTime(1900, 2, 1)).ShouldEqual(1);
            product.GetRentalPeriods(new DateTime(1900, 2, 1), new DateTime(1900, 1, 1)).ShouldEqual(1);
            product.GetRentalPeriods(new DateTime(1900, 1, 31), new DateTime(1900, 2, 1)).ShouldEqual(1);
            product.GetRentalPeriods(new DateTime(1900, 8, 31), new DateTime(1900, 9, 30)).ShouldEqual(1);
            product.GetRentalPeriods(new DateTime(1900, 8, 31), new DateTime(1900, 10, 1)).ShouldEqual(2);
            product.GetRentalPeriods(new DateTime(1900, 1, 1), new DateTime(1901, 1, 1)).ShouldEqual(12);
            product.GetRentalPeriods(new DateTime(1900, 1, 1), new DateTime(1911, 1, 1)).ShouldEqual(132);
            product.GetRentalPeriods(new DateTime(1900, 8, 31), new DateTime(1901, 8, 30)).ShouldEqual(12);


            //rental period length = 2 months
            product.RentalPriceLength = 2;
            //the same date
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //several days but less than a month
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 4)).ShouldEqual(1);
            //1 month
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 4, 5)).ShouldEqual(1);
            //several days but less than two months
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 4, 13)).ShouldEqual(1);
            //2 months
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 5, 5)).ShouldEqual(1);
            //3 months
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 5, 8)).ShouldEqual(2);
        }
        [Test]
        public void Can_calculate_rental_periods_for_years()
        {
            var product = new Product
            {
                IsRental = true,
                RentalPricePeriod = RentalPricePeriod.Years
            };

            //rental period length = 1 years
            product.RentalPriceLength = 1;
            //the same date
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //several days but less than a year
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2015, 1, 1)).ShouldEqual(1);
            //more than one year
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2015, 3, 7)).ShouldEqual(2);

            //rental period length = 2 years
            product.RentalPriceLength = 2;
            //the same date
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //several days but less than a year
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2015, 1, 1)).ShouldEqual(1);
            //more than one year
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2015, 3, 7)).ShouldEqual(1);
            //more than two year
            product.GetRentalPeriods(new DateTime(2014, 3, 5), new DateTime(2016, 3, 7)).ShouldEqual(2);

        }
    }
}
