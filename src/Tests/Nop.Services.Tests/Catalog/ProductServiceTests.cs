using System;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Services.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class ProductServiceTests : ServiceTest
    {
        private IProductService _productService;

        [SetUp]
        public new void SetUp()
        {
            _productService = new ProductService(new CatalogSettings(), new CommonSettings(), null, new NopNullCache(),
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, new LocalizationSettings());
        }

        [Test]
        public void Can_parse_required_product_ids()
        {
            var product = new Product
            {
                RequiredProductIds = "1, 4,7 ,a,"
            };

            var ids = _productService.ParseRequiredProductIds(product);
            ids.Length.ShouldEqual(3);
            ids[0].ShouldEqual(1);
            ids[1].ShouldEqual(4);
            ids[2].ShouldEqual(7);
        }

        [Test]
        public void Should_be_available_when_startdate_is_not_set()
        {
            var product = new Product
            {
                AvailableStartDateTimeUtc = null
            };

            _productService.ProductIsAvailable(product, new DateTime(2010, 01, 03)).ShouldEqual(true);
        }

        [Test]
        public void Should_be_available_when_startdate_is_less_than_somedate()
        {
            var product = new Product
            {
                AvailableStartDateTimeUtc = new DateTime(2010, 01, 02)
            };

            _productService.ProductIsAvailable(product, new DateTime(2010, 01, 03)).ShouldEqual(true);
        }

        [Test]
        public void Should_not_be_available_when_startdate_is_greater_than_somedate()
        {
            var product = new Product
            {
                AvailableStartDateTimeUtc = new DateTime(2010, 01, 02)
            };

            _productService.ProductIsAvailable(product, new DateTime(2010, 01, 01)).ShouldEqual(false);
        }

        [Test]
        public void Should_be_available_when_enddate_is_not_set()
        {
            var product = new Product
            {
                AvailableEndDateTimeUtc = null
            };

            _productService.ProductIsAvailable(product, new DateTime(2010, 01, 03)).ShouldEqual(true);
        }

        [Test]
        public void Should_be_available_when_enddate_is_greater_than_somedate()
        {
            var product = new Product
            {
                AvailableEndDateTimeUtc = new DateTime(2010, 01, 02)
            };

            _productService.ProductIsAvailable(product, new DateTime(2010, 01, 01)).ShouldEqual(true);
        }

        [Test]
        public void Should_not_be_available_when_enddate_is_less_than_somedate()
        {
            var product = new Product
            {
                AvailableEndDateTimeUtc = new DateTime(2010, 01, 02)
            };

            _productService.ProductIsAvailable(product, new DateTime(2010, 01, 03)).ShouldEqual(false);
        }

        [Test]
        public void Can_parse_allowed_quantities()
        {
            var product = new Product
            {
                AllowedQuantities = "1, 5,4,10,sdf"
            };

            var result = _productService.ParseAllowedQuantities(product);
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


            var result = _productService.GetTotalStockQuantity(product, true);
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

            var result = _productService.GetTotalStockQuantity(product, true);
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

            var result = _productService.GetTotalStockQuantity(product, false);
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

            var result = _productService.GetTotalStockQuantity(product, true, 1);
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
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //1 day
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 6)).ShouldEqual(1);
            //2 days
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 7)).ShouldEqual(2);
            //3 days
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 8)).ShouldEqual(3);

            //rental period length = 2 days
            product.RentalPriceLength = 2;
            //the same date
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //1 day
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 6)).ShouldEqual(1);
            //2 days
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 7)).ShouldEqual(1);
            //3 days
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 8)).ShouldEqual(2);

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
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //several days but less than a week
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 3)).ShouldEqual(1);
            //1 week
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 12)).ShouldEqual(1);
            //several days but less than two weeks
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 13)).ShouldEqual(2);
            //2 weeks
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 19)).ShouldEqual(2);
            //3 weeks
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 26)).ShouldEqual(3);

            //rental period length = 2 weeks
            product.RentalPriceLength = 2;
            //the same date
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //several days but less than a week
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 3)).ShouldEqual(1);
            //1 week
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 12)).ShouldEqual(1);
            //several days but less than two weeks
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 13)).ShouldEqual(1);
            //2 weeks
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 19)).ShouldEqual(1);
            //3 weeks
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 26)).ShouldEqual(2);

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
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //several days but less than a month
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 4)).ShouldEqual(1);
            //1 month
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 4, 5)).ShouldEqual(1);
            //1 month and 1 day
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 4, 6)).ShouldEqual(2);
            //several days but less than two months
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 4, 13)).ShouldEqual(2);
            //2 months
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 5, 5)).ShouldEqual(2);
            //3 months
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 5, 8)).ShouldEqual(3);
            //several more unit tests
            _productService.GetRentalPeriods(product, new DateTime(1900, 1, 1), new DateTime(1900, 1, 1)).ShouldEqual(1);
            _productService.GetRentalPeriods(product, new DateTime(1900, 1, 1), new DateTime(1900, 1, 2)).ShouldEqual(1);
            _productService.GetRentalPeriods(product, new DateTime(1900, 1, 2), new DateTime(1900, 1, 1)).ShouldEqual(1);
            _productService.GetRentalPeriods(product, new DateTime(1900, 1, 1), new DateTime(1900, 2, 1)).ShouldEqual(1);
            _productService.GetRentalPeriods(product, new DateTime(1900, 2, 1), new DateTime(1900, 1, 1)).ShouldEqual(1);
            _productService.GetRentalPeriods(product, new DateTime(1900, 1, 31), new DateTime(1900, 2, 1)).ShouldEqual(1);
            _productService.GetRentalPeriods(product, new DateTime(1900, 8, 31), new DateTime(1900, 9, 30)).ShouldEqual(1);
            _productService.GetRentalPeriods(product, new DateTime(1900, 8, 31), new DateTime(1900, 10, 1)).ShouldEqual(2);
            _productService.GetRentalPeriods(product, new DateTime(1900, 1, 1), new DateTime(1901, 1, 1)).ShouldEqual(12);
            _productService.GetRentalPeriods(product, new DateTime(1900, 1, 1), new DateTime(1911, 1, 1)).ShouldEqual(132);
            _productService.GetRentalPeriods(product, new DateTime(1900, 8, 31), new DateTime(1901, 8, 30)).ShouldEqual(12);


            //rental period length = 2 months
            product.RentalPriceLength = 2;
            //the same date
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //several days but less than a month
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 4)).ShouldEqual(1);
            //1 month
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 4, 5)).ShouldEqual(1);
            //several days but less than two months
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 4, 13)).ShouldEqual(1);
            //2 months
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 5, 5)).ShouldEqual(1);
            //3 months
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 5, 8)).ShouldEqual(2);
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
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //several days but less than a year
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2015, 1, 1)).ShouldEqual(1);
            //more than one year
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2015, 3, 7)).ShouldEqual(2);

            //rental period length = 2 years
            product.RentalPriceLength = 2;
            //the same date
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).ShouldEqual(1);
            //several days but less than a year
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2015, 1, 1)).ShouldEqual(1);
            //more than one year
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2015, 3, 7)).ShouldEqual(1);
            //more than two year
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2016, 3, 7)).ShouldEqual(2);

        }
    }
}
