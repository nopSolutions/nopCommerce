using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class ProductServiceTests : ServiceTest
    {
        #region Fields

        private IProductService _productService;
        private Mock<IRepository<Product>> _productRepository;
        private Mock<IRepository<ProductWarehouseInventory>> _productWarehouseInventoryRepository;

        private Product _productNotUseMultipleWarehouses = new Product
        {
            Id = 1,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            UseMultipleWarehouses = false,
            StockQuantity = 6,
        };

        private Product _productUseMultipleWarehousesWithReserved = new Product
        {
            Id = 2,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            UseMultipleWarehouses = true,
            StockQuantity = 6,
        };

        private Product _productUseMultipleWarehousesWithoutReserved = new Product
        {
            Id = 3,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            UseMultipleWarehouses = true,
            StockQuantity = 6,
        };

        private Product _productUseMultipleWarehousesWithWarehouseSpecified = new Product
        {
            Id = 4,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            UseMultipleWarehouses = true,
            StockQuantity = 6,
        };

        #endregion

        #region SetUp

        [SetUp]
        public new void SetUp()
        {
            _productRepository = new Mock<IRepository<Product>>();
            _productRepository.Setup(p => p.Table).Returns(GetMockProducts);

            _productWarehouseInventoryRepository = new Mock<IRepository<ProductWarehouseInventory>>();
            _productWarehouseInventoryRepository.Setup(x => x.Table).Returns(GetMockProductWarehouseInventoryRecords);

            _productService = new ProductService(new CatalogSettings(), new CommonSettings(), null, new FakeCacheKeyService(),  null,
                null, null, null, null, null, null, null, null, null, null, _productRepository.Object, null, null, null, null, null, null, _productWarehouseInventoryRepository.Object, null, null, null, null, null, null,
                null, null, null, null, new LocalizationSettings());
        }

        #endregion

        #region Utilities

        private IQueryable<Product> GetMockProducts()
        {
            return new List<Product>
            {
                _productNotUseMultipleWarehouses,
                _productUseMultipleWarehousesWithReserved,
                _productUseMultipleWarehousesWithoutReserved,
                _productUseMultipleWarehousesWithWarehouseSpecified
            }.AsQueryable();
        }

        private IQueryable<ProductWarehouseInventory> GetMockProductWarehouseInventoryRecords()
        {
            return new List<ProductWarehouseInventory>
            {
                new ProductWarehouseInventory
                {
                    ProductId = 1,
                    WarehouseId = 1,
                    StockQuantity = 7,
                },
                new ProductWarehouseInventory
                {
                    ProductId = 1,
                    WarehouseId = 2,
                    StockQuantity = 8,
                },
                new ProductWarehouseInventory
                {
                    ProductId = 1,
                    WarehouseId = 3,
                    StockQuantity = -2,
                },
                new ProductWarehouseInventory
                {
                    ProductId = 2,
                    WarehouseId = 1,
                    StockQuantity = 7,
                    ReservedQuantity = 4,
                },
                new ProductWarehouseInventory
                {
                    ProductId = 2,
                    WarehouseId = 2,
                    StockQuantity = 8,
                    ReservedQuantity = 1,
                },
                new ProductWarehouseInventory
                {
                    ProductId = 2,
                    WarehouseId = 3,
                    StockQuantity = -2,
                },
                new ProductWarehouseInventory
                {
                    ProductId = 3,
                    WarehouseId = 1,
                    StockQuantity = 7,
                    ReservedQuantity = 4,
                },
                new ProductWarehouseInventory
                {
                    ProductId = 3,
                    WarehouseId = 2,
                    StockQuantity = 8,
                    ReservedQuantity = 1,
                },
                new ProductWarehouseInventory
                {
                    ProductId = 3,
                    WarehouseId = 3,
                    StockQuantity = -2,
                },
                new ProductWarehouseInventory
                {
                    ProductId = 4,
                    WarehouseId = 1,
                    StockQuantity = 7,
                    ReservedQuantity = 4,
                },
                new ProductWarehouseInventory
                {
                    WarehouseId = 2,
                    StockQuantity = 8,
                    ReservedQuantity = 1,
                },
                new ProductWarehouseInventory
                {
                    WarehouseId = 3,
                    StockQuantity = -2,
                }
            }.AsQueryable();
        }

        #endregion

        #region Tests

        [Test]
        public void Can_parse_required_product_ids()
        {
            var product = new Product
            {
                RequiredProductIds = "1, 4,7 ,a,"
            };

            var ids = _productService.ParseRequiredProductIds(product);
            ids.Length.Should().Be(3);
            ids[0].Should().Be(1);
            ids[1].Should().Be(4);
            ids[2].Should().Be(7);
        }

        [Test]
        public void Should_be_available_when_startdate_is_not_set()
        {
            var product = new Product
            {
                AvailableStartDateTimeUtc = null
            };

            _productService.ProductIsAvailable(product, new DateTime(2010, 01, 03)).Should().BeTrue();
        }

        [Test]
        public void Should_be_available_when_startdate_is_less_than_somedate()
        {
            var product = new Product
            {
                AvailableStartDateTimeUtc = new DateTime(2010, 01, 02)
            };

            _productService.ProductIsAvailable(product, new DateTime(2010, 01, 03)).Should().BeTrue();
        }

        [Test]
        public void Should_not_be_available_when_startdate_is_greater_than_somedate()
        {
            var product = new Product
            {
                AvailableStartDateTimeUtc = new DateTime(2010, 01, 02)
            };

            _productService.ProductIsAvailable(product, new DateTime(2010, 01, 01)).Should().BeFalse();
        }

        [Test]
        public void Should_be_available_when_enddate_is_not_set()
        {
            var product = new Product
            {
                AvailableEndDateTimeUtc = null
            };

            _productService.ProductIsAvailable(product, new DateTime(2010, 01, 03)).Should().BeTrue();
        }

        [Test]
        public void Should_be_available_when_enddate_is_greater_than_somedate()
        {
            var product = new Product
            {
                AvailableEndDateTimeUtc = new DateTime(2010, 01, 02)
            };

            _productService.ProductIsAvailable(product, new DateTime(2010, 01, 01)).Should().BeTrue();
        }

        [Test]
        public void Should_not_be_available_when_enddate_is_less_than_somedate()
        {
            var product = new Product
            {
                AvailableEndDateTimeUtc = new DateTime(2010, 01, 02)
            };

            _productService.ProductIsAvailable(product, new DateTime(2010, 01, 03)).Should().BeFalse();
        }

        [Test]
        public void Can_parse_allowed_quantities()
        {
            var product = new Product
            {
                AllowedQuantities = "1, 5,4,10,sdf"
            };

            var result = _productService.ParseAllowedQuantities(product);
            result.Length.Should().Be(4);
            result[0].Should().Be(1);
            result[1].Should().Be(5);
            result[2].Should().Be(4);
            result[3].Should().Be(10);
        }

        [Test]
        public void Can_calculate_total_quantity_when_we_do_not_use_multiple_warehouses()
        {
            var result = _productService.GetTotalStockQuantity(_productNotUseMultipleWarehouses, true);
            result.Should().Be(6);
        }

        [Test]
        public void Can_calculate_total_quantity_when_we_do_use_multiple_warehouses_with_reserved()
        {
            var result = _productService.GetTotalStockQuantity(_productUseMultipleWarehousesWithReserved, true);
            result.Should().Be(8);
        }

        [Test]
        public void Can_calculate_total_quantity_when_we_do_use_multiple_warehouses_without_reserved()
        {
            var result = _productService.GetTotalStockQuantity(_productUseMultipleWarehousesWithoutReserved, false);
            result.Should().Be(13);
        }

        [Test]
        public void Can_calculate_total_quantity_when_we_do_use_multiple_warehouses_with_warehouse_specified()
        {
            var result = _productService.GetTotalStockQuantity(_productUseMultipleWarehousesWithWarehouseSpecified, true, 1);
            result.Should().Be(3);
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
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //1 day
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 6)).Should().Be(1);
            //2 days
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 7)).Should().Be(2);
            //3 days
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 8)).Should().Be(3);

            //rental period length = 2 days
            product.RentalPriceLength = 2;
            //the same date
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //1 day
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 6)).Should().Be(1);
            //2 days
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 7)).Should().Be(1);
            //3 days
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 8)).Should().Be(2);
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
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //several days but less than a week
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 3)).Should().Be(1);
            //1 week
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 12)).Should().Be(1);
            //several days but less than two weeks
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 13)).Should().Be(2);
            //2 weeks
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 19)).Should().Be(2);
            //3 weeks
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 26)).Should().Be(3);

            //rental period length = 2 weeks
            product.RentalPriceLength = 2;
            //the same date
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //several days but less than a week
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 3)).Should().Be(1);
            //1 week
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 12)).Should().Be(1);
            //several days but less than two weeks
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 13)).Should().Be(1);
            //2 weeks
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 19)).Should().Be(1);
            //3 weeks
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 26)).Should().Be(2);
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
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //several days but less than a month
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 4)).Should().Be(1);
            //1 month
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 4, 5)).Should().Be(1);
            //1 month and 1 day
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 4, 6)).Should().Be(2);
            //several days but less than two months
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 4, 13)).Should().Be(2);
            //2 months
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 5, 5)).Should().Be(2);
            //3 months
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 5, 8)).Should().Be(3);
            //several more unit tests
            _productService.GetRentalPeriods(product, new DateTime(1900, 1, 1), new DateTime(1900, 1, 1)).Should().Be(1);
            _productService.GetRentalPeriods(product, new DateTime(1900, 1, 1), new DateTime(1900, 1, 2)).Should().Be(1);
            _productService.GetRentalPeriods(product, new DateTime(1900, 1, 2), new DateTime(1900, 1, 1)).Should().Be(1);
            _productService.GetRentalPeriods(product, new DateTime(1900, 1, 1), new DateTime(1900, 2, 1)).Should().Be(1);
            _productService.GetRentalPeriods(product, new DateTime(1900, 2, 1), new DateTime(1900, 1, 1)).Should().Be(1);
            _productService.GetRentalPeriods(product, new DateTime(1900, 1, 31), new DateTime(1900, 2, 1)).Should().Be(1);
            _productService.GetRentalPeriods(product, new DateTime(1900, 8, 31), new DateTime(1900, 9, 30)).Should().Be(1);
            _productService.GetRentalPeriods(product, new DateTime(1900, 8, 31), new DateTime(1900, 10, 1)).Should().Be(2);
            _productService.GetRentalPeriods(product, new DateTime(1900, 1, 1), new DateTime(1901, 1, 1)).Should().Be(12);
            _productService.GetRentalPeriods(product, new DateTime(1900, 1, 1), new DateTime(1911, 1, 1)).Should().Be(132);
            _productService.GetRentalPeriods(product, new DateTime(1900, 8, 31), new DateTime(1901, 8, 30)).Should().Be(12);
            
            //rental period length = 2 months
            product.RentalPriceLength = 2;
            //the same date
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //several days but less than a month
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 4)).Should().Be(1);
            //1 month
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 4, 5)).Should().Be(1);
            //several days but less than two months
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 4, 13)).Should().Be(1);
            //2 months
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 5, 5)).Should().Be(1);
            //3 months
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 5, 8)).Should().Be(2);
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
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //several days but less than a year
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2015, 1, 1)).Should().Be(1);
            //more than one year
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2015, 3, 7)).Should().Be(2);

            //rental period length = 2 years
            product.RentalPriceLength = 2;
            //the same date
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //several days but less than a year
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2015, 1, 1)).Should().Be(1);
            //more than one year
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2015, 3, 7)).Should().Be(1);
            //more than two year
            _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2016, 3, 7)).Should().Be(2);
        } 

        #endregion
    }
}
