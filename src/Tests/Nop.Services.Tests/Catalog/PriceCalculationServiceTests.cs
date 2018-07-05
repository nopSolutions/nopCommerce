using System;
using System.Collections.Generic;
using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Tests;
using NUnit.Framework;


namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class PriceCalculationServiceTests : ServiceTest
    {
        private Mock<IStoreContext> _storeContext;
        private Mock<IDiscountService> _discountService;
        private Mock<ICategoryService> _categoryService;
        private Mock<IManufacturerService> _manufacturerService;
        private Mock<IProductAttributeParser> _productAttributeParser;
        private Mock<IProductService> _productService;
        private IPriceCalculationService _priceCalcService;
        private ShoppingCartSettings _shoppingCartSettings;
        private CatalogSettings _catalogSettings;
        private IStaticCacheManager _cacheManager;

        private Store _store;

        [SetUp]
        public new void SetUp()
        {
            var serviceProvider = new TestServiceProvider();
            _store = new Store { Id = 1 };
            _storeContext = new Mock<IStoreContext>();
            _storeContext.Setup(x => x.CurrentStore).Returns(_store);

            _discountService = new Mock<IDiscountService>();
            _categoryService = new Mock<ICategoryService>();
            _manufacturerService = new Mock<IManufacturerService>();
            _productService = new Mock<IProductService>();

            _productAttributeParser = new Mock<IProductAttributeParser>();

            _shoppingCartSettings = new ShoppingCartSettings();
            _catalogSettings = new CatalogSettings();

            _cacheManager = new NopNullCache();

            _priceCalcService = new PriceCalculationService(_catalogSettings,
                _categoryService.Object,
                _discountService.Object,
                _manufacturerService.Object,
                _productAttributeParser.Object,
                _productService.Object,
                _cacheManager,
                _storeContext.Object,
                serviceProvider.WorkContext.Object,
                _shoppingCartSettings);

            var nopEngine = new Mock<NopEngine>();

            nopEngine.Setup(x => x.ServiceProvider).Returns(serviceProvider);
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
                CustomerRole = customerRole1
            });
            product.TierPrices.Add(new TierPrice
            {
                Price = 9,
                Quantity = 2,
                Product = product,
                CustomerRole = customerRole2
            });
            product.TierPrices.Add(new TierPrice
            {
                Price = 8,
                Quantity = 5,
                Product= product,
                CustomerRole = customerRole1
            });
            product.TierPrices.Add(new TierPrice
            {
                Price = 5,
                Quantity = 10,
                Product = product,
                CustomerRole = customerRole2
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
            _discountService.Setup(ds => ds.ValidateDiscount(discount1, customer)).Returns(new DiscountValidationResult {IsValid = true});
            _discountService.Setup(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories, null, null, false)).Returns(new List<DiscountForCaching>());
            _discountService.Setup(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers, null, null, false)).Returns(new List<DiscountForCaching>());

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

            _discountService.Setup(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories, null, null, false)).Returns(new List<DiscountForCaching>());
            _discountService.Setup(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers, null, null, false)).Returns(new List<DiscountForCaching>());

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

            _discountService.Setup(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories, null, null, false)).Returns(new List<DiscountForCaching>());
            _discountService.Setup(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers, null, null, false)).Returns(new List<DiscountForCaching>());

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

            _discountService.Setup(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories, null, null, false)).Returns(new List<DiscountForCaching>());
            _discountService.Setup(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers, null, null, false)).Returns(new List<DiscountForCaching>());

            return shoppingCartItem;
        }

        class TestProduct:Product
        {
            public TestProduct()
            {
                _discountProductMappings = new List<DiscountProductMapping>();
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
    }
}