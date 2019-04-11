using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class PriceCalculationServiceTests : ServiceTest
    {
        private Mock<IStoreContext> _storeContext;
        private IDiscountService _discountService;
        private Mock<ICategoryService> _categoryService;
        private Mock<IManufacturerService> _manufacturerService;
        private Mock<IProductAttributeParser> _productAttributeParser;
        private IProductService _productService;
        private IPriceCalculationService _priceCalcService;
        private ShoppingCartSettings _shoppingCartSettings;
        private CatalogSettings _catalogSettings;
        private IStaticCacheManager _cacheManager;
        private Mock<IWorkContext> _workContext;
        private Store _store;
        private TestServiceProvider _serviceProvider;

        [SetUp]
        public new void SetUp()
        {
            _serviceProvider = new TestServiceProvider();
            _store = new Store { Id = 1 };
            _storeContext = new Mock<IStoreContext>();
            _storeContext.Setup(x => x.CurrentStore).Returns(_store);
            
            _categoryService = new Mock<ICategoryService>();
            _manufacturerService = new Mock<IManufacturerService>();
            _productService = TestProductService.Init();
            
            _productAttributeParser = new Mock<IProductAttributeParser>();

            _shoppingCartSettings = new ShoppingCartSettings();
            _catalogSettings = new CatalogSettings();

            _cacheManager = new TestCacheManager();
            _workContext = new Mock<IWorkContext>();

            _discountService = TestDiscountService.Init();

            _priceCalcService = new PriceCalculationService(_catalogSettings, new CurrencySettings{ PrimaryStoreCurrencyId = 1 }, _categoryService.Object,
                _serviceProvider.CurrencyService.Object, _discountService, _manufacturerService.Object, _productAttributeParser.Object,
                _productService, _cacheManager, _storeContext.Object, _workContext.Object, _shoppingCartSettings);

            var nopEngine = new Mock<NopEngine>();

            nopEngine.Setup(x => x.ServiceProvider).Returns(_serviceProvider);
            EngineContext.Replace(nopEngine.Object);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            EngineContext.Replace(null);
        }

        [Test]
        public void Can_get_final_product_price()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true
            };

            //customer
            var customer = new Customer();

            _priceCalcService.GetFinalPrice(product, customer, 0, false).ShouldEqual(12.34M);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 2).ShouldEqual(12.34M);
        }

        [Test]
        public void Can_get_final_product_price_with_tier_prices()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true
            };

            //add tier prices
            product.TierPrices.Add(new TierPrice
            {
                Price = 10,
                Quantity = 2,
                Product = product
            });
            product.TierPrices.Add(new TierPrice
            {
                Price = 9,
                Quantity = 5,
                Product = product,
                StartDateTimeUtc = new DateTime(2010, 01, 03)
            });
            product.TierPrices.Add(new TierPrice
            {
                Price = 8,
                Quantity = 5,
                Product = product,
                StartDateTimeUtc = new DateTime(2027, 01, 03)
            });
            product.TierPrices.Add(new TierPrice
            {
                Price = 5,
                Quantity = 10,
                Product = product,
                StartDateTimeUtc = new DateTime(2010, 01, 03),
                EndDateTimeUtc = new DateTime(2012, 01, 03)
            });
            //set HasTierPrices property
            product.HasTierPrices = true;

            //customer
            var customer = new Customer();

            _priceCalcService.GetFinalPrice(product, customer, 0, false).ShouldEqual(12.34M);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 2).ShouldEqual(10);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 3).ShouldEqual(10);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 5).ShouldEqual(9);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 10).ShouldEqual(9);
        }

        [Test]
        public void Can_get_final_product_price_with_tier_prices_by_customerRole()
        {
            var product = new TestProduct
            {
                Id = 1,
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true
            };

            //customer roles
            var customerRole1 = new CustomerRole
            {
                Id = 1,
                Name = "Some role 1",
                Active = true
            };
            var customerRole2 = new CustomerRole
            {
                Id = 2,
                Name = "Some role 2",
                Active = true
            };

            //add tier prices
            product.TierPrices.Add(new TierPrice
            {
                Price = 10,
                Quantity = 2,
                Product= product,
                CustomerRole = customerRole1,
                CustomerRoleId = customerRole1.Id
            });
            product.TierPrices.Add(new TierPrice
            {
                Price = 9,
                Quantity = 2,
                Product = product,
                CustomerRole = customerRole2,
                CustomerRoleId = customerRole2.Id
            });
            product.TierPrices.Add(new TierPrice
            {
                Price = 8,
                Quantity = 5,
                Product= product,
                CustomerRole = customerRole1,
                CustomerRoleId = customerRole1.Id
            });
            product.TierPrices.Add(new TierPrice
            {
                Price = 5,
                Quantity = 10,
                Product = product,
                CustomerRole = customerRole2,
                CustomerRoleId = customerRole2.Id
            });
            //set HasTierPrices property
            product.HasTierPrices = true;

            //customer
            var customer = new Customer();
            customer.CustomerRoles.Add(customerRole1);

            _priceCalcService.GetFinalPrice(product, customer, 0, false).ShouldEqual(12.34M);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 2).ShouldEqual(10);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 3).ShouldEqual(10);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 5).ShouldEqual(8);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 10).ShouldEqual(8);
        }

        [Test]
        public void Can_get_final_product_price_with_additionalFee()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true
            };

            //customer
            var customer = new Customer();

            _priceCalcService.GetFinalPrice(product, customer, 5, false).ShouldEqual(17.34M);
        }

        [Test]
        public void Can_get_final_product_price_with_discount()
        {
            var product = new TestProduct
            {
                Id = 1,
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true
            };

            //customer
            var customer = new Customer();

            //discounts
            var discount1 = new Discount
            {
                Id = 1,
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToSkus,
                DiscountAmount = 3,
                DiscountLimitation = DiscountLimitationType.Unlimited
            };
            //discount1.AppliedToProducts.Add(product);
            product.AddAppliedDiscounts(discount1);
            //set HasDiscountsApplied property
            product.HasDiscountsApplied = true;
           
            _priceCalcService.GetFinalPrice(product, customer).ShouldEqual(9.34M);
        }

        [Test]
        public void Can_get_shopping_cart_item_unitPrice()
        {
            //customer
            var customer = new Customer();

            //shopping cart
            var product1 = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true
            };
            var sci1 = new ShoppingCartItem
            {
                Customer = customer,
                CustomerId = customer.Id,
                Product= product1,
                ProductId = product1.Id,
                Quantity = 2
            };

            _priceCalcService.GetUnitPrice(sci1).ShouldEqual(12.34);
        }

        [Test]
        public void Can_get_shopping_cart_item_subTotal()
        {
            //customer
            var customer = new Customer();

            //shopping cart
            var product1 = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true
            };
            var sci1 = new ShoppingCartItem
            {
                Customer = customer,
                CustomerId = customer.Id,
                Product= product1,
                ProductId = product1.Id,
                Quantity = 2
            };

            _priceCalcService.GetSubTotal(sci1).ShouldEqual(24.68);
        }

        [Test]
        [TestCase(12.00009, 12.00)]
        [TestCase(12.119, 12.12)]
        [TestCase(12.115, 12.12)]
        [TestCase(12.114, 12.11)]        
        public void Test_GetUnitPrice_WhenRoundPricesDuringCalculationIsTrue_PriceMustBeRounded(decimal inputPrice, decimal expectedPrice)
        {
            // arrange
            var shoppingCartItem = CreateTestShopCartItem(inputPrice);

            // act
            _shoppingCartSettings.RoundPricesDuringCalculation = true;
            var resultPrice = _priceCalcService.GetUnitPrice(shoppingCartItem);

            // assert
            resultPrice.ShouldEqual(expectedPrice);
        }

        [Test]
        [TestCase(12.00009, 12.00009)]
        [TestCase(12.119, 12.119)]
        [TestCase(12.115, 12.115)]
        [TestCase(12.114, 12.114)]
        public void Test_GetUnitPrice_WhenNotRoundPricesDuringCalculationIsFalse_PriceMustNotBeRounded(decimal inputPrice, decimal expectedPrice)
        {
            // arrange            
            var shoppingCartItem = CreateTestShopCartItem(inputPrice);

            // act
            _shoppingCartSettings.RoundPricesDuringCalculation = false;
            var resultPrice = _priceCalcService.GetUnitPrice(shoppingCartItem);

            // assert
            resultPrice.ShouldEqual(expectedPrice);
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
        public void can_round(decimal valueToRoundig, decimal roundedValue, RoundingType roundingType)
        {
            _priceCalcService.Round(valueToRoundig, roundingType).ShouldEqual(roundedValue);
        }

        private ShoppingCartItem CreateTestShopCartItem(decimal productPrice, int quantity = 1)
        {
            //customer
            var customer = new Customer();

            //shopping cart
            var product = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = productPrice,
                CustomerEntersPrice = false,
                Published = true
            };

            var shoppingCartItem = new ShoppingCartItem
            {
                Customer = customer,
                CustomerId = customer.Id,
                Product = product,
                ProductId = product.Id,
                Quantity = quantity
            };

            return shoppingCartItem;
        }

        class TestProduct:Product
        {
            public TestProduct()
            {
                _discountProductMappings = new List<DiscountProductMapping>();
                _tierPrices = new List<TierPrice>();
            }

            public void AddAppliedDiscounts(Discount discount)
            {
                _discountProductMappings.Add(new DiscountProductMapping
                {
                    Discount = discount,
                    DiscountId = discount.Id,
                    Id=1,
                    Product = this
                });
            }
        }

        class TestProductService : ProductService
        {
            private TestProductService(CatalogSettings catalogSettings, CommonSettings commonSettings,
                IAclService aclService, ICacheManager cacheManager, IDataProvider dataProvider,
                IDateRangeService dateRangeService, IDbContext dbContext, IEventPublisher eventPublisher,
                ILanguageService languageService, ILocalizationService localizationService,
                IProductAttributeParser productAttributeParser, IProductAttributeService productAttributeService,
                IRepository<AclRecord> aclRepository, IRepository<CrossSellProduct> crossSellProductRepository,
                IRepository<Product> productRepository, IRepository<ProductPicture> productPictureRepository,
                IRepository<ProductReview> productReviewRepository,
                IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
                IRepository<RelatedProduct> relatedProductRepository,
                IRepository<StockQuantityHistory> stockQuantityHistoryRepository,
                IRepository<StoreMapping> storeMappingRepository, IRepository<TierPrice> tierPriceRepository,
                IStoreService storeService, IStoreMappingService storeMappingService, IWorkContext workContext,
                LocalizationSettings localizationSettings) : base(catalogSettings, commonSettings, aclService,
                cacheManager, dataProvider, dateRangeService, dbContext, eventPublisher, languageService,
                localizationService, productAttributeParser, productAttributeService, aclRepository,
                crossSellProductRepository, productRepository, productPictureRepository, productReviewRepository,
                productWarehouseInventoryRepository, relatedProductRepository, stockQuantityHistoryRepository,
                storeMappingRepository,  tierPriceRepository, storeService, storeMappingService, workContext, localizationSettings)
            {
            }

            public static TestProductService Init()
            {
                return new TestProductService(new CatalogSettings(), new CommonSettings(), null,
                    null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                    null, null, null, null, null, null, new LocalizationSettings());
            }
        }
    }
}