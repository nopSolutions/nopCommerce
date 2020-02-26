using System;
using System.Collections.Generic;
using FluentAssertions;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Services.Caching.CachingDefaults;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class PriceCalculationServiceTests : ServiceTest
    {
        #region Fields

        private Mock<IStoreContext> _storeContext;
        private IDiscountService _discountService;
        private Mock<ICategoryService> _categoryService;

        private CustomerService _customerService;
        private Mock<IRepository<CustomerCustomerRoleMapping>> _customerCustomerRoleMappingRepository;
        private Mock<IRepository<CustomerRole>> _customerRoleRepository;

        private Mock<IManufacturerService> _manufacturerService;
        private Mock<IProductAttributeParser> _productAttributeParser;
        private Mock<IRepository<DiscountProductMapping>> _discountProductMappingRepository;
        private Mock<IRepository<Product>> _productRepository;
        private Mock<IRepository<TierPrice>> _tierPriceRepository;
        private IProductService _productService;
        private IPriceCalculationService _priceCalcService;
        private CatalogSettings _catalogSettings;        
        private IStaticCacheManager _cacheManager;
        private Mock<IWorkContext> _workContext;
        private Store _store;
        private TestServiceProvider _serviceProvider;

        #endregion

        #region SetUp

        [SetUp]
        public new void SetUp()
        {
            _serviceProvider = new TestServiceProvider();
            _store = new Store { Id = 1 };
            _storeContext = new Mock<IStoreContext>();
            _storeContext.Setup(x => x.CurrentStore).Returns(_store);

            _categoryService = new Mock<ICategoryService>();
            
            _customerRoleRepository = new Mock<IRepository<CustomerRole>>();
            _customerRoleRepository.Setup(r => r.Table).Returns(GetMockCustomerRoles);

            _customerCustomerRoleMappingRepository = new Mock<IRepository<CustomerCustomerRoleMapping>>();

            var customerCustomerRoleMapping = new List<CustomerCustomerRoleMapping>();
            _customerCustomerRoleMappingRepository.Setup(r => r.Table).Returns(customerCustomerRoleMapping.AsQueryable());

            _customerCustomerRoleMappingRepository.Setup(r => r.Insert(It.IsAny<CustomerCustomerRoleMapping>()))
                .Callback(
                    (CustomerCustomerRoleMapping ccrm) => { customerCustomerRoleMapping.Add(ccrm); });

            _customerService = new CustomerService(null, new Mock<ICacheKeyFactory>().Object, new Mock<ICacheManager>().Object, null, null, null, null, null, null, _customerCustomerRoleMappingRepository.Object, null, _customerRoleRepository.Object, null, null, null, null);

            _manufacturerService = new Mock<IManufacturerService>();

            _productRepository = new Mock<IRepository<Product>>();
            _productRepository.Setup(p => p.Table).Returns(GetMockProducts);
            _productRepository.Setup(p => p.GetById(It.IsAny<int>())).Returns((int id) => GetMockProducts().FirstOrDefault(p => p.Id == id));

            _tierPriceRepository = new Mock<IRepository<TierPrice>>();
            _tierPriceRepository.Setup(t => t.Table).Returns(GetMockTierPrices);

            _discountProductMappingRepository = new Mock<IRepository<DiscountProductMapping>>();
            _discountProductMappingRepository.Setup(r => r.Table).Returns(GetMockDiscountProductMapping);
            _serviceProvider.DiscountProductMappingRepository.Setup(r => r.Table)
                .Returns(GetMockDiscountProductMapping);

            var shipmentRepository = new Mock<IRepository<Shipment>>();
            
            _productService = new ProductService(new CatalogSettings(), new CommonSettings(), null, new Mock<ICacheKeyFactory>().Object, _customerService,
                null, null, null, null, null, null, null, null, null, _discountProductMappingRepository.Object,
                _productRepository.Object, null, null, null, null, null, null, null, null, shipmentRepository.Object,
                null, null, _tierPriceRepository.Object, null,
                null, null, null, null, new LocalizationSettings());

            _productAttributeParser = new Mock<IProductAttributeParser>();

            _catalogSettings = new CatalogSettings();

            _cacheManager = new TestCacheManager();
            _workContext = new Mock<IWorkContext>();

            _discountService = TestDiscountService.Init(
                new List<Discount> { _mockDiscount }.AsQueryable(), 
                new List<DiscountProductMapping>
                {
                    new DiscountProductMapping { DiscountId = 1, EntityId = 1 }
                }.AsQueryable());

            _priceCalcService = new PriceCalculationService(_catalogSettings, new CurrencySettings { PrimaryStoreCurrencyId = 1 }, _categoryService.Object,
                _serviceProvider.CurrencyService.Object, _customerService, _discountService, _manufacturerService.Object, _productAttributeParser.Object,
                _productService, _cacheManager, _storeContext.Object, _workContext.Object);

            var nopEngine = new Mock<NopEngine>();

            nopEngine.Setup(x => x.ServiceProvider).Returns(_serviceProvider);
            EngineContext.Replace(nopEngine.Object);
        }

        #endregion

        #region Utilities

        private IQueryable<Product> GetMockProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Product name 1",
                    Price = 12.34M,
                    CustomerEntersPrice = false,
                    Published = true,
                    //set HasTierPrices property
                    HasTierPrices = true
                },
                new Product
                {
                    Id = 2,
                    Name = "Product TierPrices without CustomerRoles",
                    Price = 12.34M,
                    CustomerEntersPrice = false,
                    Published = true,
                    //set HasTierPrices property
                    HasTierPrices = true
                },
                new Product
                {
                    Id = 3,
                    Name = "Product name 1",
                    Price = 12.34M,
                    CustomerEntersPrice = false,
                    Published = true
                }
            }.AsQueryable();
        }

        private IQueryable<TierPrice> GetMockTierPrices()
        {
            return new List<TierPrice>
            {
                new TierPrice
                {
                    Price = 10,
                    Quantity = 2,
                    ProductId = 1,
                    CustomerRoleId = 1
                },
                    new TierPrice
                {
                    Price = 9,
                    Quantity = 2,
                    ProductId = 1,
                    CustomerRoleId = 2
                },
                    new TierPrice
                {
                    Price = 8,
                    Quantity = 5,
                    ProductId = 1,
                    CustomerRoleId = 1
                },
                    new TierPrice
                {
                    Price = 5,
                    Quantity = 10,
                    ProductId = 1,
                    CustomerRoleId = 2
                },

                //productId = 2
                new TierPrice
                {
                    Price = 10,
                    Quantity = 2,
                    ProductId = 2
                },
                new TierPrice
                {
                    Price = 9,
                    Quantity = 5,
                    ProductId = 2,
                    StartDateTimeUtc = new DateTime(2010, 01, 03)
                },
                 new TierPrice
                {
                    Price = 8,
                    Quantity = 5,
                    ProductId = 2,
                    StartDateTimeUtc = new DateTime(2027, 01, 03)
                },
                new TierPrice
                {
                    Price = 5,
                    Quantity = 10,
                    ProductId = 2,
                    StartDateTimeUtc = new DateTime(2010, 01, 03),
                    EndDateTimeUtc = new DateTime(2012, 01, 03)
                }
            }.AsQueryable();
        }

        private IQueryable<CustomerRole> GetMockCustomerRoles()
        {
            return new List<CustomerRole>
            {
                new CustomerRole
                {
                    Id = 1,
                    Name = "Some role 1",
                    Active = true
                },
                new CustomerRole
                {
                    Id = 2,
                    Name = "Some role 2",
                    Active = true
                }
            }.AsQueryable();
        }

        private readonly Discount _mockDiscount = new Discount
        {
            Id = 1,
            Name = "Discount 1",
            DiscountType = DiscountType.AssignedToSkus,
            DiscountAmount = 3,
            DiscountLimitation = DiscountLimitationType.Unlimited
        };

        private IQueryable<DiscountProductMapping> GetMockDiscountProductMapping()
        {
            return new List<DiscountProductMapping>
            {
                new DiscountProductMapping
                {
                    EntityId = 1,
                    DiscountId = _mockDiscount.Id
            }
            }.AsQueryable();
        }

        #endregion

        #region Tests

        [Test]
        public void Can_get_final_product_price()
        {
            var product = _productService.GetProductById(1);

            //customer
            var customer = new Customer();

            _priceCalcService.GetFinalPrice(product, customer, 0, false).Should().Be(12.34M);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 2).Should().Be(12.34M);
        }

        [Test]
        public void Can_get_final_product_price_with_tier_prices()
        {
            var product = _productService.GetProductById(2);

            //customer
            var customer = new Customer();

            _priceCalcService.GetFinalPrice(product, customer, 0, false).Should().Be(12.34M);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 2).Should().Be(10);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 3).Should().Be(10);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 5).Should().Be(9);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 10).Should().Be(9);
        }

        [Test]
        public void Can_get_final_product_price_with_tier_prices_by_customerRole()
        {
            var product = _productService.GetProductById(1);

            //customer
            var customer = new Customer { Id = 1 };

            var customerRole = GetMockCustomerRoles().FirstOrDefault(cr => cr.Id == 1);
            
            customerRole.Should().NotBeNull();

            _customerCustomerRoleMappingRepository.Object.Insert(new CustomerCustomerRoleMapping { CustomerRoleId = customerRole?.Id ?? 0, CustomerId = customer.Id });

            _priceCalcService.GetFinalPrice(product, customer, 0, false).Should().Be(12.34M);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 2).Should().Be(10);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 3).Should().Be(10);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 5).Should().Be(8);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 10).Should().Be(8);
        }

        [Test]
        public void Can_get_final_product_price_with_additionalFee()
        {
            var product = _productService.GetProductById(1);

            //customer
            var customer = new Customer();

            _priceCalcService.GetFinalPrice(product, customer, 5, false).Should().Be(17.34M);
        }

        [Test]
        public void Can_get_final_product_price_with_discount()
        {
            var product = _productService.GetProductById(1);

            //customer
            var customer = new Customer();

            //discount1.AppliedToProducts.Add(product);
            // ------------------- ------------------ --------------product.AddAppliedDiscounts(discount1);
            //set HasDiscountsApplied property
            product.HasDiscountsApplied = true;
           
            _priceCalcService.GetFinalPrice(product, customer).Should().Be(9.34M);
        }

        [TestCase(12.366, 12.37, RoundingType.Rounding001)]
        [TestCase(12.363, 12.36, RoundingType.Rounding001)]
        [TestCase(12.000, 12.00, RoundingType.Rounding001)]
        [TestCase(12.001, 12.00, RoundingType.Rounding001)]
        [TestCase(12.34, 12.35, RoundingType.Rounding005Up)]
        [TestCase(12.36, 12.40, RoundingType.Rounding005Up)]
        [TestCase(12.35, 12.35, RoundingType.Rounding005Up)]
        [TestCase(12.00, 12.00, RoundingType.Rounding005Up)]
        [TestCase(12.05, 12.05, RoundingType.Rounding005Up)]
        [TestCase(12.20, 12.20, RoundingType.Rounding005Up)]
        [TestCase(12.001, 12.00, RoundingType.Rounding005Up)]
        [TestCase(12.34, 12.30, RoundingType.Rounding005Down)]
        [TestCase(12.36, 12.35, RoundingType.Rounding005Down)]
        [TestCase(12.00, 12.00, RoundingType.Rounding005Down)]
        [TestCase(12.05, 12.05, RoundingType.Rounding005Down)]
        [TestCase(12.20, 12.20, RoundingType.Rounding005Down)]
        [TestCase(12.35, 12.40, RoundingType.Rounding01Up)]
        [TestCase(12.36, 12.40, RoundingType.Rounding01Up)]
        [TestCase(12.00, 12.00, RoundingType.Rounding01Up)]
        [TestCase(12.10, 12.10, RoundingType.Rounding01Up)]
        [TestCase(12.35, 12.30, RoundingType.Rounding01Down)]
        [TestCase(12.36, 12.40, RoundingType.Rounding01Down)]
        [TestCase(12.00, 12.00, RoundingType.Rounding01Down)]
        [TestCase(12.10, 12.10, RoundingType.Rounding01Down)]
        [TestCase(12.24, 12.00, RoundingType.Rounding05)]
        [TestCase(12.49, 12.50, RoundingType.Rounding05)]
        [TestCase(12.74, 12.50, RoundingType.Rounding05)]
        [TestCase(12.99, 13.00, RoundingType.Rounding05)]
        [TestCase(12.00, 12.00, RoundingType.Rounding05)]
        [TestCase(12.50, 12.50, RoundingType.Rounding05)]
        [TestCase(12.49, 12.00, RoundingType.Rounding1)]
        [TestCase(12.50, 13.00, RoundingType.Rounding1)]
        [TestCase(12.00, 12.00, RoundingType.Rounding1)]
        [TestCase(12.01, 13.00, RoundingType.Rounding1Up)]
        [TestCase(12.99, 13.00, RoundingType.Rounding1Up)]
        [TestCase(12.00, 12.00, RoundingType.Rounding1Up)]
        public void Can_round(decimal valueToRoundig, decimal roundedValue, RoundingType roundingType)
        {
            _priceCalcService.Round(valueToRoundig, roundingType).Should().Be(roundedValue);
        }

        #endregion
    }
}